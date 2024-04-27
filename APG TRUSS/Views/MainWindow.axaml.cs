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
		public NodalLoad nodalLoad;
	
		public MainWindow()
		{
			InitializeComponent();
			this.WhenActivated(action =>
						   action(ViewModel!.ShowDialog.RegisterHandler(DoShowDialogAsync)));
		}

		private async Task DoShowDialogAsync(IInteractionContext<NodalLoadViewModel,
										NodalLoad?> interaction)
		{
			var dialog = new NodalLoadView();
			dialog.DataContext = interaction.Input;
			if (CustomSkia.IsNodePressed is false) 
			{
				CustomSkia.Info = "Please Select Node First";
				interaction.SetOutput(new NodalLoad());
				return;
			}
			nodalLoad = await dialog.ShowDialog<NodalLoad?>(this);
			if (nodalLoad is not null)
			{
				nodalLoad.LoadCase = CustomSkia.loadCase;
				CustomSkia.searchedPoint.NodalLoads.Add(nodalLoad);
			}
			interaction.SetOutput(nodalLoad);
		}
	}
}