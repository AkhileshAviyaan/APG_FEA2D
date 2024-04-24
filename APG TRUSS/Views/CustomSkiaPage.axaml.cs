using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using SkiaSharp;
using System.ComponentModel;
using Avalonia.Media.Transformation;
using Avalonia.Reactive;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using FEALiTE2D.Materials;
using FEALiTE2D.CrossSections;
using FEALiTE2D.Loads;

namespace APG_TRUSS.Views
{
	public partial class CustomSkiaPage : UserControl, INotifyPropertyChanged
	{


		FEALiTE2D.Structure.Structure structure = new FEALiTE2D.Structure.Structure();
		IMaterial material;
		IFrame2DSection section;
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
			FrameCommand=new RelayCommand(Frame_Pressed);
			RollerCommand = new RelayCommand(Rollar_Pressed);
			HingeCommand = new RelayCommand(Hinge_Pressed);
			FixedCommand = new RelayCommand(Fixed_Pressed);

			IMaterial material = new GenericIsotropicMaterial() { E = 30E6, U = 0.2, Label = "Steel", Alpha = 0.000012, Gama = 39885, MaterialType = MaterialType.Steel };
			IFrame2DSection section = new Generic2DSection(0.075, 0.075, 0.075, 0.000480, 0.000480, 0.000480 * 2, 0.1, 0.1, material);

			LoadCase loadCase = new LoadCase("live", LoadCaseType.Live);
			structure.LoadCasesToRun.Add(loadCase);

			structure.LinearMesher.NumberSegements = 20;
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

		private void InvalidateProperties()
		{
			SetAndRaise(ZoomProperty, ref _zoom, _matrix.M11);
			SetAndRaise(OffsetXProperty, ref _offsetX, _matrix.M31);
			SetAndRaise(OffsetYProperty, ref _offsetY, _matrix.M32);
		}
	}
}