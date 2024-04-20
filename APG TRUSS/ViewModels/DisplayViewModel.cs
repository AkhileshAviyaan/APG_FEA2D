using Avalonia.Controls.PanAndZoom;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APG_TRUSS.ViewModels
{
	public partial class DisplayViewModel : ViewModelBase
	{
		[ObservableProperty]
		public double _ZoomX;

		[RelayCommand]
		public void Zoom(ZoomBorder border)
		{
			ZoomX= border.ZoomX;
		}
	}
}
