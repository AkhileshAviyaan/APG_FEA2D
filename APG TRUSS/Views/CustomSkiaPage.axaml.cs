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
using FEA2D.Structures;
using FEA2D.Elements;
using Microsoft.Data.Sqlite;

namespace APG_FEA2D.Views
{
	public partial class CustomSkiaPage : UserControl, INotifyPropertyChanged
	{
		public Structure structure;
		public StructureIO structureIO;
		public static readonly string FileCache = "DefaultName";
		IMaterial material;
		IFrame2DSection section;
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
			FrameCommand = new RelayCommand(Frame_Pressed);
			RollerCommand = new RelayCommand(Rollar_Pressed);
			HingeCommand = new RelayCommand(Hinge_Pressed);
			FixedCommand = new RelayCommand(Fixed_Pressed);
			RunCommand = new RelayCommand(Run_Pressed);

			NoneCommand= new RelayCommand(None_Pressed);
			AFDCommand = new RelayCommand(AFD_Pressed);
			SFDCommand = new RelayCommand(SFD_Pressed);
			BMDCommand = new RelayCommand(BMD_Pressed);
			DISPLACEMENTCommand = new RelayCommand(DISPLACEMENT_Pressed);
			SLOPECommand = new RelayCommand(SLOPE_Pressed);

			FileOpenCommand = new RelayCommand(FileOpen_Pressed);
			FileSaveCommand = new RelayCommand(FileSave_Pressed);

            structure = new Structure();
			structureIO = new StructureIO();

            material = new GenericIsotropicMaterial() { E = 2E11, U = 0.2, Label = "Steel", Alpha = 0, Gama = 0, MaterialType = MaterialType.Steel };
			section = new Generic2DSection(0.05, 0.05, 0.05, 0.0001, 0.0001, 0.0001 * 2, 1, 1, material);
			loadCase = new LoadCase("live", LoadCaseType.Live);
			structure.LoadCasesToRun.Add(loadCase);
			structure.LinearMesher.NumberSegements = 20;
		}


	}
}