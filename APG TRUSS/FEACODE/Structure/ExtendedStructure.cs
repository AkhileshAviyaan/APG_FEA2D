﻿using APG_FEA2D.Views;
using FEA2D.Elements;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using System.Reflection;

namespace FEA2D.Structures
{
	public partial class Structure
	{
		public SKPaint paint = new SKPaint
		{
			Color = SKColors.Blue,
			StrokeWidth = 1,
			IsAntialias = true,
			Style = SKPaintStyle.Stroke
		};

		public void DrawResult(SKCanvas canvas, APG_FEA2D.Views.Grid _grid, String loadName)
		{
			float pixalDisplay = 50;
			double maxForce = 0;
			PropertyInfo forceProperty;
			if (loadName == "None") { 
				return;
			}
			if (loadName.StartsWith("U") || loadName.StartsWith("R"))
			{
				forceProperty = typeof(Displacement).GetProperty(loadName);
			}
			else
			{
				forceProperty = typeof(Force).GetProperty(loadName);
			}
			foreach (FrameElement2D element in this.Elements)
			{
				var segments = this.Results.GetElementInternalForces(element, this.LoadCasesToRun.FirstOrDefault());
				foreach (var segment in segments)
				{
					double forceValue1 = 0;
					if (loadName.StartsWith("U") || loadName.StartsWith("R"))
					{
						forceValue1 = (double)forceProperty.GetValue(segment.Displacement1);
					}
					else
					{
						forceValue1 = (double)forceProperty.GetValue(segment.Internalforces1);
					}
					if (maxForce < Math.Abs(forceValue1))
					{
						maxForce = Math.Abs(forceValue1);
					}
				}
			}
			foreach (FrameElement2D element in this.Elements)
			{

				var segments = this.Results.GetElementInternalForces(element, this.LoadCasesToRun.FirstOrDefault());
				double maxForceInElement = 0;
				int maxForceIndex = 0;
				double forceValue1 = 0;
				double forceValue2 = 0;

				if (loadName.StartsWith("U") || loadName.StartsWith("R"))
				{
					forceValue1 = (double)forceProperty.GetValue(segments[0].Displacement1);
				}
				else
				{
					forceValue1 = (double)forceProperty.GetValue(segments[0].Internalforces1);
				}
				if (maxForceInElement <= Math.Abs(forceValue1))
				{
					maxForceInElement = Math.Abs(forceValue1);
					maxForceIndex = 0;
				}
				for (int i = 0; i < segments.Count; i++)
				{
					if (loadName.StartsWith("U") || loadName.StartsWith("R"))
					{
						forceValue2 = (double)forceProperty.GetValue(segments[i].Displacement2);
					}
					else
					{
						forceValue2 = (double)forceProperty.GetValue(segments[i].Internalforces2);
					}
					if (maxForceInElement <= Math.Abs(forceValue2))
					{
						maxForceInElement = Math.Abs(forceValue2);
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
					if (loadName.StartsWith("U") || loadName.StartsWith("R"))
					{
						forceValue1 = (double)forceProperty.GetValue(segments[i].Displacement1);
						forceValue2 = (double)forceProperty.GetValue(segments[i].Displacement2);
					}
					else
					{
						forceValue1 = (double)forceProperty.GetValue(segments[i].Internalforces1);
						forceValue2 = (double)forceProperty.GetValue(segments[i].Internalforces2);
					}
					double scaledFx1 = forceValue1 / maxForce * pixalDisplay;
					double scaledFx2 = forceValue2/ maxForce * pixalDisplay;
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
