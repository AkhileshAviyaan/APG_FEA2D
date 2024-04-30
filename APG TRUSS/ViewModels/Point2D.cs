using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
namespace APG_FEA2D.Helper;
public partial class Point2D:ObservableObject
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
		X = x; Y=y;
	}
	public static explicit operator Point2D(Avalonia.Point point)
	{
		return new Point2D((float)point.X, (float)point.Y);
	}
}
