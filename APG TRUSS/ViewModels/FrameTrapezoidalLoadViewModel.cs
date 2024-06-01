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
	public partial class FrameTrapezoidalLoadViewModel:ObservableObject
	{ 
		[ObservableProperty]
		public string _wy1="10";
		[ObservableProperty]
		public string _wy2 = "10";

		public FrameTrapezoidalLoadViewModel()
		{
			FrameTrapezoidalLoadOkCommand = ReactiveCommand.Create(ReturnFrameTrapezoidalLoad);
		}
		public FrameTrapezoidalLoad ReturnFrameTrapezoidalLoad()
		{
			return new FrameTrapezoidalLoad(0,0,double.Parse(_wy1), double.Parse(_wy2),LoadDirection.Local);
		}
		public ReactiveCommand<Unit, FrameTrapezoidalLoad?> FrameTrapezoidalLoadOkCommand { get; }
	}
}
