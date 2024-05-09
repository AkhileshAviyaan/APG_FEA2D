using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using FEA2D.Elements;
using Spatial;
namespace APG_FEA2D.Helper;
public partial class Point2D : ObservableObject, IComparable<Point2D>
{
    [ObservableProperty]
    private float x;

    [ObservableProperty]
    private float y;
    public Point2D()
    {

    }
    public Point2D(float x, float y)
    {
        X = x; Y = y;
    }
    public static explicit operator Point2D(Avalonia.Point point)
    {
        return new Helper.Point2D((float)point.X, (float)point.Y);
    }
    public static explicit operator Point2D(Node2D point)
    {
        return new Point2D((float)point.X, (float)point.Y);
    }



    public void Deconstruct(out float x, out float y)
    {
        x = X;
        y = Y;
    }

    public static Point2D FromXY(float x, float y)
    {
        return new Point2D(x, y);
    }

    public static Point2D operator +(Point2D point1, Point2D point2)
    {
        return new Point2D(point1.X + point2.X, point1.Y + point2.Y);
    }

    public static Point2D operator -(Point2D point1, Point2D point2)
    {
        return new Point2D(point1.X - point2.X, point1.Y - point2.Y);
    }

    public float DistanceTo(Point2D other)
    {
        float num = X - other.X;
        float num2 = Y - other.Y;
        return (float)Math.Sqrt(num * num + num2 * num2);
    }

    public float AngleBetween(Point2D other)
    {
        float num = (float) Math.Atan2(other.Y - Y, other.X - X) * 180.0f / (float)Math.PI;
        if (num < 0.0)
        {
            num += 360.0f;
        }

        return num;
    }

    public Point2D RotateAt(Point2D center, float angleInDegrees)
    {
        float num = angleInDegrees * (float)(Math.PI / 180.0);
        float num2 = (float)Math.Cos(num);
        float num3 = (float)Math.Sin(num);
        return new Point2D(num2 * (X - center.X) - num3 * (Y - center.Y) + center.X, num3 * (X - center.X) + num2 * (Y - center.Y) + center.Y);
    }

    public Point2D ProjectOnLine(Point2D a, Point2D b)
    {
        float x = a.X;
        float y = a.Y;
        float x2 = b.X;
        float y2 = b.Y;
        float x3 = X;
        float y3 = Y;
        float num = x2 - x;
        float num2 = y2 - y;
        float num3 = num * num + num2 * num2;
        float num4 = ((x3 - x) * num + (y3 - y) * num2) / num3;
        float x4 = x + num4 * num;
        float y4 = y + num4 * num2;
        return new Point2D(x4, y4);
    }

    public Point2D NearestOnLine(Point2D a, Point2D b)
    {
        float num = X - a.X;
        float num2 = Y - a.Y;
        float num3 = b.X - a.X;
        float num4 = b.Y - a.Y;
        float num5 = (num * num3 + num2 * num4) / (num3 * num3 + num4 * num4);
        if (num5 < 0.0)
        {
            return new Point2D(a.X, a.Y);
        }

        if (num5 > 1.0)
        {
            return new Point2D(b.X, b.Y);
        }

        return new Point2D(num3 * num5 + a.X, num4 * num5 + a.Y);
    }

    public bool IsOnLine(Point2D a, Point2D b)
    {
        float num = Math.Min(a.X, b.X);
        float num2 = Math.Max(a.X, b.X);
        float num3 = Math.Min(a.Y, b.Y);
        float num4 = Math.Max(a.Y, b.Y);
        if (num <= X && X <= num2 && num3 <= Y)
        {
            return Y <= num4;
        }

        return false;
    }

    public Rect2 ExpandToRect(float radius)
    {
        float num = radius * 2.0f;
        return new Rect2(X - radius, Y - radius, num, num);
    }

    public static bool operator <(Point2D p1, Point2D p2)
    {
        if (!(p1.X < p2.X))
        {
            if (p1.X == p2.X)
            {
                return p1.Y < p2.Y;
            }

            return false;
        }

        return true;
    }

    public static bool operator >(Point2D p1, Point2D p2)
    {
        if (!(p1.X > p2.X))
        {
            if (p1.X == p2.X)
            {
                return p1.Y > p2.Y;
            }

            return false;
        }

        return true;
    }

    public int CompareTo(Point2D other)
    {
        if (!(this > other))
        {
            if (!(this < other))
            {
                return 0;
            }

            return 1;
        }

        return -1;
    }

    public bool Equals(Point2D other)
    {
        if (X == other.X)
        {
            return Y == other.Y;
        }

        return false;
    }

    public override bool Equals(object? obj)
    {
        if (!(obj is Point2D))
        {
            return false;
        }

        return Equals((Point2D)obj);
    }

    public override int GetHashCode()
    {
        return X.GetHashCode() + Y.GetHashCode();
    }

    public override string ToString()
    {
        return X + CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator + Y;
    }
}