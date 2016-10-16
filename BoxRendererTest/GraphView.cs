using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;


namespace BoxRendererTest
{
	public class GraphData
	{
		public string X { get; set; }
		public double Y { get; set; }
	}

	public class GraphOptions
	{
		public Color BackgroundColor { get; set; }
		public Color BandColor { get; set; }
		public Color LineShadowColor { get; set; }
		public Color LineColor { get; set; }
		public Color MarkerTextShadowColor { get; set; }
		public Color MarkerTextColor { get; set; }
		public float XAxisLabelOffset { get; set; }
		public float YAxisLabelOffset { get; set; }
		public float SectionHeight { get; set; }
		public float AxisStrokeWidth { get; set; }
		public float LineStrokeWidth { get; set; }
		public float MarkerTextOffset { get; set; }
		public float MarkerShadowTextOffset { get; set; }
		public float MarkerTextSize { get; set; }
		public float MarkerDefaultRadius { get; set; }
		public float MarkerSelectedRadius { get; set; }
		public float LabelTextSize { get; set; }
		public Pad Padding { get; set; }

		public static GraphOptions Default()
		{
			return new GraphOptions
			{
				BackgroundColor = Color.FromHex("#2CBCEB"),
				BandColor = Color.FromHex("#36ACD4"),
				LineShadowColor = Color.FromHex("#1A7596"),
				LineColor = Color.FromHex("#EDEDED"),
				MarkerTextShadowColor = Color.FromHex("#0E3D4D"),
				MarkerTextColor = Color.FromHex("#FFFFFF"),
				XAxisLabelOffset = 2f,
				YAxisLabelOffset = 2f,
				SectionHeight = 50f,
				LineStrokeWidth = 3f,
				AxisStrokeWidth = 2f,
				MarkerTextSize = 16f,
				MarkerTextOffset = 10f,
				MarkerShadowTextOffset = 8f,
				MarkerDefaultRadius = 4f,
				MarkerSelectedRadius = 5f,
				LabelTextSize = 12f,
				Padding = new Pad { Bottom = 5, Left = 5, Right = 10, Top = 10 }
			};
		}

		public class Pad
		{
			public float Left { get; set; }
			public float Right { get; set; }
			public float Top { get; set; }
			public float Bottom { get; set; }
		}
	}

	public class GraphView : BoxView
	{
		public static readonly BindableProperty DataProperty =
			  BindableProperty.Create(
				  propertyName: "Data",
				  returnType: typeof(IEnumerable<GraphData>),
				  declaringType: typeof(GraphView),
				  defaultValue: new List<GraphData>());

		public IEnumerable<GraphData> Data
		{
			get { return (IEnumerable<GraphData>)GetValue(DataProperty); }
			set { SetValue(DataProperty, value); }
		}

		public static readonly BindableProperty GraphOptionsProperty =
			  BindableProperty.Create(
				  propertyName: "GraphOptions",
				  returnType: typeof(GraphOptions),
				  declaringType: typeof(GraphView),
				  defaultValue: GraphOptions.Default());

		public GraphOptions GraphOptions
		{
			get { return (GraphOptions)GetValue(GraphOptionsProperty); }
			set { SetValue(DataProperty, value); }
		}
	}

}
