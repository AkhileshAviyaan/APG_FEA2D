using System;
using System.Diagnostics;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using Avalonia.Threading;
using SkiaSharp;
using System.ComponentModel;
using Avalonia.Media.Transformation;
using MatrixHelperList;
using System.Runtime.CompilerServices;
using Avalonia.Reactive;
using Avalonia.Animation;
using FEALiTE2D;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using FEALiTE2D.Elements;
using FEALiTE2D.Materials;
using FEALiTE2D.CrossSections;

namespace APG_TRUSS.Views
{
	public partial class CustomSkiaPage : UserControl, INotifyPropertyChanged
	{


		FEALiTE2D.Structure.Structure structure = new FEALiTE2D.Structure.Structure();

		/// <summary>
		/// Identifies the <seealso cref="Zoom"/> avalonia property.
		/// </summary>
		public static readonly DirectProperty<CustomSkiaPage, double> ZoomProperty =
			AvaloniaProperty.RegisterDirect<CustomSkiaPage, double>(nameof(Zoom), o => o.Zoom, null, 1.0);

		/// <summary>
		/// Identifies the <seealso cref="OffsetX"/> avalonia property.
		/// </summary>
		public static readonly DirectProperty<CustomSkiaPage, double> OffsetXProperty =
			AvaloniaProperty.RegisterDirect<CustomSkiaPage, double>(nameof(OffsetX), o => o.OffsetX, null, 0.0);

		/// <summary>
		/// Identifies the <seealso cref="OffsetY"/> avalonia property.
		/// </summary>
		public static readonly DirectProperty<CustomSkiaPage, double> OffsetYProperty =
			AvaloniaProperty.RegisterDirect<CustomSkiaPage, double>(nameof(OffsetY), o => o.OffsetY, null, 0.0);
		/// <summary>
		/// Gets the zoom ratio
		/// </summary>
		public double Zoom => _zoom;

		/// <summary>
		/// Gets the pan offset for x axis.
		/// </summary>
		public double OffsetX => _offsetX;

		public bool AddNodeOn = false;
		public bool AddFrameOn = false;
		public bool AddMemberSection = false;
		public bool AddInternalHinge = false;
		public bool AddSupport = false;
		public bool AddNodalLoad = false;
		public bool AddFrameLoad = false;
		public ICommand NodeCommand { get;}
		public void Node_Pressed()
		{
			AddNodeOn = true;
		}
		/// <summary>
		/// Gets the pan offset for y axis.
		/// </summary>
		public double OffsetY => _offsetY;
		private double _offsetX;
		private double _offsetY;
		private SKCanvas canvas;
		private bool _captured;
		private Matrix _matrix;
		private TransformOperations.Builder _transformBuilder;
		private Control? _element;
		private double _zoomSpeed;
		public readonly GlyphRun _noSkia;
		private Grid _grid;
		public CustomSkiaPage()
		{
			_captured = false;
			_matrix = Matrix.Identity;
			ClipToBounds = true;
			_zoomSpeed = 1.5;
			var text = "Current rendering API is not Skia";
			var glyphs = text.Select(ch => Typeface.Default.GlyphTypeface.GetGlyph(ch)).ToArray();
			_noSkia = new GlyphRun(Typeface.Default.GlyphTypeface, 12, text.AsMemory(), glyphs);
			_grid = new();
			NodeCommand = new RelayCommand(Node_Pressed);
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		private double _zoom = 2;
		private double _x;
		public double X
		{
			get { return _x; }
			set
			{
				_x = value;
				OnPropertyChanged(nameof(X));
			}
		}
		private double _y;
		public double Y
		{
			get { return _y; }
			set
			{
				_y = value;
				OnPropertyChanged(nameof(Y));
			}
		}
		public IBrush Background
		{
			get => GetValue(BackgroundProperty);
			set => SetValue(BackgroundProperty, value);
		}
		public static readonly StyledProperty<IBrush> BackgroundProperty =
			AvaloniaProperty.Register<CustomSkiaPage, IBrush>(nameof(Background));
		protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
		{
			var point = e.GetPosition(this);
			var delta = e.Delta.Y;
			bool skipTransitions = false;
			double ratio = Math.Pow(_zoomSpeed, delta);
			_matrix = MatrixHelper.ScaleAtPrepend(_matrix, ratio, ratio, point.X, point.Y);

		}
		protected override void OnPointerPressed(PointerPressedEventArgs e)
		{
			Point coord=_grid.DrawableCoord(e.GetPosition(this));
			X=coord.X; 
			Y=coord.Y;
			if(AddNodeOn==true)
			{
				string nodename="n"+structure.Nodes.Count;
				structure.AddNode(new Node2D(X, Y, nodename));
			}
		}

		protected override void OnPointerReleased(PointerReleasedEventArgs e)
		{
			base.OnPointerReleased(e);
		}

		protected override void OnPointerMoved(PointerEventArgs e)
		{
			base.OnPointerMoved(e);
		}


		class CustomDrawOp : ICustomDrawOperation
		{
			private readonly IImmutableGlyphRunReference _noSkia;
			private Matrix _matrix;
			private SKCanvas canvas;
			private CustomSkiaPage customSkiaPage;
			public CustomDrawOp(Rect bounds, Matrix matrix,CustomSkiaPage page)
			{
				Bounds = bounds;
				_matrix = matrix;
				this.canvas = page.canvas;
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
					canvas = lease.SkCanvas;
					customSkiaPage._grid.DrawGrid(canvas,Bounds, _matrix);
				}
			}


		}

		private void InvalidateProperties()
		{
			SetAndRaise(ZoomProperty, ref _zoom, _matrix.M11);
			SetAndRaise(OffsetXProperty, ref _offsetX, _matrix.M31);
			SetAndRaise(OffsetYProperty, ref _offsetY, _matrix.M32);
		}
		public override void Render(DrawingContext context)
		{
			var background = new SolidColorBrush(Color.FromArgb(100,33,33,33));
			if (background != null)
			{
				var renderSize = Bounds.Size;
				context.FillRectangle(background, new Rect(renderSize));
			}
			base.Render(context);
			InvalidateProperties();
			context.Custom(new CustomDrawOp(new Rect(0, 0, Bounds.Width, Bounds.Height), _matrix,this));
			Dispatcher.UIThread.InvokeAsync(InvalidateVisual, DispatcherPriority.Background);
		}
	}
}