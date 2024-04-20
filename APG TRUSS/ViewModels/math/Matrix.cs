using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GnssSharp
{
	public class Matrix
	{
		public int RowCount;
		public int ColumnCount;
		public double[,] Data;

		public Matrix(int row, int column)
		{
			Data = new double[row, column];
			RowCount = row;
			ColumnCount = column;
		}

		public Matrix(Matrix gK)
		{
			RowCount = gK.RowCount;
			ColumnCount = gK.ColumnCount;
			Data = gK.Data.Clone() as double[,];
		}

		public Matrix(double[,] dat)
		{
			Data = dat.Clone() as double[,];
			RowCount = Data.GetUpperBound(0) + 1;
			ColumnCount = Data.GetUpperBound(1) + 1;
		}

		public Matrix(float[,] dat)
		{
			Data = (double[,])dat.Clone();
			RowCount = Data.GetUpperBound(0) + 1;
			ColumnCount = Data.GetUpperBound(1) + 1;
		}

		public Matrix(List<Vector> vectors)
		{
			RowCount = vectors[0].Count;
			ColumnCount = vectors.Count;
			Data = new double[RowCount, ColumnCount];
			Parallel.For(0, vectors.Count, j =>
			{
				for (int i = 0; i < RowCount; i++)
				{
					Data[i, j] = vectors[j][i];
				}
			});
		}

		public Matrix Transpose
		{
			get
			{
				Matrix ret = new Matrix(ColumnCount, RowCount);
				Parallel.For(0, RowCount, i =>
				{
					for (int j = 0; j <= ColumnCount - 1; j++)
					{
						ret.Data[j, i] = this.Data[i, j];
					}
				});
				return ret;
			}
		}


		public static Matrix operator *(Matrix A, Matrix B)
		{
			if (A.ColumnCount != B.RowCount)
				throw new Exception("Illegal Matrix Dimensions");

			Matrix ret = new Matrix(A.RowCount, B.ColumnCount);
			Parallel.For(0, A.RowCount, i =>
			{
				for (int j = 0; j < B.ColumnCount; j++)
				{
					double val = 0;
					for (int k = 0; k < A.ColumnCount; k++)
					{
						val += A.Data[i, k] * B.Data[k, j];
					}
					ret.Data[i, j] = val;
				}
			});
			return ret;
		}


		public static Matrix operator /(Matrix A, double x) => A * (1 / x);

		public static Matrix operator *(Matrix A, double x)
		{
			Matrix ret = new Matrix(A.RowCount, A.ColumnCount);
			Parallel.For(0, A.RowCount, i =>
			{
				for (int j = 0; j < A.ColumnCount; j++)
					ret.Data[i, j] = A.Data[i, j] * x;
			});
			return ret;
		}

		public static Matrix operator *(double x, Matrix A) => A * x;


		public static Matrix operator +(Matrix A, Matrix B)
		{
			if (A.RowCount != B.RowCount | A.ColumnCount != B.ColumnCount)
				throw new Exception("Illegal Matrix Dimensions");

			Matrix ret = new Matrix(A.RowCount, A.ColumnCount);
			Parallel.For(0, A.RowCount, i =>
			{
				for (int j = 0; j <= A.ColumnCount - 1; j++)
				{
					ret.Data[i, j] = A.Data[i, j] + B.Data[i, j];
				}
			});
			return ret;
		}


		public static Matrix operator -(Matrix A, Matrix B)
		{
			if (A.RowCount != B.RowCount | A.ColumnCount != B.ColumnCount)
				throw new Exception("Illegal Matrix Dimensions");

			Matrix ret = new Matrix(A.RowCount, A.ColumnCount);
			Parallel.For(0, A.RowCount, i =>
			{
				for (int j = 0; j <= A.ColumnCount - 1; j++)
				{
					ret.Data[i, j] = A.Data[i, j] - B.Data[i, j];
				}
			});

			return ret;
		}

		public static Matrix Identity(int size)
		{
			Matrix ret = new Matrix(size, size);
			Parallel.For(0, size, i =>
			{
				ret.Data[i, i] = 1;
			});
			return ret;
		}

		public static Matrix Random(int row, int column)
		{
			Random r = new Random();
			Matrix ret = new Matrix(row, column);
			Parallel.For(0, row, i =>
			{
				for (int j = 0; j < column; j++)
					ret.Data[i, j] = r.NextDouble();
			});

			return ret;
		}


		public void SwapColumns(int c1, int c2)
		{
			if (c1 == c2) return;
			for (int i = 0; i < RowCount; i++)
			{
				double tmp = Data[i, c1];
				Data[i, c1] = Data[i, c2];
				Data[i, c2] = tmp;
			}
		}

		public void SwapRows(int r1, int r2)
		{
			if (r1 == r2) return;
			for (int i = 0; i < ColumnCount; i++)
			{
				double tmp = Data[r1, i];
				Data[r1, i] = Data[r2, i];
				Data[r2, i] = tmp;
			}
		}

		public Vector Column(int n)
		{
			Vector ret = new Vector(this.RowCount);
			Parallel.For(0, RowCount, i =>
			{
				ret[i] = this.Data[i, n];
			});
			return ret;
		}


		//public void ScaleColumns()
		//{
		//	Parallel.For(0, ColumnCount, i =>
		//	{
		//		double max = 0;
		//		for (int j = 0; j < RowCount; j++)
		//		{
		//			if (Math.Abs(Data[j, i]) > Math.Abs(max))
		//				max = Data[j, i];
		//		}

		//		for (int j = 0; j < RowCount; j++)
		//		{
		//			Data[j, i] /= max;
		//		}
		//	});
		//}

		public Matrix Diagonal
		{
			get
			{
				if (RowCount != ColumnCount) throw new Exception("Matrix is not square.");
				var data = new double[this.RowCount, this.RowCount];
				for (int i = 0; i < RowCount; i++)
				{
					data[i, i] = Data[i, i];
				}
				return new Matrix(data);
			}
		}

		public Matrix Inverse
		{
			get
			{
				int N = this.RowCount;

				var A = new Matrix(N, N);
				for (int i = 0; i < N; i++)
					for (int j = 0; j < N; j++)
						A.Data[i, j] = this.Data[i, j];

				var B = Matrix.Identity(N);

				//Reduce to diagonal matrix
				for (int k = 0; k < N; k++)
				{
					for (int i = 0; i < N; i++)
					{
						if (i == k) continue;
						var ratio = A.Data[i, k] / A.Data[k, k];
						for (int j = 0; j < N; j++)
						{
							A.Data[i, j] -= ratio * A.Data[k, j];
							B.Data[i, j] -= ratio * B.Data[k, j];
						}
					}
				}

				for (int i = 0; i < N; i++)
				{
					for (int j = 0; j < N; j++)
					{
						B.Data[i, j] /= A.Data[i, i];
					}
				}
				return B;
			}
		}


		/// <summary>
		/// Solve a matrix equation Ax=b using Gauss elimination
		/// </summary>
		/// <param name="b">Right hand vector</param>
		/// <returns>The solution vector x</returns>
		public Vector Solve(Vector b)
		{
			// create copies of the data
			Matrix A = new Matrix(this);
			int N = ColumnCount;
			// Gaussian elimination with partial pivoting
			for (int i = 0; i < N; i++)
			{

				// find pivot row and swap
				int max = i;
				for (int j = i + 1; j < N; j++)
					if (Math.Abs(A.Data[j, i]) > Math.Abs(A.Data[max, i]))
						max = j;
				A.SwapRows(i, max);
				b.SwapRows(i, max);

				// singular
				if (A.Data[i, i] == 0.0)
					throw new Exception("Matrix is singular.");

				// pivot within b
				for (int j = i + 1; j < N; j++)
					b[j] -= b[i] * A.Data[j, i] / A.Data[i, i];

				// pivot within A
				for (int j = i + 1; j < N; j++)
				{
					double m = A.Data[j, i] / A.Data[i, i];
					for (int k = i + 1; k < N; k++)
					{
						A.Data[j, k] -= A.Data[i, k] * m;
					}
					A.Data[j, i] = 0.0;
				}
			}

			// back substitution
			Vector x = new Vector(N);
			for (int j = N - 1; j >= 0; j--)
			{
				double t = 0.0;
				for (int k = j + 1; k < N; k++)
					t += A.Data[j, k] * x[k];
				x[j] = (b[j] - t) / A.Data[j, j];
			}
			return x;

		}

		public string GetCsvTable()
		{
			var builder = new System.Text.StringBuilder();
			for (int i = 0; i < RowCount; i++)
			{
				for (int j = 0; j < ColumnCount; j++)
				{
					builder.Append(Data[i, j] + ",");
				}
				builder.AppendLine();
			}
			return builder.ToString();
		}
	}
}