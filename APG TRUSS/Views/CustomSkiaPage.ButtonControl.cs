using System.Windows.Input;
using FEALiTE2D.Elements;

namespace APG_TRUSS.Views
{
	public partial class CustomSkiaPage
	{
		public SupportName supportType; 
		public ICommand NodeCommand { get; }
		public void Node_Pressed()
		{
			AddNodeOn = true;
		}
		public ICommand FrameCommand { get; }
		public void Frame_Pressed()
		{
			AddFrameOn = true;
		}
		
		public ICommand RollerCommand { get; }
		public void Rollar_Pressed()
		{
			supportType = SupportName.Roller;
			AddSupport = true;
		}
		public ICommand HingeCommand { get; }
		public void Hinge_Pressed()
		{
			supportType = SupportName.Hinge;
			AddSupport = true;
		}
		public ICommand FixedCommand { get; }
		public void Fixed_Pressed()
		{
			supportType = SupportName.Fixed;
			AddSupport = true;
		}
	}
}
