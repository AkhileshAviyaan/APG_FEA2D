using APG_FEA2D.ViewModels;
using CommunityToolkit.Mvvm.Input;
using ReactiveUI;
using System.Windows.Input;
using System.Reactive.Linq;
using FEA2D.Loads;
using System.Threading.Tasks;

namespace APG_FEA2D.ViewModels
{
	public partial class MainWindowViewModel : ViewModelBase
	{
		public MainWindowViewModel()
		{
			NodalLoadDialog = new Interaction<NodalLoadViewModel, NodalLoad?>();
			NodalLoadCommand = new RelayCommand(NodalLoadCommandMethod);
			SupportDisplacementLoadCommand = new RelayCommand(SupportDisplacementLoadCommandMethod);
			SupportDisplacementLoadDialog = new Interaction<SupportDisplacementLoadViewModel, SupportDisplacementLoad?>();
			FrameTrapezoidalLoadCommand = new RelayCommand(FrameTrapezoidalLoadCommandMethod);
			FrameTrapezoidalLoadDialog = new Interaction<FrameTrapezoidalLoadViewModel, FrameTrapezoidalLoad?>();
		}
		public ICommand NodalLoadCommand { get; }
		public ICommand SupportDisplacementLoadCommand { get; }
		public ICommand FrameTrapezoidalLoadCommand { get; }
		public async void FrameTrapezoidalLoadCommandMethod()
		{
			var result = await FrameTrapezoidalLoadDialog.Handle(new FrameTrapezoidalLoadViewModel());
		}
		public async void NodalLoadCommandMethod()
		{
			var result = await NodalLoadDialog.Handle(new NodalLoadViewModel());
		}
		public async void SupportDisplacementLoadCommandMethod()
		{
			var result = await SupportDisplacementLoadDialog.Handle(new SupportDisplacementLoadViewModel());
		}
		public Interaction<NodalLoadViewModel, NodalLoad?> NodalLoadDialog { get; }
		public Interaction<SupportDisplacementLoadViewModel, SupportDisplacementLoad?> SupportDisplacementLoadDialog { get; }
		public Interaction<FrameTrapezoidalLoadViewModel, FrameTrapezoidalLoad?> FrameTrapezoidalLoadDialog { get; }
	}
}
