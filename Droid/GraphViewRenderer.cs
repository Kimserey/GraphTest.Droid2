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

[assembly: ExportRenderer(typeof(GraphView), typeof(GraphViewRenderer))]
namespace BoxRendererTest
{
	public class GraphViewRenderer : BoxRenderer
	{
		Paint paint = new Paint();

		protected override void OnElementChanged(ElementChangedEventArgs<BoxView> e)
		{
			base.OnElementChanged(e);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == GraphView.DataProperty.PropertyName
			    || e.PropertyName == GraphView.PaddingProperty.PropertyName
			    || e.PropertyName == VisualElement.WidthProperty.PropertyName
			    || e.PropertyName == VisualElement.WidthRequestProperty.PropertyName
				|| e.PropertyName == VisualElement.HeightProperty.PropertyName
			    || e.PropertyName == VisualElement.HeightRequestProperty.PropertyName)
			{
				this.Invalidate();
			}
		}

		protected override void OnDraw(Canvas canvas)
		{
			base.OnDraw(canvas);

			var data = ((GraphView)Element).Data;
			var padding = ((GraphView)Element).Padding;

			DrawPlot(canvas, this.Width, this.Height, padding, Resources.DisplayMetrics.Density, paint, data);
		}

		static void DrawPlot(Canvas canvas, int viewWidth, int viewHeight, Padding padding, float density, Paint paint, IEnumerable<Data> items)
		{
			paint.Reset();
			var horizontalL = new Line
			{
				XStart = padding.Left,
				XStop = viewWidth - padding.Right,
				YStart = viewHeight - padding.Bottom,
				YStop = viewHeight - padding.Bottom
			};


			var verticalL = new Line
			{
				XStart = padding.Left,
				XStop = padding.Left,
				YStart = padding.Top,
				YStop = viewHeight - padding.Bottom
			};

			canvas.DrawLine(horizontalL.XStart, horizontalL.YStart, horizontalL.XStop, horizontalL.YStop, paint);
			canvas.DrawLine(verticalL.XStart, verticalL.YStart, verticalL.XStop, verticalL.YStop, paint);


			var horizontal = new Section
			{
				Width = (horizontalL.XStop - horizontalL.XStart) / items.Count(),
				Count = items.Count()
				                                                        
			};

			var vertical = new Section
			{
				Max = (int)Math.Ceiling(items.Max(i => i.Y) / 100f) * 100f,
				Count = (int)Math.Ceiling(items.Select(i => i.Y).Max() / 100),
				Width = (verticalL.YStop - verticalL.YStart) / ((int)Math.Ceiling(items.Select(i => i.Y).Max() / 100))
			};


			var points = new List<Tuple<float, float, double>>();
			foreach (var l in items.Select((l, index) => Tuple.Create(l.X, l.Y, index)))
			{
				var x = horizontal.Width * (l.Item3 + 1f / 2f) + horizontalL.XStart;
				var y = (float)l.Item2 * (verticalL.YStop - verticalL.YStart) / vertical.Max;

				points.Add(Tuple.Create(x, verticalL.YStop - y, l.Item2));
			}

			// Draws X axis
			paint.Reset();
			paint.TextAlign = Paint.Align.Left;
			paint.TextSize = 9 * density;
			paint.SetTypeface(Typeface.Create(Typeface.Default, TypefaceStyle.Bold));
			paint.Color = Color.ParseColor("#ededed");
			foreach (Tuple<string, int> l in items.Select((Data l, int index) => Tuple.Create(l.X, index)))
			{
				var x = horizontal.Width * (l.Item2 + 1f / 2f) + horizontalL.XStart;
				var halfedTextSize = paint.MeasureText(l.Item1) / 2f;

				canvas.DrawText(
					text: l.Item1,
					x: x - halfedTextSize,
					y: horizontalL.YStart + paint.TextSize,
					paint: paint);
			}

			// Draw Y axis
			paint.Reset();
			paint.TextAlign = Paint.Align.Center;
			paint.TextSize = 9 * density;
			paint.Color = Color.ParseColor("#ededed");
			foreach (var v in Enumerable.Range(0, vertical.Count).Select(i => Tuple.Create(i * 100, i)))
			{
				var y = verticalL.YStop - vertical.Width * v.Item2;

				canvas.DrawText(
					text: v.Item1.ToString(),
					x: verticalL.XStart - 2 * density,
					y: y - (paint.TextSize / 2f),
					paint: paint);

				if (v.Item2 % 2 > 0 && v.Item2 < vertical.Count)
					canvas.DrawRect(
						left: horizontalL.XStart,
						top: y - vertical.Width,
						right: horizontalL.XStop,
						bottom: y,
						paint: paint);
			}

			// Draws line shadow
			paint.Reset();
			paint.Color = Color.ParseColor("#1A7596");
			for (int i = 0; i < points.Count; i++)
			{
				if (i < points.Count - 1)
					canvas.DrawLine(
						   points[i].Item1,
						   points[i].Item2 + 2f * density,
						   points[i + 1].Item1,
						   points[i + 1].Item2 + 2f * density,
						   paint);

				canvas.DrawCircle(
					cx: points[i].Item1,
					cy: points[i].Item2 + 2f * density,
					radius: 5 * density,
					paint: paint);
			}

			// Draw main line
			paint.Reset();
			paint.Color = Color.ParseColor("#ededed");
			for (int i = 0; i < points.Count; i++)
			{
				if (i < points.Count - 1)
					canvas.DrawLine(
						points[i].Item1,
						points[i].Item2,
						points[i + 1].Item1,
						points[i + 1].Item2,
						paint);

				canvas.DrawCircle(
					cx: points[i].Item1,
					cy: points[i].Item2,
					radius: 5 * density,
					paint: paint);
			}

			// Draw market text
			paint.Reset();
			paint.TextSize = 14f * density;
			paint.TextAlign = Paint.Align.Center;
			paint.SetTypeface(Typeface.Create(Typeface.Default, TypefaceStyle.Bold));
			paint.Color = Color.ParseColor("#1A7596");
			for (int i = 0; i < points.Count; i++)
			{
				var text = points[i].Item3.ToString();
				canvas.DrawText(
					text: text,
					x: points[i].Item1 + density,
					y: points[i].Item2,
					paint: paint);
			}

			// Draw marker text
			paint.Reset();
			paint.TextSize = 14f * density;
			paint.TextAlign = Paint.Align.Center;
			paint.SetTypeface(Typeface.Create(Typeface.Default, TypefaceStyle.Bold));
			paint.Color = Color.ParseColor("#FFFFFF");
			for (int i = 0; i < points.Count; i++)
			{
				var text = points[i].Item3.ToString();
				canvas.DrawText(
					text: text,
					x: points[i].Item1,
					y: points[i].Item2 - 2f * density,
					paint: paint);
			}
		}
	}
}
