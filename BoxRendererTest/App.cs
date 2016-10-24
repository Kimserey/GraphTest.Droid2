using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace BoxRendererTest
{
	public class App : Application
	{
		public App()
		{
			MainPage =
				new TabbedPage
				{
					Children = {
						new GraphPage(),
						new BarChartPage(),
						new ContentPage {
							Title = "Elevation",
							Content = new Frame {
								Content = new Label { Text = "Hello" },
						        HorizontalOptions = LayoutOptions.Center,
								VerticalOptions = LayoutOptions.CenterAndExpand
							}
						}
					}
				};
		}
	}
}
