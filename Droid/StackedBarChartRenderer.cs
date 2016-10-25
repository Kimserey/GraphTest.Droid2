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
		EventHandler<TouchEventArgs> handler;
		int touchXCoordinate = 0;
		float[] tempHSV = new float[3];

		~StackedBarChartRenderer()
		{
			this.Touch -= handler;
		}

		// Make color darker by altering the third HSV value
		void Darken(ref Color color)
		{
			Color.ColorToHSV(color, tempHSV);
			tempHSV[2] *= .5f;
			color = Color.HSVToColor(tempHSV);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<BoxView> e)
		{
			base.OnElementChanged(e);

			if (handler == null)
			{
				handler = (sender, touchEvent) =>
				{
					var newXCoordinate = (int)touchEvent.Event.GetX();
					if (touchXCoordinate != newXCoordinate)
					{
						touchXCoordinate = newXCoordinate;
						this.Invalidate();
					}
				};

				this.Touch += handler;
			}
		}

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

			var density = Resources.DisplayMetrics.Density;
			var margin = 15 * density;

			var backgroundColor = Color.ParseColor("#2CBCEB");
			var shadowColor = Color.ParseColor("#1A7596");
			var barColor = Color.White;
			var markerColor = Color.White;

			// Set paint text properties
			paint.TextAlign = Paint.Align.Center;
			paint.TextSize = 14 * density;


			var element = (StackedBarChartView)this.Element;

			var data = element.Data;

			var k = (this.Width - 2 * margin) / data.Sum(i => i.Value);
			var values = data.Select((i, index) => Tuple.Create(i, (float)(i.Value * k), index)).ToList();


			var position = margin;

			// Shadow
			var offset = 3 * density;


			if (touchXCoordinate >= margin && touchXCoordinate <= this.Width - margin)
			{
				Darken(ref backgroundColor);
				Darken(ref shadowColor);
				Darken(ref barColor);

				paint.Color = backgroundColor;
				canvas.DrawRect(new Rect(0, 0, this.Width, this.Height), paint);

				foreach (var v in values)
				{
					if (touchXCoordinate > position && touchXCoordinate <= position + v.Item2)
					{ 
						// Draw shadow
						paint.Color = shadowColor;
						canvas.DrawRect(new RectF(position + offset, margin + offset, position + offset + v.Item2, this.Height - margin - paint.TextSize + offset), paint);

						//Draw bar
						paint.Color = markerColor;
						canvas.DrawRect(new RectF(v.Item3 == 0 ? position : position + offset, margin, position + v.Item2, this.Height - margin - paint.TextSize), paint);

						//Draw label
						paint.Color = markerColor;
						canvas.DrawText(String.Format("{0} - {1}", v.Item1.Name, v.Item1.Value.ToString("C2")), this.Width / 2,  this.Height - (margin / 2), paint);
					}
					else
					{
						// Draw shadow
						paint.Color = shadowColor;
						canvas.DrawRect(new RectF(position + offset, margin + offset, position + offset + v.Item2, this.Height - margin - paint.TextSize + offset), paint);

						//Draw bar
						paint.Color = barColor;
						canvas.DrawRect(new RectF(v.Item3 == 0 ? position : position + offset, margin, position + v.Item2, this.Height - paint.TextSize - margin), paint);
					}

					position += v.Item2;
				}
			}
			else
			{
				paint.Color = backgroundColor;
				canvas.DrawRect(new Rect(0, 0, this.Width, this.Height), paint);

				foreach (var v in values)
				{
					// Draw shadow
					paint.Color = shadowColor;
					canvas.DrawRect(new RectF(position + offset, margin + offset, position + offset + v.Item2, this.Height - margin - paint.TextSize + offset), paint);

					//Draw bar
					paint.Color = barColor;
					canvas.DrawRect(new RectF(v.Item3 == 0 ? position : position + offset, margin, position + v.Item2, this.Height - paint.TextSize - margin), paint);

					position += v.Item2;
				}
			}
		}
	}
}
