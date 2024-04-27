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
			ShowDialog = new Interaction<NodalLoadViewModel, NodalLoad?>();
			NodalLoadCommand = new RelayCommand(NodalLoadCommandMethod);
		}
		public ICommand NodalLoadCommand { get; }
		public async void NodalLoadCommandMethod()
		{
			try
			{
				await ShowDialog.Handle(new NodalLoadViewModel());
			}
			catch { }
		}
		public Interaction<NodalLoadViewModel, NodalLoad?> ShowDialog { get; }
	}
}
