using Avalonia.Media;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using Avalonia.Threading;
using Avalonia;
using FEA2D.Elements;
using Avalonia.Platform;
using FEA2D.Structures;
using System;
using SkiaSharp;

namespace APG_FEA2D.Views
{
	public partial class CustomSkiaPage
	{
		class CustomDrawOp : ICustomDrawOperation
		{
			private readonly IImmutableGlyphRunReference _noSkia;
			private Matrix _matrix;
			//private SKCanvas canvas;
			private CustomSkiaPage customSkiaPage;
			public CustomDrawOp(Rect bounds, Matrix matrix, CustomSkiaPage page)
			{
				Bounds = bounds;
				_matrix = matrix;
				//this.canvas = page.canvas;
				customSkiaPage = page;
			}

			public void Dispose()
			{
				// No-op
			}

			public Rect Bounds { get; }
			public bool HitTest(Point p) => false;
			public bool Equals(ICustomDrawOperation other) => false;
			public void Render(ImmediateDrawingContext context)
			{
				var leaseFeature = context.TryGetFeature<ISkiaSharpApiLeaseFeature>();
				if (leaseFeature == null)
					context.DrawGlyphRun(Brushes.Black, _noSkia);
				else
				{
					using var lease = leaseFeature.Lease();
					customSkiaPage.canvas = lease.SkCanvas;
					customSkiaPage._grid.DrawGrid(customSkiaPage.canvas, Bounds, _matrix);
					foreach (var node in customSkiaPage.structure.Nodes)
					{
						node.Draw(customSkiaPage.canvas, customSkiaPage._grid);
						if (node.Support is not null)
						{
							node.DrawSupport(customSkiaPage.canvas, customSkiaPage._grid);
						}

						if (node.NodalLoads.Count > 0)
						{
							node.DrawNodalLoad(customSkiaPage.canvas, customSkiaPage._grid);
						}
					}
					foreach (FrameElement2D element in customSkiaPage.structure.Elements)
					{
						element.Draw(customSkiaPage.canvas, customSkiaPage._grid);
					}
					if (customSkiaPage.structure.AnalysisStatus is AnalysisStatus.Successful && customSkiaPage.AFDOn is true)
					{
						float pixalDisplay = 50;
						double maxForce=0;
						foreach(FrameElement2D element in customSkiaPage.structure.Elements)
						{
							var segments = customSkiaPage.structure.Results.GetElementInternalForces(element, customSkiaPage.loadCase);
							foreach(var segment in segments)
							{
								if (maxForce < Math.Abs(segment.Internalforces1.Fx))
								{
									maxForce=Math.Abs(segment.Internalforces1.Fx);
								}
							}
						}
						foreach(FrameElement2D element in customSkiaPage.structure.Elements)
						{
							var paint = new SKPaint
							{
								Color = SKColors.Blue,
								StrokeWidth = 1,
								IsAntialias = true,
								Style = SKPaintStyle.Stroke
							};

							var segments = customSkiaPage.structure.Results.GetElementInternalForces(element, customSkiaPage.loadCase);
							double maxForceInElement = 0;
							int maxForceIndex=0;
							if (maxForceInElement <= Math.Abs(segments[0].Internalforces1.Fx))
							{
								maxForceInElement = Math.Abs(segments[0].Internalforces1.Fx);
								maxForceIndex = 0;
							}
							for (int i = 0; i< segments.Count;i++)
							{
								if (maxForceInElement <= Math.Abs(segments[i].Internalforces2.Fx))
								{
									maxForceInElement = Math.Abs(segments[i].Internalforces2.Fx);
									maxForceIndex = i;
								}
							}
							int segmentNo = customSkiaPage.structure.LinearMesher.NumberSegements;
							double slopeAngle = Math.Atan((element.EndNode.Y-element.StartNode.Y) / (element.EndNode.X - element.StartNode.X));
							double elementPerpendicularSlopeAngle = Math.Atan(-1 / Math.Tan(slopeAngle));
							Point segmentStartPointInPixel = customSkiaPage._grid.RealDisplayCoord(new Point(element.StartNode.X,element.StartNode.Y)) ;
								double delL = element.Length / segmentNo *customSkiaPage._grid.spacing;
								Point delPointChange = new Point(delL * Math.Cos(slopeAngle), -delL * Math.Sin(slopeAngle));
							for (int i = 0; i < segmentNo; i++)
							{
								double scaledFx1 = segments[i].Internalforces1.Fx / maxForce* pixalDisplay;
								double scaledFx2 = segments[i].Internalforces2.Fx / maxForce* pixalDisplay;
								Point segmentEndPointInPixel = segmentStartPointInPixel + delPointChange;
								Point forceStartPointInPixel = segmentStartPointInPixel + new Point(scaledFx1 * Math.Cos(elementPerpendicularSlopeAngle), -scaledFx1 * Math.Sin(elementPerpendicularSlopeAngle));
								Point forceEndPointInPixel = segmentEndPointInPixel + new Point(scaledFx2 * Math.Cos(elementPerpendicularSlopeAngle), -scaledFx2 * Math.Sin(elementPerpendicularSlopeAngle));
								if (i == 0)
								{
									customSkiaPage.canvas.DrawLine((float)segmentStartPointInPixel.X, (float)segmentStartPointInPixel.Y, (float)forceStartPointInPixel.X, (float)forceStartPointInPixel.Y, paint);
								}
								
								customSkiaPage.canvas.DrawLine( (float)forceStartPointInPixel.X, (float)forceStartPointInPixel.Y, (float)forceEndPointInPixel.X, (float)forceEndPointInPixel.Y, paint);
								customSkiaPage.canvas.DrawLine((float)forceEndPointInPixel.X, (float)forceEndPointInPixel.Y, (float)segmentEndPointInPixel.X, (float)segmentEndPointInPixel.Y, paint);
								segmentStartPointInPixel = segmentEndPointInPixel;
								if (maxForceIndex == i)
								{
									string text = $"{maxForceInElement:f3}";
									customSkiaPage.canvas.DrawText(text, (float)forceEndPointInPixel.X, (float)forceEndPointInPixel.Y, paint);
								}
							}
						}
					}
				}
			}

		}
		public override void Render(DrawingContext context)
		{
			var background = new SolidColorBrush(Color.FromArgb(100, 33, 33, 33));
			if (background != null)
			{
				var renderSize = Bounds.Size;
				context.FillRectangle(background, new Rect(renderSize));
			}
			base.Render(context);
			InvalidateProperties();
			context.Custom(new CustomDrawOp(new Rect(0, 0, Bounds.Width, Bounds.Height), _matrix, this));
			Dispatcher.UIThread.InvokeAsync(InvalidateVisual, DispatcherPriority.Background);
		}
	}
}
