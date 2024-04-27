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
		public string _rotation="90";

		[ObservableProperty]
		public string _load="10";
		public NodalLoadViewModel()
		{
			NodalLoadOkCommand = ReactiveCommand.Create(ReturnNodalLoad);
		}
		public NodalLoad ReturnNodalLoad()
		{
			return new NodalLoad(double.Parse(_rotation), double.Parse(_load));
		}
		public ReactiveCommand<Unit, NodalLoad?> NodalLoadOkCommand { get; }
	}
}
