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

	public class PlotColors
	{ 
		public string Text { get; set; }
		public string Line { get; set; }
		public string Market { get; set; }
		public string TextMarket { get; set; }
		public string Shadow { get; set; }
		public string Band { get; set; }
	}

	public class PlotOptions
	{ 
		public Data Data { get; set; }
		public Padding Padding { get; set;}
		public PlotColors PlotColors { get; set; }
	}
}
