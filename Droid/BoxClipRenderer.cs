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
	public class BoxClipRenderer : BoxRenderer
	{
		readonly Paint paint = new Paint();

		protected override void OnDraw(Canvas canvas)
		{
			base.OnDraw(canvas);

			var density = Resources.DisplayMetrics.Density;

			// Draws a Green rectangle 100x100 at 0,0
			paint.Color = Color.Green;
			canvas.DrawRect(new RectF(0, 0, 100 * density, 100 * density), paint);

			// Save canvas before executing clipping
			// Clips canvas to only draw in 50x100 at 50,50
			canvas.Save();
			canvas.ClipRect(new RectF(50 * density, 50 * density, 100 * density, 150 * density));

			// Draws a Red rectangle 100x100 at 50,50
			// Only 50x100 is drawn because the canvas is previously clipped at 50x100
			paint.Color = Color.Red;
			canvas.DrawRect(new RectF(50 * density, 50 * density, 150 * density, 150 * density), paint);

			// Restore canvas to before executing clipping
			// Translate canvas to 150x100
			// Draws a Blue rectangle 200x200 a 0,0 (translated)
			canvas.Restore();
			canvas.Translate(150 * density, 100 * density);
			paint.Color = Color.Blue;
			canvas.DrawRect(new RectF(0, 0, 200 * density, 200 * density), paint);
		}
	}
}
