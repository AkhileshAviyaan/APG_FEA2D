using CommunityToolkit.Mvvm.ComponentModel;
using FEA2D.Loads;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System;

namespace APG_FEA2D.ViewModels
{
	public partial class NodalLoadViewModel:ObservableObject
	{
		[ObservableProperty]
		public NodalLoadReturn _nodalValueReturn;

		[ObservableProperty]
		public string _rotation="10";

		[ObservableProperty]
		public string _load="10";
		public NodalLoadViewModel()
		{
			NodalLoadOkCommand = ReactiveCommand.Create(ReturnViewModel);
		}
		public NodalLoadReturn ReturnViewModel()
		{
			return new NodalLoadReturn(_rotation, _load);
		}
		public ReactiveCommand<Unit, NodalLoadReturn?> NodalLoadOkCommand { get; }

	}
}
