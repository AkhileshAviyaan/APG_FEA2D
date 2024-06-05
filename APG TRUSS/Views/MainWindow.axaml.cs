using ReactiveUI;
using System.Threading.Tasks;
using APG_FEA2D.ViewModels;
using Avalonia.ReactiveUI;
using FEA2D.Loads;
namespace APG_FEA2D.Views
{
	public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
	{
		public NodalLoad nodalLoad;
		public SupportDisplacementLoad supportDisplacementLoad;
		public FrameTrapezoidalLoad frameTrapezoidalLoad;

		public MainWindow()
		{
			InitializeComponent();
			this.WhenActivated(action =>
						   action(ViewModel!.NodalLoadDialog.RegisterHandler(NodalLoadDialogAsync)));
			this.WhenActivated(action =>
			   action(ViewModel!.SupportDisplacementLoadDialog.RegisterHandler(SupportDisplacementLoadDialogAsync)));
			this.WhenActivated(action =>
			   action(ViewModel!.FrameTrapezoidalLoadDialog.RegisterHandler(FrameTrapezoidalLoadDialogAsync)));
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
				nodalLoad.LoadCase = CustomSkia.structure.LoadCasesToRun[0];
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
		private async Task FrameTrapezoidalLoadDialogAsync(IInteractionContext<FrameTrapezoidalLoadViewModel,
										FrameTrapezoidalLoad?> interaction)
		{
			var dialog = new FrameTrapezoidalLoadView();
			dialog.DataContext = interaction.Input;
			if (CustomSkia.frameGet is null)
			{
				CustomSkia.Info = "Please Select Frame First";
				interaction.SetOutput(new FrameTrapezoidalLoad());
				return;
			}
			else
			{
				CustomSkia.Info = "Input FrameLoad";
				frameTrapezoidalLoad = await dialog.ShowDialog<FrameTrapezoidalLoad?>(this);
				CustomSkia.Info = "FrameLoad Added";

				interaction.SetOutput(frameTrapezoidalLoad);
				if (frameTrapezoidalLoad is not null)
				{
					frameTrapezoidalLoad.LoadCase = CustomSkia.loadCase;
					CustomSkia.frameGet.Loads.Add(frameTrapezoidalLoad);
				}
				return;
			}
		}
	}
}