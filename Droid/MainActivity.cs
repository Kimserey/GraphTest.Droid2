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
using BoxRendererTest.Droid;

[assembly: ExportRenderer(typeof(GraphView), typeof(GraphViewRenderer))]
namespace BoxRendererTest.Droid
{
	public class GraphViewRenderer : BoxRenderer
	{

		Paint textPaint;
		Paint axesPaint;
		Paint bandsPaint;
		Paint linePaint;
		Paint markersPaint;
		Padding padding;

		protected override void OnElementChanged(ElementChangedEventArgs<BoxView> e)
		{
			base.OnElementChanged(e);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == GraphView.DataProperty.PropertyName)
			{
				this.Invalidate();
			}
		}

		protected override void OnDraw(Canvas canvas)
		{
			base.OnDraw(canvas);
			Initialise();

			var data = ((GraphView)Element).Data;
			
			GraphViewRenderer.DrawChart(padding,
						   canvas,
						   linePaint,
						   markersPaint,
						   axesPaint,
						   bandsPaint,
						   textPaint,
						   Resources.DisplayMetrics.Density,
						   this.Width,
						   this.Height,
						   data);
		}

		static void DrawChart(
			Padding padding,
			Canvas canvas,
			Paint linePaint,
			Paint marketsPaint,
			Paint axesPaint,
			Paint bandsPaint,
			Paint textPaint,
			float density,
			int viewWidth,
			int viewHeight,
			IEnumerable<Data> items)
		{
			var horizontal = new Line
			{
				XStart = padding.Left,
				XStop = viewWidth - padding.Right,
				YStart = viewHeight - padding.Bottom,
				YStop = viewHeight - padding.Bottom
			};


			var vertical = new Line
			{
				XStart = padding.Left,
				XStop = padding.Left,
				YStart = padding.Top,
				YStop = viewHeight - padding.Bottom
			};

			DrawXLabels(canvas, textPaint, horizontal, items.Select(i => i.X));
			DrawYLabels(canvas, density, bandsPaint, textPaint, horizontal, vertical, items.Select(i => i.Y));
			DrawPlot(canvas, density, linePaint, marketsPaint, textPaint, horizontal, vertical, items);

			canvas.DrawLine(horizontal.XStart, horizontal.YStart, horizontal.XStop, horizontal.YStop, axesPaint);
			canvas.DrawLine(vertical.XStart, vertical.YStart, vertical.XStop, vertical.YStop, axesPaint);

		}

		static void DrawXLabels(Canvas canvas, Paint textPaint, Line horizontal, IEnumerable<string> labels)
		{
			textPaint.TextAlign = Paint.Align.Left;

			var sectionWidth = (horizontal.XStop - horizontal.XStart) / labels.Count();

			foreach (Tuple<string, int> l in labels.Select((string l, int index) => Tuple.Create(l, index)))
			{
				var x = sectionWidth * (l.Item2 + 1f / 2f) + horizontal.XStart;
				var halfedTextSize = textPaint.MeasureText(l.Item1) / 2f;

				canvas.DrawText(
					text: l.Item1,
					x: x - halfedTextSize,
					y: horizontal.YStart + textPaint.TextSize,
					paint: textPaint);
			}
		}

		static void DrawYLabels(Canvas canvas, float density, Paint bandsPaint, Paint textPaint, Line horizontal, Line vertical, IEnumerable<double> values)
		{
			textPaint.TextAlign = Paint.Align.Right;

			var numberOfSections = (int)Math.Ceiling(values.Max() / 100);
			var sectionWidth = (vertical.YStop - vertical.YStart) / numberOfSections;

			foreach (var v in Enumerable.Range(0, numberOfSections).Select(i => Tuple.Create(i * 100, i)))
			{
				var y = vertical.YStop - sectionWidth * v.Item2;

				canvas.DrawText(
					text: v.Item1.ToString(),
					x: vertical.XStart - 2 * density,
					y: y - (textPaint.Ascent() / 2f),
					paint: textPaint);

				if (v.Item2 % 2 > 0 && v.Item2 < numberOfSections)
					canvas.DrawRect(
						left: horizontal.XStart,
						top: y - sectionWidth,
						right: horizontal.XStop,
						bottom: y,
						paint: bandsPaint);
			}
		}

