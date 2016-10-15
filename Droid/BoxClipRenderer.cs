using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Color = Android.Graphics.Color;

using BoxRendererTest;

[assembly: ExportRenderer(typeof(BoxClip), typeof(BoxClipRenderer))]
namespace BoxRendererTest
{
	public class BoxClipRenderer: BoxRenderer
	{
		protected override void OnDraw(Canvas canvas)
		{
			base.OnDraw(canvas);
		}
	}
}
