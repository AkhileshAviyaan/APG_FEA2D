using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using FEA2D.Loads;
using ReactiveUI;
using System.Reactive;

namespace APG_FEA2D.ViewModels
{
	public partial  class SupportDisplacementLoadViewModel:ObservableObject
	{
		[ObservableProperty]
		public SupportDisplacementLoad supportDisplacementLoad;
		
		[ObservableProperty]
		public string _ux = "0.0";

		[ObservableProperty]
		public string _uy = "0.0";

		[ObservableProperty]
		public string _rz = "0.0";

		public ReactiveCommand<Unit, SupportDisplacementLoad> SupportDiplacementLoadOkCommand { get; }
		public SupportDisplacementLoadViewModel()
		{
			SupportDiplacementLoadOkCommand = ReactiveCommand.Create(ReturnSupportDisplacementLoadCommand);
		}
		public SupportDisplacementLoad ReturnSupportDisplacementLoadCommand()
		{
			return new SupportDisplacementLoad(_ux,_uy,_rz);
		}
	}
}
