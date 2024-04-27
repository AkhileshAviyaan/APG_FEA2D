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

			interaction.SetOutput(await dialog.ShowDialog<NodalLoadReturn?>(this));

		}
	}
}