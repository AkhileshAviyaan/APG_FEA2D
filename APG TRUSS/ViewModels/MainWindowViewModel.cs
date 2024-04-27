using APG_FEA2D.ViewModels;
using CommunityToolkit.Mvvm.Input;
using ReactiveUI;
using System.Windows.Input;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace APG_FEA2D.ViewModels
{
	public partial class MainWindowViewModel : ViewModelBase
	{
		public MainWindowViewModel()
		{
			ShowDialog = new Interaction<NodalLoadViewModel, NodalLoadReturn?>();
			NodalLoadCommand = new RelayCommand(NodalLoadCommandMethod);
		}
		public ICommand NodalLoadCommand { get; }
		public async void NodalLoadCommandMethod()
		{
			await ShowDialog.Handle(new NodalLoadViewModel());
		}
		public Interaction<NodalLoadViewModel, NodalLoadReturn?> ShowDialog { get; }
	}
}
