using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace BoxRendererTest
{
	public class GraphPage : ContentPage
	{
		private class ExpenseCell : ViewCell
		{
			public ExpenseCell()
			{
				var layout = new AbsoluteLayout { Padding = new Thickness(20, 5) };
				var x = new Label { HorizontalTextAlignment = TextAlignment.Start, HorizontalOptions = LayoutOptions.Start, VerticalTextAlignment = TextAlignment.Center };
				var y = new Label { HorizontalTextAlignment = TextAlignment.End, HorizontalOptions = LayoutOptions.End, VerticalTextAlignment = TextAlignment.Center };
				layout.Children.Add(x, new Rectangle(0, 0, .5, 1), AbsoluteLayoutFlags.All);
				layout.Children.Add(y, new Rectangle(1, 0, .5, 1), AbsoluteLayoutFlags.All);
				x.SetBinding(Label.TextProperty, "X");
				y.SetBinding(Label.TextProperty, "Y");
				View = layout;
			}
		}

		public GraphPage()
		{
			var data =
				new List<GraphData> {
					new GraphData { X = "Jan", Y = 100.05 },
					new GraphData { X = "Feb", Y = 250.15 },
					new GraphData { X = "Mar", Y = 325 },
					new GraphData { X = "Jun", Y = 311.25 },
					new GraphData { X = "Jul", Y = 320.15 },
					new GraphData { X = "Aug", Y = 287 },
					new GraphData { X = "Sep", Y = 300.05 },
					new GraphData { X = "Oct", Y = 250.05 },
					new GraphData { X = "Nov", Y = 320.05 },
					new GraphData { X = "Dec", Y = 250.05 }
			};

			var list = new ListView
			{
				ItemsSource = data,
				ItemTemplate = new DataTemplate(typeof(ExpenseCell)),
				BackgroundColor = Color.White
			};

			var graph = new GraphView
			{
				Data = data,
				BackgroundColor = Color.FromHex(CustomColors.LightBlue)
			};

			var layout = new AbsoluteLayout();

			layout.Children.Add(graph, new Rectangle(0, 0, 1, .4), AbsoluteLayoutFlags.All);
			layout.Children.Add(list, new Rectangle(0, 1, 1, .6), AbsoluteLayoutFlags.All);

			Title = "My expenses";
			Content = layout;
		}
	}
}
