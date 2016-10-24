using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace BoxRendererTest
{
	// A clipping test
	// Code is in custom renderer
	public class BoxClip : BoxView
	{ }

	public class App : Application
	{
		public App()
		{
			MainPage =
				new TabbedPage
				{
					Children = {
						new GraphPage(),
						new StackedBarChartPage()
					}
				};
		}
	}
}
