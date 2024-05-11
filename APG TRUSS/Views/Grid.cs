using Avalonia;
using Avalonia.Rendering.SceneGraph;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APG_FEA2D.Views
{
    public class Grid
    {
        public double offsetX;
        public double offsetY;
        public double zoom;
        public float w;
        public float h;
        public float spacing = 100;
        int wNo;
        int hNo;
        float sideExtentW;
        float sideExtentH;
        float sideExtent;
        public float OriginX = 0;
        public float OriginY = 0;
        public float DelX = 0;
        public float DelY = 0;
        public static float SpacingEquivalentInGrid=1;
        public float[] SpacingEquivalentInGridList = new float[]
        {
            50,
            25,
            10,
            5,
            2,
            1,
            0.5f,
            0.25f,
            0.1f,
            0.05f,
            0.025f,
            0.01f
        };
        public void DrawGrid(SKCanvas canvas, Rect Bounds, Matrix matrix)
        {
            w = (float)Bounds.Width;
            h = (float)Bounds.Height;
            canvas.Save();
            offsetX = matrix.M31;
            offsetY = matrix.M32;
            zoom = matrix.M11;

            var paint = new SKPaint
            {
                Color = new SKColor(255, 255, 255, 70),
                StrokeWidth = 1,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke
            };
            var paintText = new SKPaint
            {
                Color = new SKColor(255, 255, 255, 70),
                StrokeWidth = 1,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke
            };

            if (OriginX == 0 && OriginY == 0)
            {
                OriginX = w / 2;
                OriginY = h / 2;
            }

            //vertical line
            for (float i = OriginX; i < w; i += spacing)
            {
                canvas.DrawLine(i, 0, i, h - 4, paint);
                canvas.DrawText($"{TransformCoord(OriginX, i,spacing):F2}", i + 2, h, paintText);
            }

            //horizontal line
            for (float i = OriginY; i < h; i += spacing)
            {
                canvas.DrawLine(0 + 4, i, w, i, paint);
                canvas.DrawText($"{TransformCoord(h-OriginY, h-i, spacing):F2}", 0, i-2, paintText);

            }
            //vertical line
            for (float i = OriginX; i > 0; i -= spacing)
            {
                canvas.DrawLine(i, 0, i, h - 4, paint);
                canvas.DrawText($"{TransformCoord(OriginX, i,spacing):F2}", i + 2, h, paintText);

            }

            //horizontal line
            for (float i = OriginY; i > 0; i -= spacing)
            {
                canvas.DrawLine(0 + 4, i, w, i, paint);
                canvas.DrawText($"{TransformCoord(h-OriginY, h - i, spacing):F2}", 0, i - 2, paintText);

            }
            canvas.Restore();
        }

      
        public static float TransformCoord(float origin, float point, float spacing)
        {
            float fullNumber = ((point-origin) / spacing);
            int intPart = Math.Sign(fullNumber) * (int)Math.Floor(Math.Abs(( point- origin) / spacing));
            float remainingPart = fullNumber - (float)intPart;
            if (Math.Abs(remainingPart) > 0.5)
            {
                if (intPart > 0)
                {
                    intPart++;
                }
                else if(intPart<0)
                {
                    intPart--;
                }
                else
                {
                    if (fullNumber > 0) intPart++;
                    else intPart--;
                }
            };
            return SpacingEquivalentInGrid* intPart;
        }
        public Point DrawableCoord(Point point)
        {
            float X = TransformCoord(OriginX, (float)point.X, spacing);
            float Y = TransformCoord(h-OriginY, h-(float)point.Y, spacing);
            return new Point(X, Y);
        }
        public Point RealDisplayCoord(Point point)
        {
            float X = (float)point.X/ SpacingEquivalentInGrid * spacing + OriginX;
            float Y = OriginY - (float)point.Y * spacing/ SpacingEquivalentInGrid;
            return new Point(X, Y);
        }
        public Point RealDisplayCoordYImprove(Point point)
        {
            float X = (float)point.X / SpacingEquivalentInGrid * spacing + OriginX;
            float Y = h-(OriginY - (float)point.Y * spacing / SpacingEquivalentInGrid);
            return new Point(X, Y);
        }
    }

}