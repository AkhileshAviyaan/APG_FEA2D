using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GnssSharp
{
	/// <summary>
	/// A Column Matrix of arbitrary size
	/// </summary>
	public class Vector
	{
		public int Count { get => Data.Length; }
		public double[] Data;

		public Vector(int n, double defaultValue = 0)
		{
			Data = new double[n];
			for (int i = 0; i < n; i++)
			{
				Data[i] = defaultValue;
			}
		}
		public Vector(double[] data)
		{
			Data = data.Clone() as double[];
		}

		public double this[int i]
		{
			get => Data[i];
			set => Data[i] = value;
		}
		public Vector Clone()
		{
			return new Vector(this.Data);
		}

		public static Vector operator *(Vector b, double x)
		{
			Vector ret = new Vector(b.Count);
			for (int i = 0; i < b.Count; i++)
			{
				ret[i] = b[i] * x;
			}
			return ret;
		}

		public static Vector operator /(Vector b, double x) => b * (1 / x);
		public static Vector operator *(double x, Vector b) => b * x;


		public static Vector operator +(Vector a, Vector b)
		{
			if (a == null) return b;
			if (b == null) return a;
			Vector ret = new Vector(a.Count);
			for (int i = 0; i < a.Count; i++) ret[i] = a[i] + b[i];
			return ret;
		}


		public static Vector operator -(Vector a, Vector b)
		{
			Vector ret = new Vector(a.Count);
			for (int i = 0; i < a.Count; i++) ret[i] = a[i] - b[i];
			return ret;
		}


		/// <summary>
		/// Returns the outer product a * Tr(b) of two vectors a and b.
		/// </summary>
		/// <param name="a">The first vector</param>
		/// <param name="b">the second vector</param>
		/// <returns>the outer product</returns>
		public static Matrix OuterProduct(Vector a, Vector b)
		{
			Matrix ret = new Matrix(a.Count, b.Count);
			for (int i = 0; i < a.Count; i++)
			{
				for (int j = 0; j < b.Count; j++)
					ret.Data[i, j] = a[i] * b[j];
			}
			return ret;
		}

		public double Magnitude
		{
			get
			{
				double sqrSum = 0;
				foreach (double d in Data)
				{
					sqrSum += d * d;
				}
				return Math.Sqrt(sqrSum);
			}
		}

		public Vector Normalize()
		{
			Vector ret = this.Clone();
			double mag = Magnitude;
			Parallel.For(0, Count, i =>
			{
				ret[i] /= mag;
			});
			return ret;
		}


		public double Dot(Vector other)
		{
			double sum = 0;
			for (int i = 0; i < Count; i++)
			{
				sum += this[i] * other[i];
			}
			return sum;
		}

		/// <summary>
		/// Multiplies two vectors element by element
		/// </summary>
		/// <param name="v2">The vector to multiply this vector with</param>
		/// <returns>The product</returns>
		public Vector ElementProduct(Vector v2)
		{
			if (Count != v2.Count) throw new Exception("Invalid vector dimensions");
			var ret = this.Clone();
			Parallel.For(0, v2.Count, i =>
			{
				ret[i] *= v2.Data[i];
			});
			return ret;

		}

		/// <summary>
		/// Premultiplies a vector with a matrix
		/// </summary>
		/// <param name="A">The matrix</param>
		/// <param name="b">The vector</param>
		/// <returns>The product as a column vector</returns>
		public static Vector operator *(Matrix A, Vector b)
		{
			if (A.ColumnCount != b.Count) throw new Exception("Invalid matrix dimensions");
			Vector ret = new Vector(A.RowCount);

			Parallel.For(0, A.RowCount, i =>
			{
				for (int j = 0; j < A.ColumnCount; j++)
				{
					ret[i] += A.Data[i, j] * b[j];
				}
			});

			return ret;
		}



		/// <summary>
		/// Premultiplies a matrix with the transpose of a vector
		/// </summary>
		/// <param name="A">The matrix</param>
		/// <param name="b">The vector</param>
		/// <returns>The product as a column vector</returns>
		public static Vector operator *(Vector b, Matrix A)
		{
			if (A.ColumnCount != b.Count) throw new Exception("Invalid matrix dimensions");
			Vector ret = new Vector(b.Count);
			Parallel.For(0, A.ColumnCount, i =>
			{
				for (int j = 0; j < b.Count; j++)
				{
					ret[i] += A.Data[j, i] * b[j];
				}
			});

			return ret;
		}


		public void SwapRows(int r1, int r2)
		{
			double temp = this[r1];
			this[r1] = this[r2];
			this[r2] = temp;
		}
	}
}