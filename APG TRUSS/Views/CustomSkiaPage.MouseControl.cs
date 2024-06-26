﻿using Avalonia.Input;
using FEA2D.Elements;
using MatrixHelperList;
using System;
using System.Linq;
using Avalonia;
using Avalonia.Reactive;
using PanAndZoom;
using APG_FEA2D.Helper;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Reactive.Concurrency;
using System.Runtime.InteropServices;
using Spatial;
using Avalonia.Controls.Shapes;
using System.Xml.Linq;
using DynamicData;
using System.Collections.Generic;
using FEA2D.Structures;
namespace APG_FEA2D.Views
{
	public partial class CustomSkiaPage
	{
		int spacingIndexForGrid = 5;
		float initialDistanceX;
		float initialDistanceY;
				protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
		{
			var point = e.GetPosition(this);
			var point2D = (Point2D)point;
			var delta = (float)e.Delta.Y;

			float diffx = CurrentMovingPoint.X - _grid.OriginX;
			float diffy = CurrentMovingPoint.Y - _grid.OriginY;
			float spacingBeforeZoom = (this._grid.spacing / Grid.SpacingEquivalentInGrid);
			_grid.OriginX += diffx;
			_grid.OriginY += diffy;

			this._grid.spacing += delta * 20;
			if (this._grid.spacing < 80)
			{
				spacingIndexForGrid--;
				if (spacingIndexForGrid >= 0)
				{
					Grid.SpacingEquivalentInGrid = this._grid.SpacingEquivalentInGridList[spacingIndexForGrid];
					this._grid.spacing = 160;
				}

				else
				{
					this._grid.spacing = 80;
					spacingIndexForGrid = 0;
				}
			}

			if (this._grid.spacing > 160)
			{

				spacingIndexForGrid++;
				if (spacingIndexForGrid < this._grid.SpacingEquivalentInGridList.Length)
				{
					Grid.SpacingEquivalentInGrid = this._grid.SpacingEquivalentInGridList[spacingIndexForGrid];
					this._grid.spacing = 80;
				}
				else
				{
					this._grid.spacing = 160;
					spacingIndexForGrid = this._grid.SpacingEquivalentInGridList.Length - 1;
				}
			}
			float spacingAfterZoom = (this._grid.spacing / Grid.SpacingEquivalentInGrid);
			float diffxAfterZoom = diffx*spacingAfterZoom/spacingBeforeZoom;
			float diffyAfterZoom = diffy* spacingAfterZoom / spacingBeforeZoom;
			_grid.OriginX -= diffxAfterZoom;
			_grid.OriginY -= diffyAfterZoom;
		}
		int nodeCountForFrame = 0;
		Node2D firstPoint;
		Node2D secondPoint;
		public Node2D NodeSearch(Avalonia.Point point)
		{
			var list = structure.Nodes;
			Node2D node2D = list.Where(p => (p.X == point.X & p.Y == point.Y)).FirstOrDefault();
			return node2D;
		}
		public void InfoUpdate()
		{
			if (IsNodePressed is true)
			{
				Info = $"Node {searchedPoint.Label} Pressed";
			}
			else
			{
				Info = "";
			}
		}
		public Node2D searchedPoint;
		public bool IsPanning = false;

