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

[assembly: ExportRenderer(typeof(BarChart), typeof(BarChartRenderer))]
namespace BoxRendererTest
{
	public class BarChartRenderer: BoxRenderer
	{
		public class Data { 
			public string Name { get; set; }
			public double Value { get; set; }
		}

		IEnumerable<Data> data = new List<Data> {
			new Data { Name = "A", Value = 90 },
			new Data { Name = "B", Value = 32 },
			new Data { Name = "C", Value = 100 }
		};

		Random randomGen = new Random();
		KnownColor[] names = (KnownColor[])Enum.GetValues(typeof(KnownColor));

		Color RandomColor()
		{
			KnownColor randomColorName = names[randomGen.Next(names.Length)];
			var color = System.Drawing.Color.FromKnownColor(randomColorName);
			return Color.ParseColor(String.Format("#{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B));
		}

		protected override void OnDraw(Canvas canvas)
		{
			base.OnDraw(canvas);

			var k = this.Width / data.Sum(i => i.Value);
			var values = data.Select(i => Tuple.Create(i, (float)(i.Value * k)));

			var position = 0f;
			foreach (var v in values)
			{
				canvas.DrawRect(new RectF(position, 0, position + v.Item2, this.Height), new Paint { Color = RandomColor() });
				position += v.Item2;
			}

		}
	}
}
