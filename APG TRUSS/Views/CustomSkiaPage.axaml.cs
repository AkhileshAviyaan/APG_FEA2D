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
using FEA2D.Materials;
using FEA2D.CrossSections;
using FEA2D.Loads;

namespace APG_FEA2D.Views
{
	public partial class CustomSkiaPage : UserControl, INotifyPropertyChanged
	{

	public LoadCase loadCase;
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

			 material = new GenericIsotropicMaterial() { E = 30E6, U = 0.2, Label = "Steel", Alpha = 0.000012, Gama = 39885, MaterialType = MaterialType.Steel };
			 section = new Generic2DSection(0.075, 0.075, 0.075, 0.000480, 0.000480, 0.000480 * 2, 0.1, 0.1, material);

			loadCase = new LoadCase("live", LoadCaseType.Live);
			structure.LoadCasesToRun.Add(loadCase);

			structure.LinearMesher.NumberSegements = 20;
		}

		
	}
}