using System;
using System.Collections.Generic;
using System.Text;

namespace GnssSharp
{
	public static class Operators
	{
		public static double sin(double x) => Math.Sin(x);
		public static double cos(double x) => Math.Cos(x);
		public static double tan(double x) => Math.Tan(x);
		public static double sqrt(double x) => Math.Sqrt(x);
		public static double atan2(double y, double x) => Math.Atan2(y, x);
		public static double pow(double x, double y) => Math.Pow(x, y);
		public static double exp(double x) => Math.Exp(x);
		public static double log(double x) => Math.Log(x);
		public static double log10(double x) => Math.Log10(x);
		public static double abs(double x) => Math.Abs(x);
	}
}