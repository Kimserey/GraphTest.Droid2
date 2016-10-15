using System;

namespace BoxRendererTest
{
	public static class CustomColors
	{
		public static string LightBlue = "#27B4E8";
	}

	public class Section
	{ 
		public int Count { get; set; }
		public float Width { get; set; }
		public float Max { get; set; }
	}

	public class Padding
	{
		public float Left { get; set; }
		public float Right { get; set; }
		public float Top { get; set; }
		public float Bottom { get; set; }
	}

	public class Data
	{
		public string X { get; set; }
		public double Y { get; set; }
	}
}
