using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace BoxRendererTest
{
	public class StackedBarDataItem
	{
		public string Name { get; set; }
		public double Value { get; set; }
	}

	public class StackedBarChartView : BoxView
	{
		public static readonly BindableProperty DataProperty =
  			BindableProperty.Create(
			  propertyName: "Data",
			  returnType: typeof(IList<StackedBarDataItem>),
			  declaringType: typeof(StackedBarChartView),
			  defaultValue: new List<StackedBarDataItem>());

		public IList<StackedBarDataItem> Data
		{
			get { return (IList<StackedBarDataItem>)GetValue(DataProperty); }
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
						new StackedBarDataItem { Name = "Apples", Value = 90 },
						new StackedBarDataItem { Name = "Bananas", Value = 32 },
						new StackedBarDataItem { Name = "Carrots", Value = 100 },
						new StackedBarDataItem { Name = "Oranges", Value = 10 }
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
					Data = data
				};

			var layout = new AbsoluteLayout();

			layout.Children.Add(chart, new Rectangle(0, 0, 1, .1), AbsoluteLayoutFlags.All);
			layout.Children.Add(list, new Rectangle(0, 1, 1, .9), AbsoluteLayoutFlags.All);

			this.Content = layout;
		}
	}
}
