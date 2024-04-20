using APG_TRUSS.ViewModels;
using APG_TRUSS.Views;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;

namespace APG_TRUSS
{
	public partial class App : Application
	{
		public override void Initialize()
		{
			AvaloniaXamlLoader.Load(this);
		}

		public override void OnFrameworkInitializationCompleted()
		{
			if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
			{
				// Line below is needed to remove Avalonia data validation.
				// Without this line you will get duplicate validations from both Avalonia and CT
				BindingPlugins.DataValidators.RemoveAt(0);
				desktop.MainWindow = new MainWindow
				{
					DataContext = new ZoomBorder(),
				};
			}

			base.OnFrameworkInitializationCompleted();
		}
	}
}