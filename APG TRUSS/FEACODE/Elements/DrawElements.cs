using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;
using Avalonia;
using Avalonia.Controls;
using APG_TRUSS.Views;
namespace FEALiTE2D.Elements
{
	public partial class Node2D
	{
		public void Draw(SKCanvas canvas, APG_TRUSS.Views.Grid grid)
		{
			var paint = new SKPaint
			{
				Color = SKColors.OrangeRed,
				StrokeWidth = 1,
				IsAntialias = true,
				Style = SKPaintStyle.Fill
			};
			Point pointRealCoord=grid.RealDisplayCoord(new Point(this.X, this.Y));
			canvas.DrawCircle((float)pointRealCoord.X,(float)pointRealCoord.Y, 3.5F, paint);
			canvas.DrawText(this.Label.Substring(1,1), (float)pointRealCoord.X+10, (float)pointRealCoord.Y+10,paint);
		}
		public void DrawSupport(SKCanvas canvas, APG_TRUSS.Views.Grid grid)
		{
			var paint = new SKPaint
			{
				Color = SKColors.OrangeRed,
				StrokeWidth = 1,
				IsAntialias = true,
				Style = SKPaintStyle.Stroke
			};
			Point pointRealCoord = grid.RealDisplayCoord(new Point(this.X, this.Y));
			if (this.Support.RestraintCount == 1)
			{
				canvas.DrawCircle((float)pointRealCoord.X, (float)pointRealCoord.Y+5F, 5F, paint);
				canvas.DrawLine((float)pointRealCoord.X-10F, (float)pointRealCoord.Y+10F, (float)pointRealCoord.X + 10F, (float)pointRealCoord.Y+10F, paint);
			}
			if (this.Support.RestraintCount == 2)
			{
				canvas.DrawLine((float)pointRealCoord.X - 7.5F, (float)pointRealCoord.Y + 7.5F, (float)pointRealCoord.X + 7.5F, (float)pointRealCoord.Y + 7.5F, paint);
				canvas.DrawLine((float)pointRealCoord.X, (float)pointRealCoord.Y, (float)pointRealCoord.X + 7.5F, (float)pointRealCoord.Y + 7.5F, paint); 
				canvas.DrawLine((float)pointRealCoord.X - 7.5F, (float)pointRealCoord.Y + 7.5F, (float)pointRealCoord.X, (float)pointRealCoord.Y , paint);
			}
			if (this.Support.RestraintCount == 3)
			{
				canvas.DrawLine((float)pointRealCoord.X - 10F, (float)pointRealCoord.Y + 6F, (float)pointRealCoord.X + 10F, (float)pointRealCoord.Y + 6F, paint);
				canvas.DrawLine((float)pointRealCoord.X - 10F, (float)pointRealCoord.Y + 6F, (float)pointRealCoord.X - 13F, (float)pointRealCoord.Y + 12F, paint);
				canvas.DrawLine((float)pointRealCoord.X, (float)pointRealCoord.Y + 6F, (float)pointRealCoord.X-3F, (float)pointRealCoord.Y + 12F, paint);
				canvas.DrawLine((float)pointRealCoord.X + 10F, (float)pointRealCoord.Y + 6F, (float)pointRealCoord.X + 7F, (float)pointRealCoord.Y + 12F, paint);
			}
		}
	}
	public partial class FrameElement2D
	{
		public void Draw(SKCanvas canvas, APG_TRUSS.Views.Grid grid)
		{
			var paint = new SKPaint
			{
				Color = SKColors.OrangeRed,
				StrokeWidth = 1,
				IsAntialias = true,
				Style = SKPaintStyle.Stroke
			};
			Point firstPointRealCoord = grid.RealDisplayCoord(new Point(this.StartNode.X, this.StartNode.Y));
			Point secondPointRealCoord = grid.RealDisplayCoord(new Point(this.EndNode.X, this.EndNode.Y));
			canvas.DrawLine((float)firstPointRealCoord.X, (float)firstPointRealCoord.Y, (float)secondPointRealCoord.X, (float)secondPointRealCoord.Y, paint);
		}
	}
	public partial class NodalSupport
	{

	}
}