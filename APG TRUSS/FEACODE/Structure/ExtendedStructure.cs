using APG_FEA2D.Views;
using FEA2D.Elements;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;

namespace FEA2D.Structures
{
	public partial class Structure
	{
		public void DrawAFD(SKCanvas canvas, APG_FEA2D.Views.Grid _grid)
		{
			float pixalDisplay = 50;
			double maxForce = 0;
			foreach (FrameElement2D element in this.Elements)
			{
				var segments = this.Results.GetElementInternalForces(element, this.LoadCasesToRun.FirstOrDefault());
				foreach (var segment in segments)
				{
					if (maxForce < Math.Abs(segment.Internalforces1.Fx))
					{
						maxForce = Math.Abs(segment.Internalforces1.Fx);
					}
				}
			}
			foreach (FrameElement2D element in this.Elements)
			{
				var paint = new SKPaint
				{
					Color = SKColors.Blue,
					StrokeWidth = 1,
					IsAntialias = true,
					Style = SKPaintStyle.Stroke
				};

				var segments = this.Results.GetElementInternalForces(element, this.LoadCasesToRun.FirstOrDefault());
				double maxForceInElement = 0;
				int maxForceIndex = 0;
				if (maxForceInElement <= Math.Abs(segments[0].Internalforces1.Fx))
				{
					maxForceInElement = Math.Abs(segments[0].Internalforces1.Fx);
					maxForceIndex = 0;
				}
				for (int i = 0; i < segments.Count; i++)
				{
					if (maxForceInElement <= Math.Abs(segments[i].Internalforces2.Fx))
					{
						maxForceInElement = Math.Abs(segments[i].Internalforces2.Fx);
						maxForceIndex = i;
					}
				}
				int segmentNo = this.LinearMesher.NumberSegements;
				double slopeAngle = Math.Atan((element.EndNode.Y - element.StartNode.Y) / (element.EndNode.X - element.StartNode.X));
				double elementPerpendicularSlopeAngle = Math.Atan(-1 / Math.Tan(slopeAngle));
				Point segmentStartPointInPixel = _grid.RealDisplayCoord(new Point(element.StartNode.X, element.StartNode.Y));
				double delL = element.Length / segmentNo * _grid.spacing;
				Point delPointChange = new Point(delL * Math.Cos(slopeAngle), -delL * Math.Sin(slopeAngle));
				for (int i = 0; i < segmentNo; i++)
				{
					double scaledFx1 = segments[i].Internalforces1.Fx / maxForce * pixalDisplay;
					double scaledFx2 = segments[i].Internalforces2.Fx / maxForce * pixalDisplay;
					Point segmentEndPointInPixel = segmentStartPointInPixel + delPointChange;
					Point forceStartPointInPixel = segmentStartPointInPixel + new Point(scaledFx1 * Math.Cos(elementPerpendicularSlopeAngle), -scaledFx1 * Math.Sin(elementPerpendicularSlopeAngle));
					Point forceEndPointInPixel = segmentEndPointInPixel + new Point(scaledFx2 * Math.Cos(elementPerpendicularSlopeAngle), -scaledFx2 * Math.Sin(elementPerpendicularSlopeAngle));
					if (i == 0)
					{
						canvas.DrawLine((float)segmentStartPointInPixel.X, (float)segmentStartPointInPixel.Y, (float)forceStartPointInPixel.X, (float)forceStartPointInPixel.Y, paint);
					}

					canvas.DrawLine((float)forceStartPointInPixel.X, (float)forceStartPointInPixel.Y, (float)forceEndPointInPixel.X, (float)forceEndPointInPixel.Y, paint);
					canvas.DrawLine((float)forceEndPointInPixel.X, (float)forceEndPointInPixel.Y, (float)segmentEndPointInPixel.X, (float)segmentEndPointInPixel.Y, paint);
					segmentStartPointInPixel = segmentEndPointInPixel;
					if (maxForceIndex == i)
					{
						string text = $"{maxForceInElement:f3}";
						canvas.DrawText(text, (float)forceEndPointInPixel.X, (float)forceEndPointInPixel.Y, paint);
					}
				}
			}
		}
	}
}
