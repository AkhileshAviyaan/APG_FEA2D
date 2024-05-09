using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;
using Avalonia;
using Avalonia.Controls;
using APG_FEA2D.Views;
using DynamicData;
using System;
using System.Numerics;
using System.Net;
namespace FEA2D.Elements
{
    public partial class Node2D
    {
        public Point pointRealCoord { get; set; }
        public float x;
        public float y;
        public void RPoint(Point point)
        {
            pointRealCoord = point;
            x = (float)pointRealCoord.X;
            y = (float)pointRealCoord.Y;
        }
        public void DrawNode(SKCanvas canvas, APG_FEA2D.Views.Grid grid)
        {
            var paintForArc = new SKPaint
            {
                Color = SKColors.OrangeRed,
                StrokeWidth = 1,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke
            };
            var paint = new SKPaint
            {
                Color = SKColors.OrangeRed,
                StrokeWidth = 1,
                IsAntialias = true,
                Style = SKPaintStyle.Fill
            };
            var paintWhenSelected = new SKPaint
            {
                Color = SKColors.Green,
                StrokeWidth = 1,
                IsAntialias = true,
                Style = SKPaintStyle.Fill
            };

            RPoint(grid.RealDisplayCoord(new Point(this.X, this.Y)));

            if (this.IsNodeSelected is true)
            {
                canvas.DrawCircle(x, y, 4F, paintWhenSelected);
            }
            else
            {
                canvas.DrawCircle(x, y, 3.5F, paint);
            }
            canvas.DrawText(this.Label.Substring(1, 1), x + 10, y + 10, paint);


        }

