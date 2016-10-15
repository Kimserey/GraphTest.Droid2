using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace BoxRendererTest
{
	public static class CustomColors {
		public static string LightBlue = "#27B4E8";
	}

	public class Line
	{
		public float XStart { get; set; }
		public float XStop { get; set; }
		public float YStart { get; set; }
		public float YStop { get; set; }
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

	public class CustomCell : ViewCell
	{
		public CustomCell()
		{
			var layout = new AbsoluteLayout { Padding = new Thickness(20, 5) };
			var x = new Label { HorizontalTextAlignment = TextAlignment.Start, HorizontalOptions = LayoutOptions.Start, VerticalTextAlignment = TextAlignment.Center };
			var y = new Label { HorizontalTextAlignment = TextAlignment.End, HorizontalOptions = LayoutOptions.End, VerticalTextAlignment = TextAlignment.Center };
			layout.Children.Add(x, new Rectangle(0, 0, .5, 1), AbsoluteLayoutFlags.All);
			layout.Children.Add(y, new Rectangle(1, 0, .5, 1), AbsoluteLayoutFlags.All);
			x.SetBinding(Label.TextProperty, "X");
			y.SetBinding(Label.TextProperty, "Y");
			this.View = layout;
		}
	}

	public class GraphView : BoxView
	{
		public GraphView()
		{
			this.BackgroundColor = Color.FromHex(CustomColors.LightBlue);
		}

		public static readonly BindableProperty DataProperty =
			  BindableProperty.Create(
				  propertyName: "Data",
				  returnType: typeof(IList<Data>),
				  declaringType: typeof(GraphView),
				  defaultValue:new List<Data>());

		public IList<Data> Data
		{
			get { return (IList<Data>)GetValue(DataProperty); }
			set { SetValue(DataProperty, value); }
		}
	}

	public class App : Application
	{
		public App()
		{
			var data =
				new List<Data> {
					new Data { X = "Jan", Y = 100.05 },
					new Data { X = "Feb", Y = 250.15 },
					new Data { X = "Mar", Y = 325 },
					new Data { X = "Jun", Y = 311.25 },
					new Data { X = "Jul", Y = 320.15 },
					new Data { X = "Aug", Y = 287 },
					new Data { X = "Sep", Y = 300.05 },
					new Data { X = "Oct", Y = 250.05 },
					new Data { X = "Nov", Y = 320.05 }
			};

			var list = new ListView
			{
				ItemsSource = data,
				ItemTemplate = new DataTemplate(typeof(CustomCell)),
				BackgroundColor = Color.White
			};

			var graph = new GraphView { 
				Data = data
			};

			var layout = new AbsoluteLayout();

			layout.Children.Add(graph, new Rectangle(0, 0, 1, .3), AbsoluteLayoutFlags.All);
			layout.Children.Add(list, new Rectangle(0, 1, 1, .7), AbsoluteLayoutFlags.All);

			var content = new ContentPage
			{
				Title = "Test",
				Content = layout
			};

			MainPage = new NavigationPage(content);
		}
	}
}
