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
			MainPage = new NavigationPage(new GraphPage());
		}
	}
}
