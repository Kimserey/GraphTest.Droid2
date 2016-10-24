using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace BoxRendererTest
{
	public class BarDataItem
	{
		public string Name { get; set; }
		public string Color { get; set; }
		public double Value { get; set; }
	}

	public class BarChartPage : ContentPage
	{
		public BarChartPage()
		{
			this.Title = "Bar chart";

			var list =
				new ListView
				{
					ItemsSource = new List<BarDataItem> {
						new BarDataItem { Name = "A", Value = 90, Color = "#" },
						new BarDataItem { Name = "B", Value = 32, Color = "#" },
						new BarDataItem { Name = "C", Value = 100, Color = "#" }
					},
					ItemTemplate = new DataTemplate(typeof(TextCell))
				};

			list.ItemTemplate.SetBinding(TextCell.TextProperty, "Name");
			list.ItemTemplate.SetBinding(TextCell.DetailProperty, "Value");

			var layout = new AbsoluteLayout();

			layout.Children.Add(new BarChart(), new Rectangle(0, 0, 1, .1), AbsoluteLayoutFlags.All);
			layout.Children.Add(list, new Rectangle(0, 1, 1, .9), AbsoluteLayoutFlags.All);

			this.Content = layout;
		}
	}

	public class BarChart: BoxView
	{
		public BarChart()
		{
			this.Margin = 15;
			this.HeightRequest = 350;
		}
	}
}
