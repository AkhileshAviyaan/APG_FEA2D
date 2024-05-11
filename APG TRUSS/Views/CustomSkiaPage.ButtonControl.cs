using System;
using System.Windows.Input;
using FEA2D.Elements;
using FEA2D.Structures;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace APG_FEA2D.Views
{
	public partial class CustomSkiaPage
	{
		public SupportName supportType;
		public string DiagramMode;
        public ICommand FileOpenCommand { get; }
        public void FileOpen_Pressed()
        {
        }
        public ICommand FileSaveCommand { get; }
        public void FileSave_Pressed()
        {
        }

        public ICommand NoneCommand { get; }
		public void None_Pressed()
		{
			DiagramMode = "None";
		}
		public ICommand AFDCommand { get; }
		public void AFD_Pressed()
		{
			DiagramMode = "AFD";

		}
		public ICommand SFDCommand { get; }
		public void SFD_Pressed()
		{
			DiagramMode = "SFD";

		}
		public ICommand BMDCommand { get; }
		public void BMD_Pressed()
		{
			DiagramMode = "BMD";

		}
		public ICommand DISPLACEMENTCommand { get; }
		public void DISPLACEMENT_Pressed()
		{
			DiagramMode = "Displacement";

		}

		public ICommand SLOPECommand { get; }
		public void SLOPE_Pressed()
		{
			DiagramMode = "Slope";
		}
		public ICommand NodeCommand { get; }
		public void Node_Pressed()
		{
			AddNodeOn = true;
			AddFrameOn = false;
			AddSupport = false;
		}
		public ICommand FrameCommand { get; }
		public void Frame_Pressed()
		{
			AddFrameOn = true;
			AddSupport = false;
			AddNodeOn = false;
		}

		public ICommand RollerCommand { get; }
		public void Rollar_Pressed()
		{
			supportType = SupportName.Roller;
			AddSupport = true;
			AddNodeOn = false;
			AddFrameOn = false;

		}
		public ICommand HingeCommand { get; }
		public void Hinge_Pressed()
		{
			supportType = SupportName.Hinge;
			AddSupport = true;
			AddNodeOn = false;
			AddFrameOn = false;
		}
		public ICommand FixedCommand { get; }
		public void Fixed_Pressed()
		{
			supportType = SupportName.Fixed;
			AddSupport = true;
			AddNodeOn = false;
			AddFrameOn = false;
		}
		public ICommand RunCommand { get; }
		public void Run_Pressed()
		{
			if (this.structure.Nodes.Count <2 && this.structure.Elements.Count < 1) return;
			if (this.structure.AlreadyRun is true) { Info = "Analysis AlreadyRun"; return; }
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

				this.structure.Solve();
				var R1 = structure.Results.GetSupportReaction(firstPoint, loadCase);

				if (structure.AnalysisStatus is FEA2D.Structures.AnalysisStatus.Failure)
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
