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
		public SupportDisplacementLoad supportDisplacementLoad;

		public MainWindow()
		{
			InitializeComponent();
			this.WhenActivated(action =>
						   action(ViewModel!.NodalLoadDialog.RegisterHandler(NodalLoadDialogAsync)));
			this.WhenActivated(action =>
			   action(ViewModel!.SupportDisplacementLoadDialog.RegisterHandler(SupportDisplacementLoadDialogAsync)));
		}

		private async Task NodalLoadDialogAsync(IInteractionContext<NodalLoadViewModel,
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
		private async Task SupportDisplacementLoadDialogAsync(IInteractionContext<SupportDisplacementLoadViewModel,
										SupportDisplacementLoad?> interaction)
		{
			var dialog = new SupportDisplacementLoadView();
			dialog.DataContext = interaction.Input;
			if (CustomSkia.IsNodePressed is false)
			{
				CustomSkia.Info = "Please Select Node First";
				interaction.SetOutput(new SupportDisplacementLoad());
				return;
			}
			else
			{
				if (CustomSkia.searchedPoint.IsFree is true)
				{
					CustomSkia.Info = "No Support in Selected Node";
					interaction.SetOutput(new SupportDisplacementLoad());
					return;
				}
				else
				{
					CustomSkia.Info = "Input Support Displacement";
					supportDisplacementLoad = await dialog.ShowDialog<SupportDisplacementLoad?>(this);
					CustomSkia.Info = "Support Displacement Added";

					interaction.SetOutput(supportDisplacementLoad);
					if (supportDisplacementLoad is not null)
					{
						supportDisplacementLoad.LoadCase = CustomSkia.loadCase;
						CustomSkia.searchedPoint.SupportDisplacementLoad.Add(supportDisplacementLoad);
					}
					return;
				}
			}
		}
	}
}