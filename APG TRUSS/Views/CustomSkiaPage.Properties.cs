using Avalonia.Media.Transformation;
using Avalonia.Media;
using Avalonia;
using FEA2D.CrossSections;
using FEA2D.Materials;
using SkiaSharp;
using Avalonia.Controls;
using System.ComponentModel;
using FEA2D.Structures;
using FEA2D.Loads;
namespace APG_FEA2D.Views
{
	public partial class CustomSkiaPage
	{



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
		public bool AFDOn = false;
		public bool SFDOn = false;
		public bool BMDOn = false;
		public bool DisplacementOn = false;
		public bool ScaledDisplacementOn = false;

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
		public Point OrgCoord;
		public Point Coord;
		public event PropertyChangedEventHandler PropertyChanged;

		public bool IsNodePressed = false;
		private string _info;
		public string Info
		{
			get { return _info; }
			set
			{
				_info = value;
				OnPropertyChanged(nameof(Info));
			}
		}

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

		private void InvalidateProperties()
		{
			SetAndRaise(ZoomProperty, ref _zoom, _matrix.M11);
			SetAndRaise(OffsetXProperty, ref _offsetX, _matrix.M31);
			SetAndRaise(OffsetYProperty, ref _offsetY, _matrix.M32);
		}
	}
}
