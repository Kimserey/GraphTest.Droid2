using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace BoxRendererTest
{
	public class StackedBarDataItem
	{
		public string Name { get; set; }
		public Color Color { get; set; }
		public double Value { get; set; }
	}

	public class StackedBarChartView : BoxView
	{
		public static readonly BindableProperty DataProperty =
  			BindableProperty.Create(
			  propertyName: "Data",
			  returnType: typeof(IEnumerable<StackedBarDataItem>),
			  declaringType: typeof(StackedBarChartView),
			  defaultValue: new List<StackedBarDataItem>());

		public IEnumerable<StackedBarDataItem> Data
		{
			get { return (IEnumerable<StackedBarDataItem>)GetValue(DataProperty); }
			set { SetValue(DataProperty, value); }
		}
	}

	public class StackedBarChartPage : ContentPage
	{
		public StackedBarChartPage()
		{
			this.Title = "Stacked bar";

			var data =
				new List<StackedBarDataItem> {
						new StackedBarDataItem { Name = "A", Value = 90, Color = Color.Red },
						new StackedBarDataItem { Name = "B", Value = 32, Color = Color.Blue },
						new StackedBarDataItem { Name = "C", Value = 100, Color = Color.Yellow }
				};

			var list =
				new ListView
				{
					ItemsSource = data,
					ItemTemplate = new DataTemplate(typeof(TextCell))
				};

			list.ItemTemplate.SetBinding(TextCell.TextProperty, "Name");
			list.ItemTemplate.SetBinding(TextCell.DetailProperty, "Value");

			var chart =
				new StackedBarChartView {
					Data = data,
					BackgroundColor = Color.FromHex("#2CBCEB")
				};

			var layout = new AbsoluteLayout();

			layout.Children.Add(chart, new Rectangle(0, 0, 1, .1), AbsoluteLayoutFlags.All);
			layout.Children.Add(list, new Rectangle(0, 1, 1, .9), AbsoluteLayoutFlags.All);

			this.Content = layout;
		}
	}
}
