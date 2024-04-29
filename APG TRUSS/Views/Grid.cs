using Avalonia;
using Avalonia.Rendering.SceneGraph;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APG_FEA2D.Views
{
	public class Grid
	{
		public double offsetX;
		public double offsetY;
		public double zoom;
		public float w;
		public float h;
		public float spacing;
		int wNo;
		int hNo;
		float sideExtentW;
		float sideExtentH;
		float sideExtent;
		public void DrawGrid(SKCanvas canvas, Rect Bounds, Matrix matrix)
		{
			canvas.Save();
			offsetX = matrix.M31;
			offsetY = matrix.M32;
			zoom = matrix.M11;
			 w = (float)Bounds.Width;
			 h = (float)Bounds.Height;
			//Point NewBoundStart = MatrixHelper.TransformPoint(matrix, new Point(0, 0));
			//Point NewBoundEnd=MatrixHelper.TransformPoint(matrix, new Point(w, h));

			var paint = new SKPaint
			{
				Color = new SKColor(255, 255, 255, 70),
				StrokeWidth = 1,
				IsAntialias = true,
				Style = SKPaintStyle.Stroke
			};
			var paintText = new SKPaint
			{
				Color = new SKColor(255, 255, 255, 70),
				StrokeWidth = 1,
				IsAntialias = true,
				Style = SKPaintStyle.Stroke
			};

			 spacing = 100;
			 wNo = (int)(w / spacing);
			 hNo = (int)(h / spacing);
			 sideExtentW = (w - (float)(wNo * spacing)) / 2;
			 sideExtentH = (h - (float)(hNo * spacing)) / 2;
			 sideExtent = 0;
			//vertical line
			for (int i = 0; i <= wNo; i++)
			{
				if (i == 0) sideExtent = sideExtentW;
				canvas.DrawLine(sideExtent, 0, sideExtent, h - 4, paint);
				canvas.DrawText($"{TransformCoord(sideExtentW, sideExtent,spacing):F2}", sideExtent + 2, h, paintText);

				sideExtent += spacing;
			}

			//horizontal line
			for (int i = 0; i <= hNo; i++)
			{
				if (i == 0) sideExtent = sideExtentH;
				canvas.DrawLine(0 + 4, sideExtent, w, sideExtent, paint);
				canvas.DrawText($"{TransformCoord(h - sideExtentH, sideExtent,spacing):F2}", 0, sideExtent - 2, paintText);
				sideExtent += spacing;
			}
			//canvas.DrawLine((float)NewBoundStart.X, (float)NewBoundStart.Y, (float)NewBoundEnd.X, (float)NewBoundEnd.Y, paint);
			//canvas.DrawRect((float)NewBoundStart.X, (float)NewBoundStart.Y, (float)NewBoundEnd.X- (float)NewBoundStart.X, (float)NewBoundEnd.Y- (float)NewBoundStart.Y, paint);
			//canvas.DrawLine(0, 0,w,h, paint);
			canvas.Restore();
		}
		public static float TransformCoord(float sideExtent, float actualHeight,float spacing)
		{
			int intPart = (int)Math.Floor(Math.Abs(actualHeight - sideExtent) / spacing);
			float fullNumber = (Math.Abs(actualHeight - sideExtent) / spacing);
			float remainingPart=fullNumber - (float)intPart;
			if (remainingPart > 0.5) intPart++;
			return intPart;
		}
		public Point DrawableCoord(Point point)
		{
			float X=TransformCoord(sideExtentW,(float)point.X,spacing);
			float Y=TransformCoord(h - sideExtentH, (float)point.Y, spacing);
			return new Point(X,Y);
		}
		public Point RealDisplayCoord(Point point)
		{
			float X =(float)point.X* spacing+sideExtentW;
			float Y = h - sideExtentH-(float)point.Y* spacing;
			return new Point(X, Y);
		}
	}

}