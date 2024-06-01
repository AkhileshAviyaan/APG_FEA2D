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
using Avalonia.Media.TextFormatting;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
						node.DrawNode(customSkiaPage.canvas, customSkiaPage._grid);
						if (node.Support is not null)
						{
							node.DrawSupport(customSkiaPage.canvas, customSkiaPage._grid);
							if (customSkiaPage.structure.AnalysisStatus is AnalysisStatus.Successful)
							{
								node.force = customSkiaPage.structure.Results.GetSupportReaction(node, customSkiaPage.structure.LoadCasesToRun[0]);
								node.DrawReaction(customSkiaPage.canvas, customSkiaPage._grid);
							}
						}

						if (node.NodalLoads.Count > 0)
						{
							node.DrawNodalLoad(customSkiaPage.canvas, customSkiaPage._grid);
						}
						if(node.SupportDisplacementLoad.Count > 0)
						{
							//TO DO
							//node.DrawSupportDisplacementLoad();
						}
					}
					foreach (FrameElement2D element in customSkiaPage.structure.Elements)
					{
						element.Draw(customSkiaPage.canvas, customSkiaPage._grid);
						if (element.Loads.Count > 0 && (customSkiaPage.structure.AnalysisStatus is AnalysisStatus.Failure ||customSkiaPage.DiagramMode=="None"))
						{
							element.DrawTrapezoidalLoad(customSkiaPage.canvas, customSkiaPage._grid);
						}
					}
					if (customSkiaPage.structure.AnalysisStatus is AnalysisStatus.Successful && customSkiaPage.DiagramMode is not null)

					{

						int drawCase = customSkiaPage.DiagramModeDictionary[customSkiaPage.DiagramMode];
						if (drawCase == 0)
						{
							customSkiaPage.structure.DrawResult(customSkiaPage.canvas, customSkiaPage._grid, "None");
						}
						else if (drawCase == 1)
						{
							customSkiaPage.structure.DrawResult(customSkiaPage.canvas, customSkiaPage._grid, "Fx");
						}
						else if (drawCase == 2)
						{
							customSkiaPage.structure.DrawResult(customSkiaPage.canvas, customSkiaPage._grid, "Fy");
						}
						else if (drawCase == 3)
						{
							customSkiaPage.structure.DrawResult(customSkiaPage.canvas, customSkiaPage._grid, "Mz");
						}
						else if (drawCase == 4)
						{
							customSkiaPage.structure.DrawResult(customSkiaPage.canvas, customSkiaPage._grid, "Ux");
						}
						else if (drawCase == 5)
						{
							customSkiaPage.structure.DrawResult(customSkiaPage.canvas, customSkiaPage._grid, "Rz");
						}
						//customSkiaPage.structure.DrawResult(customSkiaPage.canvas, customSkiaPage._grid);
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