        public void DrawSupport(SKCanvas canvas, APG_FEA2D.Views.Grid grid)
        {
            var paint = new SKPaint
            {
                Color = SKColors.OrangeRed,
                StrokeWidth = 1,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke
            };
            RPoint(grid.RealDisplayCoord(new Point(this.X, this.Y)));

            if (this.Support.RestraintCount == 1)
            {
                canvas.DrawCircle(x, y + 5F, 5F, paint);
                canvas.DrawLine(x - 10F, y + 10F, x + 10F, y + 10F, paint);
            }
            if (this.Support.RestraintCount == 2)
            {
                canvas.DrawLine(x - 7.5F, y + 7.5F, x + 7.5F, y + 7.5F, paint);
                canvas.DrawLine(x, y, x + 7.5F, y + 7.5F, paint);
                canvas.DrawLine(x - 7.5F, y + 7.5F, x, y, paint);
            }
            if (this.Support.RestraintCount == 3)
            {
                canvas.DrawLine(x - 10F, y + 6F, x + 10F, y + 6F, paint);
                canvas.DrawLine(x - 10F, y + 6F, x - 13F, y + 12F, paint);
                canvas.DrawLine(x, y + 6F, x - 3F, y + 12F, paint);
                canvas.DrawLine(x + 10F, y + 6F, x + 7F, y + 12F, paint);
            }
        }
        public void DrawNodalLoad(SKCanvas canvas, APG_FEA2D.Views.Grid grid)
        {
            foreach (var load in this.NodalLoads)
            {
                var paint = new SKPaint
                {
                    Color = SKColors.Yellow,
                    StrokeWidth = 1,
                    IsAntialias = true,
                    Style = SKPaintStyle.Stroke
                };
                float arrowLength = 50;
                float arrowWidth = 10;
                RPoint(grid.RealDisplayCoord(new Point(this.X, this.Y)));

                // Calculate arrow points
                var startPoint = new SKPoint(x, y);
                var endPoint = new SKPoint(startPoint.X + arrowLength * load.CosAngle, startPoint.Y - arrowLength * load.SinAngle);
                var arrowOneSide = new SKPoint(startPoint.X + arrowWidth * load.CosPlusAngle, startPoint.Y - arrowWidth * load.SinPlusAngle);
                var arrowAnotherSide = new SKPoint(startPoint.X + arrowWidth * load.CosMinusAngle, startPoint.Y - arrowWidth * load.SinMinusAngle);

                // Define arrow path
                var path1 = new SKPath();
                path1.MoveTo(arrowOneSide);
                path1.LineTo(startPoint);
                path1.LineTo(arrowAnotherSide);

                var path2 = new SKPath();
                path2.MoveTo(startPoint);
                path2.LineTo(endPoint);


                // Draw arrow
                canvas.DrawPath(path1, paint);
                canvas.DrawPath(path2, paint);

                float textSize = 10;
                // Draw text at the tail end of arrow
                var text = $"{load.F}";
                using (var textPaint = new SKPaint())
                {
                    textPaint.TextSize = textSize;
                    textPaint.IsAntialias = true;
                    textPaint.Color = SKColors.Yellow;
                    var textWidth = textPaint.MeasureText(text);
                    var textX = 0f;
                    var textY = 0f;

                    if (load.SinAngle >= 0 && load.CosAngle > 0)
                    {
                        textX = endPoint.X + textWidth * 0.5f;
                        textY = endPoint.Y - textWidth * 0.5f;
                    }
                    else if (load.SinAngle > 0 && load.CosAngle < 0)
                    {
                        textX = endPoint.X - textWidth * 0.5f;
                        textY = endPoint.Y - textWidth * 0.5f;
                    }
                    else if (load.SinAngle < 0 && load.CosAngle < 0)
                    {
                        textX = endPoint.X - textWidth * 0.5f;
                        textY = endPoint.Y + textWidth * 0.5f;
                    }
                    else if (load.SinAngle < 0 && load.CosAngle > 0)
                    {
                        textX = endPoint.X + textWidth * 0.5f;
                        textY = endPoint.Y + textWidth * 0.5f;
                    }

                    canvas.DrawText(text, textX, textY, textPaint);
                }

            }
        }
        public void DrawReaction(SKCanvas canvas, APG_FEA2D.Views.Grid grid)
        {
            float arrowLength = 50;
            float arrowWidth = 10;
            var paint = new SKPaint
            {
                Color = SKColors.Green,
                StrokeWidth = 1,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke
            };

            RPoint(grid.RealDisplayCoord(new Point(this.X, this.Y)));
            double rotationAngle = 0;

            if (this.force.Fx != 0 && Math.Abs(this.force.Fx) > 1e-3)
            {
                if (this.force.Fx > 0)
                {
                    rotationAngle = Math.PI;
                }
                else
                {
                    rotationAngle = 0;
                }
                this.DrawForce(this.force.Fy, rotationAngle, canvas, grid);
            }
            if (this.force.Fy != 0 && Math.Abs(this.force.Fy) > 1e-3)
            {
                if (this.force.Fy > 0)
                {
                    rotationAngle = 3 * Math.PI / 2;
                }
                else
                {
                    rotationAngle = Math.PI / 2;
                }
                this.DrawForce(this.force.Fy, rotationAngle, canvas, grid);
            }

            this.DrawMoment(canvas, grid);

        }
        public void DrawForce(double force, double rotationAngle, SKCanvas canvas, APG_FEA2D.Views.Grid grid)
        {

            var paint = new SKPaint
            {
                Color = SKColors.Green,
                StrokeWidth = 1,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke
            };
            float arrowLength = 50;
            float arrowWidth = 10;
            float CosAngle = 0;
            float SinAngle = 0;
            float CosPlusAngle = 0;
            float SinPlusAngle = 0;
            float CosMinusAngle = 0;
            float SinMinusAngle = 0;
            CosAngle = (float)Math.Cos(rotationAngle);
            SinAngle = (float)Math.Sin(rotationAngle);
            CosPlusAngle = (float)Math.Cos(rotationAngle + Math.PI / 4);
            SinPlusAngle = (float)Math.Sin(rotationAngle + Math.PI / 4);
            CosMinusAngle = (float)Math.Cos(rotationAngle - Math.PI / 4);
            SinMinusAngle = (float)Math.Sin(rotationAngle - Math.PI / 4);
            // Calculate arrow points
            var startPoint = new SKPoint(x, y);
            var endPoint = new SKPoint(startPoint.X + arrowLength * CosAngle, startPoint.Y - arrowLength * SinAngle);
            var arrowOneSide = new SKPoint(startPoint.X + arrowWidth * CosPlusAngle, startPoint.Y - arrowWidth * SinPlusAngle);
            var arrowAnotherSide = new SKPoint(startPoint.X + arrowWidth * CosMinusAngle, startPoint.Y - arrowWidth * SinMinusAngle);

            // Define arrow path
            var path1 = new SKPath();
            path1.MoveTo(arrowOneSide);
            path1.LineTo(startPoint);
            path1.LineTo(arrowAnotherSide);

            var path2 = new SKPath();
            path2.MoveTo(startPoint);
            path2.LineTo(endPoint);


            // Draw arrow
            canvas.DrawPath(path1, paint);
            canvas.DrawPath(path2, paint);

            float textSize = 10;
            // Draw text at the tail end of arrow
            var text = $"{Math.Abs(force):f3}";
            using (var textPaint = new SKPaint())
            {
                textPaint.TextSize = textSize;
                textPaint.IsAntialias = true;
                textPaint.Color = SKColors.Yellow;
                var textWidth = textPaint.MeasureText(text);
                var textX = 0f;
                var textY = 0f;

                if (SinAngle >= 0 && CosAngle > 0)
                {
                    textX = endPoint.X + textWidth * 0.5f;
                    textY = endPoint.Y - textWidth * 0.5f;
                }
                else if (SinAngle > 0 && CosAngle < 0)
                {
                    textX = endPoint.X - textWidth * 0.5f;
                    textY = endPoint.Y - textWidth * 0.5f;
                }
                else if (SinAngle < 0 && CosAngle < 0)
                {
                    textX = endPoint.X - textWidth * 0.5f;
                    textY = endPoint.Y + textWidth * 0.5f;
                }
                else if (SinAngle < 0 && CosAngle > 0)
                {
                    textX = endPoint.X + textWidth * 0.5f;
                    textY = endPoint.Y + textWidth * 0.5f;
                }

                canvas.DrawText(text, textX, textY, textPaint);
            }
        }
        public void DrawMoment(SKCanvas canvas, APG_FEA2D.Views.Grid grid)
        {
            var paint = new SKPaint
            {
                Color = SKColors.Green,
                StrokeWidth = 1,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke
            };
            float radius = 25;
            float startAngle = 0;
            float sweepAngle = 0;
            if (this.force.Mz == 0) return;
            if (this.Support.Rz is false) return;
            if (this.force.Mz > 0)
            {

                startAngle = 220;
                sweepAngle = -200;
            }
            else
            {
                sweepAngle = 200;
                startAngle = -30;

            }

            var arcPath = new SKPath();
            var rect = new SKRect(x - radius, y - radius, x + radius, y + radius);
            arcPath.AddArc(rect, startAngle, sweepAngle);

            var totalAngle = 360 - (startAngle + sweepAngle);
            float newtotalAngle = 0;
            if (totalAngle >= 270)
            {
                newtotalAngle = totalAngle - 180 + 90;
            }
            else
            {
                newtotalAngle = totalAngle + 90;
            }

            float headCoordX = x + radius * (float)Math.Cos(totalAngle * Math.PI / 180);
            float headCoordY = y - radius * (float)Math.Sin(totalAngle * Math.PI / 180);

            float headOneEndX = headCoordX + 10 * (float)Math.Cos((newtotalAngle + 45) * Math.PI / 180);
            float headOneEndY = headCoordY - 10 * (float)Math.Sin((newtotalAngle + 45) * Math.PI / 180);

            float headAnotherEndX = headCoordX + 10 * (float)Math.Cos((newtotalAngle - 45) * Math.PI / 180);
            float headAnotherEndY = headCoordY - 10 * (float)Math.Sin((newtotalAngle - 45) * Math.PI / 180);
            var arrowPath = new SKPath();
            arrowPath.MoveTo(headOneEndX, headOneEndY);
            arrowPath.LineTo(headCoordX, headCoordY);
            arrowPath.LineTo(headAnotherEndX, headAnotherEndY);
            canvas.DrawPath(arrowPath, paint);
            canvas.DrawPath(arcPath, paint);
            float textSize = 10;
            // Draw text at the tail end of arrow
            var text = $"{Math.Abs(this.force.Mz):f3}";
            using (var textPaint = new SKPaint())
            {
                textPaint.TextSize = textSize;
                textPaint.IsAntialias = true;
                textPaint.Color = SKColors.Yellow;
                var textWidth = textPaint.MeasureText(text);



                canvas.DrawText(text, x, y + radius + 3f, textPaint);
            }
        }
    }
    public partial class FrameElement2D
    {
        SKPaint paint = new SKPaint
        {
            Color = SKColors.OrangeRed,
            StrokeWidth = 1,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke
    };

    public void Draw(SKCanvas canvas, APG_FEA2D.Views.Grid grid)
    {
        if (this.IsFrameTouched)
        {
                this.paint.Color = SKColors.Green;
        }
        else
        {
                this.paint.Color = SKColors.OrangeRed;
        }
        Point firstPointRealCoord = grid.RealDisplayCoord(new Point(this.StartNode.X, this.StartNode.Y));
        Point secondPointRealCoord = grid.RealDisplayCoord(new Point(this.EndNode.X, this.EndNode.Y));
        canvas.DrawLine((float)firstPointRealCoord.X, (float)firstPointRealCoord.Y, (float)secondPointRealCoord.X, (float)secondPointRealCoord.Y, paint);
    }
}
public partial class NodalSupport
{

}
}