using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace BoxRendererTest
{
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

	public class Drawing : BoxView
	{
		public static readonly BindableProperty DataProperty =
			  BindableProperty.Create(
				  propertyName: "Data",
				  returnType: typeof(IList<Data>),
				  declaringType: typeof(Drawing),
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
			var list = new ListView
			{
				ItemsSource = new List<Data> {
					new Data { X = "Hellow", Y = 100.05 },
					new Data { X = "Hellow", Y = 100.05 },
					new Data { X = "Hellow", Y = 100.05 },
					new Data { X = "Hellow", Y = 100.05 },
					new Data { X = "Hellow", Y = 100.05 },
					new Data { X = "Hellow", Y = 100.05 },
					new Data { X = "Hellow", Y = 100.05 },
					new Data { X = "Hellow", Y = 100.05 },
					new Data { X = "Hellow", Y = 100.05 },
					new Data { X = "Hellow", Y = 100.05 },
					new Data { X = "Hellow", Y = 100.05 },
					new Data { X = "Hellow", Y = 100.05 },
					new Data { X = "Hellow", Y = 100.05 },
					new Data { X = "Hellow", Y = 100.05 },
					new Data { X = "Hellow", Y = 100.05 }
				},
				ItemTemplate = new DataTemplate(typeof(CustomCell))
			};

			var layout = new AbsoluteLayout();

			layout.Children.Add(new Drawing(), new Rectangle(0, 0, 1, .3), AbsoluteLayoutFlags.All);
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
