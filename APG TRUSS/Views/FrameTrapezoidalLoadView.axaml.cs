using ReactiveUI;
using System;
using Avalonia.ReactiveUI;
using System.Reactive.Linq;
using APG_FEA2D.ViewModels;

namespace APG_FEA2D.Views;
public partial class FrameTrapezoidalLoadView : ReactiveWindow<FrameTrapezoidalLoadViewModel>
{
	public FrameTrapezoidalLoadView()
	{
		InitializeComponent();
		this.WhenActivated(action => action(ViewModel!.FrameTrapezoidalLoadOkCommand.Subscribe(Close)));
	}
}