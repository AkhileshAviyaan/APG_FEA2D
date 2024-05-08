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
using System.Drawing;
using System.Reactive.Concurrency;
using System.Runtime.InteropServices;
namespace APG_FEA2D.Views
{
    public partial class CustomSkiaPage
    {
        int spacingIndexForGrid = 5;
        protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
        {
            var point = e.GetPosition(this);
            var delta = (float)e.Delta.Y;
            double ratio = Math.Pow(_zoomSpeed, delta);
            _matrix = MatrixHelper.ScaleAtPrepend(_matrix, ratio, ratio, point.X, point.Y);
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
        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            IsNodePressed = false;
            OrgCoord = e.GetPosition(this);
            Coord = _grid.DrawableCoord(OrgCoord);
            X = Coord.X;
            Y = Coord.Y;
            searchedPoint = NodeSearch(Coord);
            if (searchedPoint is not null)
            {
                IsNodePressed = true;
            }
            InfoUpdate();
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
                        string framename = "e" + structure.Elements.Count;
                        structure.AddElement(new[] { new FrameElement2D(firstPoint, secondPoint, framename) { CrossSection = section } });
                        AddFrameOn = false;
                        nodeCountForFrame = 0;
                    }
                }
            }
            ButtonName button = PanButton;
            PointerPointProperties properties = e.GetCurrentPoint(this).Properties;
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
            else { return; }
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
            if (IsPanning is not true)
            {
                return;
            }
            CurrentMovingPoint = (Point2D)e.GetPosition(this);
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
