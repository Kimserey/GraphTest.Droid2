using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using Android.Graphics;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Color = Android.Graphics.Color;

using BoxRendererTest;
using System.Drawing;

[assembly: ExportRenderer(typeof(StackedBarChartView), typeof(StackedBarChartRenderer))]
namespace BoxRendererTest
{
	public class StackedBarChartRenderer: BoxRenderer
	{
		Paint paint = new Paint();

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == StackedBarChartView.DataProperty.PropertyName)
			{
				this.Invalidate();
			}
		}

		protected override void OnDraw(Canvas canvas)
		{
			base.OnDraw(canvas);

			var data = ((StackedBarChartView)this.Element).Data;

			var k = this.Width / data.Sum(i => i.Value);
			var values = data.Select(i => Tuple.Create(i, (float)(i.Value * k)));

			var position = 0f;
			foreach (var v in values)
			{
				paint.Color = v.Item1.Color.ToAndroid();
				canvas.DrawRect(new RectF(position, 0, position + v.Item2, this.Height), paint);
				position += v.Item2;
			}

		}
	}
}
