using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;
using Avalonia;
using Avalonia.Controls;
namespace FEALiTE2D.Elements
{
	public partial class Node2D
	{
		public virtual void Draw(SKCanvas canvas, UserControl viewControl)
		{
			//Vector2D p1 = viewControl.ToDrawableCoord(Location);
			//Paint pnt = new Paint(Selected ? selectedPaint : paint);
			//pnt.SetStyle(Paint.Style.FILL);
			//canvas.DrawCircle((float)p1.X, (float)p1.Y, 7F, pnt);
			//canvas.DrawText(id + "  " + info, (float)p1.X + 10, (float)p1.Y - 7, pnt);
		}
	}
}