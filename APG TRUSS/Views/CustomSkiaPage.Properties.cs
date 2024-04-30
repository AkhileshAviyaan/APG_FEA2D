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
using Avalonia.Data;
using PanAndZoom;
using APG_FEA2D.Helper;
using System.Collections.Generic;
using Avalonia.Automation.Peers;
namespace APG_FEA2D.Views
{
	public partial class CustomSkiaPage
	{


		/// <summary>
		/// Identifies the <seealso cref="PanButton"/> avalonia property.
		/// </summary>
		public static readonly StyledProperty<ButtonName> PanButtonProperty =
			AvaloniaProperty.Register<CustomSkiaPage, ButtonName>(nameof(CustomSkiaPage), ButtonName.Right, false, BindingMode.TwoWay);
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
		/// Gets or sets pan input button.
		/// </summary>
		public ButtonName PanButton
		{
			get => GetValue(PanButtonProperty);
			set => SetValue(PanButtonProperty, value);
		}
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


		public Dictionary<string, int> DiagramModeDictionary = new Dictionary<string, int>() { { "None", 0 },{"AFD",1}, { "SFD", 2 }, { "BMD", 3 }, { "Displacement", 4 },{"Slope",5} };


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
		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public bool IsNodePressed = false;
		private string _info;
		public string Info
		{
			get=> _info;
			set
			{
				_info = value;
				OnPropertyChanged(nameof(Info));
			}
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