		public Point2D _previousPoint;
		public Point2D _toPan;
		public Point2D PreviousPoint
		{
			get => _previousPoint;
			set
			{
				_previousPoint = value;
				OnPropertyChanged(nameof(PreviousPoint));
			}
		}
		public Point2D ToPan
		{
			get => _toPan;
			set
			{
				_toPan = value;
				OnPropertyChanged(nameof(ToPan));
			}
		}
		float OriginXWhenPressed;
		float OriginYWhenPressed;
		public FrameElement2D frameGet;
		protected override void OnPointerPressed(PointerPressedEventArgs e)
		{
			IsNodePressed = false;
			OrgCoord = e.GetPosition(this);
			Coord = _grid.DrawableCoord(OrgCoord);
			X = Coord.X;
			Y = Coord.Y;
			searchedPoint = NodeSearch(Coord);
			ButtonName button = PanButton;
			PointerPointProperties properties = e.GetCurrentPoint(this).Properties;
			if (searchedPoint is not null)
			{
				var searchedPointInPixal = (Point2D)_grid.RealDisplayCoord(new Point(searchedPoint.X, searchedPoint.Y));
				var distance = searchedPointInPixal.DistanceTo((Point2D)OrgCoord);
				if (distance < 10)
				{
					IsNodePressed = true;
					searchedPoint.IsNodeSelected = true;
					Unselection(searchedPoint);

				}
			}
			InfoUpdate();
			if ((properties.IsLeftButtonPressed) && AddNodeOn == false && AddFrameOn == false && AddSupport == false && IsNodePressed == false)
			{
				frameGet = GetFrame(this.structure.Elements, new Point2D((float)OrgCoord.X, (float)OrgCoord.Y));
				if (frameGet is not null)
				{
					Info = frameGet.Label + " is pressed";
					frameGet.IsFrameTouched = true;
					NearestDistance = 10;
					Unselection(null, frameGet);
				}
			}
			if (Info == "") Unselection();
			void Unselection(Node2D n = null, FrameElement2D f = null)
			{
				foreach (FrameElement2D frame in this.structure.Elements)
				{
					if (frame != f)
						frame.IsFrameTouched = false;

				}
				foreach (Node2D node in this.structure.Nodes)
				{
					if (node != n)
						node.IsNodeSelected = false;

				}
			}

			if (AddNodeOn == true)
			{
				string nodename = "n" + structure.Nodes.Count;
				structure.AddNode(new Node2D(X, Y, nodename));
				AddNodeOn = false;
			}
			if (AddSupport == true && IsNodePressed is true)
			{
				searchedPoint.Support = new NodalSupport(supportType);
				AddSupport = false;
			}
			if (AddFrameOn == true)
			{
				if (IsNodePressed is true)
				{
					nodeCountForFrame++;
					if (nodeCountForFrame == 1) { firstPoint = searchedPoint; firstPoint.IsNodeSelected = true; }
					if (nodeCountForFrame == 2)
					{
						firstPoint.IsNodeSelected = false;
						secondPoint = NodeSearch(Coord);
						if (secondPoint != firstPoint)
						{
							secondPoint.IsNodeSelected = false;
							string framename = "e" + structure.Elements.Count;
							structure.AddElement(new[] { new FrameElement2D(firstPoint, secondPoint, framename) { CrossSection = section } });
						}
						AddFrameOn = false;
						nodeCountForFrame = 0;
					}
				}
			}
			if ((properties.IsRightButtonPressed && button == ButtonName.Right))
			{
				if (IsPanning == false)
				{

					PreviousPoint = (Point2D)e.GetPosition(this);
					IsPanning = true;
					OriginXWhenPressed = this._grid.OriginX;
					OriginYWhenPressed = this._grid.OriginY;
				}
			}
		}
		public float NearestDistance = 10;
		FrameElement2D GetFrame(List<IElement> framelist, Point2D pressedPoint)
		{
			foreach (FrameElement2D frame in framelist)
			{

				var startPoint = this._grid.RealDisplayCoord(new Avalonia.Point(frame.StartNode.X, frame.StartNode.Y));
				var endPoint = this._grid.RealDisplayCoord(new Avalonia.Point(frame.EndNode.X, frame.EndNode.Y));
				var frameLength = Math.Sqrt(Math.Pow(endPoint.Y - startPoint.Y, 2) + Math.Pow(endPoint.X - startPoint.X, 2));
				var nearestPoint = pressedPoint.NearestOnLine((Point2D)startPoint, (Point2D)endPoint);
				float distanceEnd1 = pressedPoint.DistanceTo((Point2D)startPoint);
				float distanceEnd2 = pressedPoint.DistanceTo((Point2D)endPoint);
				float distanceEnd = Math.Min(distanceEnd1, distanceEnd2);
				float distanceMid = 5 * (float)frameLength;
				int segmentNo = (int)frameLength / 5;
				double slopeAngle = Math.Atan2((-endPoint.Y + startPoint.Y), (endPoint.X - startPoint.X));
				double delL = frameLength / segmentNo;
				Point delPointChange = new Point(delL * Math.Cos(slopeAngle), -delL * Math.Sin(slopeAngle));
				float MinDistance = 5 * (float)frameLength;
				for (int i = 0; i < segmentNo; i++)
				{
					startPoint += delPointChange;
					distanceMid = pressedPoint.DistanceTo((Point2D)startPoint);
					if (MinDistance > distanceMid)
					{
						MinDistance = distanceMid;
					}
				}
				MinDistance = Math.Min(MinDistance, distanceEnd);
				if (MinDistance < 10)
				{
					if (NearestDistance > MinDistance)
					{
						NearestDistance = MinDistance;
					}
					return frame;
				}
			}
			return null;
		}
		protected override void OnPointerReleased(PointerReleasedEventArgs e)
		{
			base.OnPointerReleased(e);
			if (IsPanning == true && CurrentMovingPoint is not null)
			{
				PreviousPoint = new Point2D(CurrentMovingPoint.X, CurrentMovingPoint.Y);
			}
			IsPanning = false;
		}
		public Point2D CurrentMovingPoint;
		protected override void OnPointerMoved(PointerEventArgs e)
		{
			base.OnPointerMoved(e);
			CurrentMovingPoint = (Point2D)e.GetPosition(this);
			if (IsPanning is not true)
			{
				return;
			}
			double x = CurrentMovingPoint.X;
			double y = CurrentMovingPoint.Y;
			var dx = x - PreviousPoint.X;
			var dy = y - PreviousPoint.Y;
			ToPan = (Point2D)new Avalonia.Point(dx, dy);
			this._grid.OriginX = OriginXWhenPressed + ToPan.X;
			this._grid.OriginY = OriginYWhenPressed + ToPan.Y;
			IsPanning = true;
		}
	}
}
