using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Security.Cryptography.X509Certificates;
namespace APG_FEA2D.ViewModels
{
	public partial class NodalLoadReturn:ObservableObject
	{
		[ObservableProperty]
		public double _rotation;

		[ObservableProperty]
		public  double _load;

		public NodalLoadReturn(string rotation, string load)
		{
			_rotation =double.Parse(rotation);
			_load = double.Parse(load);
		}
	}
}
