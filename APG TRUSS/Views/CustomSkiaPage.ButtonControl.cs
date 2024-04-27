using System.Windows.Input;
using FEA2D.Elements;
using FEA2D.Structures;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace APG_FEA2D.Views
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
		public ICommand RunCommand { get; }
		public void Run_Pressed()
		{
			if (this.structure.Nodes.Count <2 && this.structure.Elements.Count < 1) return;

			int restrainCount = 0;
			foreach (var node in this.structure.Nodes)
			{
				if(node.Support is not null)
				{
					restrainCount += node.Support.RestraintCount;
				}
			}
			if (restrainCount < 3)
			{
				Info = "Cannot Be Solved";
				return;
			}
			try
			{
				this.structure.Solve();
			}
			catch
			{
				if (structure.AnalysisStatus is not FEA2D.Structures.AnalysisStatus.Failure)
				{
					Info = "Analysis Failure";
					return;
				}
				else
				{
					Info = "Analysis Successful";
				}
			}
			
		}
	}
}
