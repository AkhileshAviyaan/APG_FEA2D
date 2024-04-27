using ReactiveUI;
using System.Threading.Tasks;
using APG_FEA2D.ViewModels;
using Avalonia.ReactiveUI;
using APG_FEA2D.Views;
using APG_FEA2D.ViewModels;
using System;
using Microsoft.VisualBasic;


using Avalonia.ReactiveUI;
using ReactiveUI;
using System.Threading.Tasks;
using FEA2D.Loads;
namespace APG_FEA2D.Views
{
	public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
	{
		public NodalLoadReturn nodalLoad;
		public MainWindow()
		{
			InitializeComponent();
			this.WhenActivated(action =>
						   action(ViewModel!.ShowDialog.RegisterHandler(DoShowDialogAsync)));
		}

		private async Task DoShowDialogAsync(IInteractionContext<NodalLoadViewModel,
										NodalLoadReturn?> interaction)
		{
			var dialog = new NodalLoadView();
			dialog.DataContext = interaction.Input;
			if (CustomSkia.IsNodePressed is false) 
			{
				CustomSkia.Info = "Please Select Node First";
				return;
			}
			nodalLoad = await dialog.ShowDialog<NodalLoadReturn?>(this);
			CustomSkia.searchedPoint.NodalLoads.Add(loadCalculation());
			interaction.SetOutput(nodalLoad);

		}
		public NodalLoad loadCalculation()
		{
			double x=nodalLoad.Load*Math.Cos(nodalLoad.Rotation*Math.PI/180);
			double y=-nodalLoad.Load*Math.Sin(nodalLoad.Rotation*Math.PI/180);
			return new NodalLoad(x, y, 0, LoadDirection.Local, CustomSkia.loadCase);
		}
	}
}