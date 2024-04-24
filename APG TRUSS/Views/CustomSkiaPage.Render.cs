using Avalonia.Media;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using Avalonia.Threading;
using Avalonia;
using FEALiTE2D.Elements;
using Avalonia.Platform;

namespace APG_TRUSS.Views
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
						if (node.Support != null)
						{
							node.DrawSupport(customSkiaPage.canvas, customSkiaPage._grid);
						}
					}
					foreach (FrameElement2D element in customSkiaPage.structure.Elements)
					{
						element.Draw(customSkiaPage.canvas, customSkiaPage._grid);
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
