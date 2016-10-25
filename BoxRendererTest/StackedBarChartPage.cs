using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace BoxRendererTest
{
	public class StackedBarDataItem
	{
		public string Name { get; set; }
		public double Value { get; set; }
	}

	public class StackedBarOptions
	{
		public float Margin { get; set; }
		public Color BackgroundColor { get; set; }
		public Color ShadowColor { get; set; }
		public Color BarColor { get; set; }
		public Color MarkerColor { get; set; }
		public int TextSize { get; set; }

		public StackedBarOptions()
		{
			this.Margin = 15;
			this.BackgroundColor = Color.FromHex("#2CBCEB");
			this.ShadowColor = Color.FromHex("#1A7596");
			this.BarColor = Color.White;
			this.MarkerColor = Color.White;
			this.TextSize = 14;
		}
	}

	public class StackedBarChartView : BoxView
	{
		public static readonly BindableProperty DataProperty =
  			BindableProperty.Create(
				propertyName: "Data",
				returnType: typeof(ObservableCollection<StackedBarDataItem>),
				declaringType: typeof(StackedBarChartView),
				defaultValue: new ObservableCollection<StackedBarDataItem>(),
				defaultBindingMode: BindingMode.TwoWay);

		public ObservableCollection<StackedBarDataItem> Data
		{
			get { return (ObservableCollection<StackedBarDataItem>)GetValue(DataProperty); }
			set { SetValue(DataProperty, value); }
		}

		public static readonly BindableProperty OptionsProperty =
			  BindableProperty.Create(
				  propertyName: "Options",
				  returnType: typeof(StackedBarOptions),
				  declaringType: typeof(StackedBarChartView),
				  defaultValue: new StackedBarOptions());

		public StackedBarOptions Options
		{
			get { return (StackedBarOptions)GetValue(OptionsProperty); }
			set { SetValue(DataProperty, value); }
		}
	}

	class StackedBarChartPageViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		ObservableCollection<StackedBarDataItem> items = new ObservableCollection<StackedBarDataItem>();
		IEnumerable<StackedBarDataItem> chartData;

		public StackedBarChartPageViewModel()
		{
			items =
				new ObservableCollection<StackedBarDataItem> {
						new StackedBarDataItem { Name = "Apples", Value = 90 },
						new StackedBarDataItem { Name = "Bananas", Value = 32 },
						new StackedBarDataItem { Name = "Carrots", Value = 100 },
						new StackedBarDataItem { Name = "Oranges", Value = 10 }
				};

			chartData = items;

			this.AddItemCommand = new Command(() =>
				{
					items.Add(new StackedBarDataItem { Name = "New one", Value = 10 });
					this.ChartData = items.ToList();
				});
		}

		public IEnumerable<StackedBarDataItem> ChartData
		{
			get
			{
				return chartData;
			}

			set
			{
				chartData = value;
				PropertyChanged(this, new PropertyChangedEventArgs("ChartData"));
			}
		}

		public ObservableCollection<StackedBarDataItem> Data
		{
			get
			{
				return items;
			}

			set
			{
				items = value;
				PropertyChanged(this, new PropertyChangedEventArgs("Data"));
			}
		}

		public ICommand AddItemCommand { get; set; }

	}

	public class StackedBarChartPage : ContentPage
	{
		public StackedBarChartPage()
		{
			this.Title = "Stacked bar";

			this.BindingContext = new StackedBarChartPageViewModel();

			var list = new ListView { ItemTemplate = new DataTemplate(typeof(TextCell)) };
			var chart = new StackedBarChartView();

			var button = new Button { Text = "Add random item" };

			var layout = new AbsoluteLayout();
			layout.Children.Add(chart, new Rectangle(0, 0, 1, .12), AbsoluteLayoutFlags.All);
			layout.Children.Add(list, new Rectangle(0, 1, 1, .88), AbsoluteLayoutFlags.All);
			layout.Children.Add(button, new Rectangle(0, 1, 1, .10), AbsoluteLayoutFlags.All);
			this.Content = layout;

			list.SetBinding(ListView.ItemsSourceProperty, "Data");
			list.ItemTemplate.SetBinding(TextCell.TextProperty, "Name");
			list.ItemTemplate.SetBinding(TextCell.DetailProperty, "Value");
			chart.SetBinding(StackedBarChartView.DataProperty, "ChartData");

			button.SetBinding(Button.CommandProperty, "AddItemCommand");
		}
	}
}

