using Avalonia.Input;
using FEALiTE2D.Elements;
using MatrixHelperList;
using System;
using System.Linq;
using Avalonia;
using Avalonia.Reactive;
namespace APG_TRUSS.Views
{
	public partial class CustomSkiaPage
	{
		protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
		{
			var point = e.GetPosition(this);
			var delta = e.Delta.Y;
			bool skipTransitions = false;
			double ratio = Math.Pow(_zoomSpeed, delta);
			_matrix = MatrixHelper.ScaleAtPrepend(_matrix, ratio, ratio, point.X, point.Y);

		}
		int nodeCountForFrame = 0;
		Node2D firstPoint;
		Node2D secondPoint;
		public Node2D NodeSearch(Point point)
		{
			var list = structure.Nodes;
			Node2D node2D = list.Where(p => (p.X == point.X & p.Y == point.Y)).FirstOrDefault();
			return node2D;
		}
		protected override void OnPointerPressed(PointerPressedEventArgs e)
		{
			OrgCoord = e.GetPosition(this);
			Coord = _grid.DrawableCoord(OrgCoord);
			X = Coord.X;
			Y = Coord.Y;
			if (AddNodeOn == true)
			{
				string nodename = "n" + structure.Nodes.Count;
				structure.AddNode(new Node2D(X, Y, nodename));
				AddNodeOn = false;
			}
			if (AddSupport == true)
			{
				var nodalPoint = NodeSearch(Coord);
				if(nodalPoint!=null) nodalPoint.Support=new NodalSupport(supportType);
				AddSupport = false;
			}
			if (AddFrameOn == true)
			{
				var point=NodeSearch(Coord);
				if(point!=null) nodeCountForFrame++;
				if (nodeCountForFrame == 1) firstPoint = point;
				if (nodeCountForFrame == 2)
				{
					secondPoint = NodeSearch(Coord);
					string framename = "e" + structure.Elements.Count;
					structure.Elements.Add(new FrameElement2D(firstPoint, secondPoint, framename) { CrossSection = section });

					AddFrameOn = false;
					nodeCountForFrame = 0;
				}
			}
		}

		protected override void OnPointerReleased(PointerReleasedEventArgs e)
		{
			base.OnPointerReleased(e);
		}

		protected override void OnPointerMoved(PointerEventArgs e)
		{
			base.OnPointerMoved(e);
		}
	}
}
