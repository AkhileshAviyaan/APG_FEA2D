using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using Avalonia.ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using APG_FEA2D.ViewModels;
using System;
namespace APG_FEA2D.Views;

public partial class SupportDisplacementLoadView : ReactiveWindow<SupportDisplacementLoadViewModel>
{
    public SupportDisplacementLoadView()
    {
        InitializeComponent();

        this.WhenActivated(action => action(ViewModel!.SupportDiplacementLoadOkCommand.Subscribe(Close)));


	}
}