		static void DrawPlot(Canvas canvas, float density, Paint linePaint, Paint markersPaint, Paint valuePaint, Line horizontal, Line vertical, IEnumerable<Data> items)
		{
			var sectionWidth = (horizontal.XStop - horizontal.XStart) / items.Count();
			var ceiling = (int)Math.Ceiling(items.Max(i => i.Y) / 100f) * 100f;
			var points = new List<Tuple<float, float, double>>();

			foreach (var l in items.Select((l, index) => Tuple.Create(l.X, l.Y, index)))
			{
				var x = sectionWidth * (l.Item3 + 1f / 2f) + horizontal.XStart;
				var y = (float)l.Item2 * (vertical.YStop - vertical.YStart) / ceiling;

				points.Add(Tuple.Create(x, vertical.YStop - y, l.Item2));
			}

			// shadow line drawing
			linePaint.Color = Color.ParseColor("#1A7596");
			for (int i = 0; i < points.Count; i++)
			{
				if (i < points.Count - 1)
					canvas.DrawLine(
						   points[i].Item1,
						   points[i].Item2 + 2f * density,
						   points[i + 1].Item1,
						   points[i + 1].Item2 + 2f * density,
						   linePaint);

				canvas.DrawCircle(
					cx: points[i].Item1,
					cy: points[i].Item2 + 2f * density,
					radius: 5 * density,
					paint: linePaint);
			}

			// main line drawing
			linePaint.Color = Color.ParseColor("#FFFFFF");
			for (int i = 0; i < points.Count; i++)
			{
				if (i < points.Count - 1)
					canvas.DrawLine(
						points[i].Item1,
						points[i].Item2,
						points[i + 1].Item1,
						points[i + 1].Item2,
						linePaint);

				canvas.DrawCircle(
					cx: points[i].Item1,
					cy: points[i].Item2,
					radius: 5 * density,
					paint: linePaint);
			}

			// text drawing
			linePaint.TextSize = 16f * density;
			linePaint.Color = Color.ParseColor("#FFFFFF");
			for (int i = 0; i < points.Count; i++)
			{
				canvas.DrawText(
					text: points[i].Item3.ToString(),
					x: points[i].Item1,
					y: points[i].Item2 - 2f * density,
					paint: valuePaint);
			}
		}

		void Initialise()
		{
			padding = new Padding
			{
				Left = 20 * Resources.DisplayMetrics.Density,
				Right = 10 * Resources.DisplayMetrics.Density,
				Bottom = 20 * Resources.DisplayMetrics.Density,
				Top = 10 * Resources.DisplayMetrics.Density
			};

			textPaint = new Paint
			{
				TextSize = 10 * Resources.DisplayMetrics.Density,
				Color = Color.ParseColor("#FFFFFF")
			};

			axesPaint = new Paint
			{
				StrokeWidth = 1 * Resources.DisplayMetrics.Density,
				Color = Color.ParseColor("#FFFFFF")
			};

			linePaint = new Paint
			{
				StrokeWidth = 2 * Resources.DisplayMetrics.Density,
				Color = Color.ParseColor("#FFFFFF")
			};

			markersPaint = new Paint
			{
				StrokeWidth = 3 * Resources.DisplayMetrics.Density,
				Color = Color.ParseColor("#448AFF")
			};

			bandsPaint = new Paint
			{
				Color = Color.ParseColor("#2FA2CD")
			};
		}
	}

	[Activity(Label = "BoxRendererTest.Droid", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			base.OnCreate(bundle);

			global::Xamarin.Forms.Forms.Init(this, bundle);

			LoadApplication(new App());
		}
	}
}
