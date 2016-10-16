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
	public class PlotBoundaries
	{
		public float Left { get; set; }
		public float Right { get; set; }
		public float Top { get; set; }
		public float Bottom { get; set; }
	}

	public class GraphViewRenderer : BoxRenderer
	{
		Paint paint = new Paint();
		EventHandler<TouchEventArgs> handler;
		int touchXCoordinate = -1;
		float[] tempHSV = new float[3];

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
						//Reset coordinate on double tap
						touchXCoordinate = newXCoordinate < 100 ? -1 : newXCoordinate;
						this.Invalidate();
					}
				};

				this.Touch += handler;
			}
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

			DrawPlot(canvas, this.Width, this.Height, padding, Resources.DisplayMetrics.Density, data, touchXCoordinate);
		}

		void Darken(ref Color color)
		{ 
			Color.ColorToHSV(color, tempHSV);
			tempHSV[2] *= .5f;
			color = Color.HSVToColor(tempHSV);
		}

		void DrawPlot(Canvas canvas, int viewWidth, int viewHeight, Padding padding, float density, IEnumerable<Data> items, int xSelect)
		{
			var backgroundColor = Color.ParseColor("#2CBCEB");
			var bandsColor = Color.ParseColor("#36ACD4");
			var lineShadowColor = Color.ParseColor("#1A7596");
			var lineColor = Color.ParseColor("#EDEDED");
			var labelTextSize = 12f * density;

			if (xSelect != -1)
			{
				Darken(ref backgroundColor);
				Darken(ref bandsColor);
				Darken(ref lineShadowColor);
				Darken(ref lineColor);
			}

			var markerTextShadowColor = Color.ParseColor("#0E3D4D");
			var markerTextColor = Color.ParseColor("#FFFFFF");

			paint.Color = backgroundColor;
			canvas.DrawRect(new Rect(0, 0, viewWidth, viewHeight), paint);

			// Set text size to measure text
			paint.TextSize = labelTextSize;

			var ceilingValue = (int)Math.Ceiling(items.Max(i => i.Y) / 100f) * 100f;

			// Computes plot boundaries
			// - Left: left padding + maximum text lenght + 2dp
			// - Bottom: bottom padding + text size
			var plotBoundaries = new PlotBoundaries
			{
				Left = padding.Left * density + paint.MeasureText(ceilingValue.ToString()) + 2f * density,
				Right = viewWidth - padding.Right * density,
				Top = padding.Top * density,
				Bottom = viewHeight - padding.Bottom * density - paint.TextSize - 2f * density
			};
			var plotWidth = plotBoundaries.Right - plotBoundaries.Left;
			var plotHeight = plotBoundaries.Bottom - plotBoundaries.Top;

			var horizontalSection = new Section
			{
				Width = plotWidth / items.Count(),
				Count = items.Count()
			};
			var verticalSection = new Section
			{
				Max = ceilingValue,
				Count = (int)Math.Ceiling(items.Select(i => i.Y).Max() / 100),
				Width = plotHeight / ((int)Math.Ceiling(items.Select(i => i.Y).Max() / 100))
			};

			// Calculates all the data coordinates
			var points = new List<Tuple<float, float, double, bool>>();
			foreach (var l in items.Select((l, index) => Tuple.Create(l.X, l.Y, index)))
			{
				var x = horizontalSection.Width * (l.Item3 + 1f / 2f) + plotBoundaries.Left;
				var y = (float)l.Item2 * plotHeight / verticalSection.Max;

				points.Add(
					Tuple.Create(
						x, 
						plotBoundaries.Bottom - y, 
						l.Item2, 
						plotBoundaries.Left + l.Item3 * horizontalSection.Width <= xSelect 
						&& xSelect < plotBoundaries.Left + (l.Item3 + 1) * horizontalSection.Width
					));
			}

			// Draws horizontal bands
			paint.Reset();
			paint.Color = bandsColor;
			for (int i = verticalSection.Count - 1; i >= 0; i = i - 2)
			{
				var y = plotBoundaries.Bottom - verticalSection.Width * i;

				canvas.DrawRect(
					left: plotBoundaries.Left,
					top: y - verticalSection.Width,
					right: plotBoundaries.Right,
					bottom: y,
					paint: paint);
			}

			// Draws line shadow
			paint.Reset();
			paint.StrokeWidth = 3f * density;
			paint.Color = lineShadowColor;
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
					radius: 4f * density,
					paint: paint);
			}

			// Draws main line
			paint.Reset();
			paint.StrokeWidth = 3f * density;
			paint.Color = lineColor;
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
					radius: 4f * density,
					paint: paint);
			}

			// Draws X axis labels
			paint.Reset();
			paint.TextAlign = Paint.Align.Center;
			paint.TextSize = labelTextSize;
			foreach (var l in items.Select((Data l, int index) => Tuple.Create(l.X, index)))
			{
				var x = horizontalSection.Width * (l.Item2 + 1f / 2f) + plotBoundaries.Left;

				if (points[l.Item2].Item4)
				{
					paint.Color = markerTextColor;
					
					canvas.DrawText(
						text: l.Item1,
						x: x,
						y: plotBoundaries.Bottom + paint.TextSize + 2f * density,
						paint: paint);
				}
				else 
				{
					paint.Color = lineColor;
					
					canvas.DrawText(
						text: l.Item1,
						x: x,
						y: plotBoundaries.Bottom + paint.TextSize + 2f * density,
						paint: paint);
				}
			}

			// Draw Y axis labels
			// The 1.5f * density on y is a hack to get the label aligned vertically.
			// It will need adjustements if the font size changes.
			paint.Reset();
			paint.TextAlign = Paint.Align.Right;
			paint.TextSize = labelTextSize;
			paint.Color = lineColor;
			for (int i = 0; i < verticalSection.Count; i++)
			{
				var y = plotBoundaries.Bottom - verticalSection.Width * i;

				canvas.DrawText(
					text: (i * 100).ToString(),
					x: plotBoundaries.Left - 2f * density,
					y: y - (paint.Ascent() / 2f + 1.5f * density),
					paint: paint);
			}

			// Draws X and Y axis lines
			paint.Reset();
			paint.StrokeWidth = 2f * density;
			paint.Color = lineColor;
			canvas.DrawLine(
				plotBoundaries.Left,
				plotBoundaries.Bottom,
				plotBoundaries.Right,
				plotBoundaries.Bottom,
				paint);
			canvas.DrawLine(
				plotBoundaries.Left,
				plotBoundaries.Top,
				plotBoundaries.Left,
				plotBoundaries.Bottom,
				paint);

			// Draws markers
			paint.Reset();
			for (int i = 0; i < points.Count; i++)
			{
				if (points[i].Item4)
				{
					paint.Color = markerTextShadowColor;
					canvas.DrawCircle(
						cx: points[i].Item1,
						cy: points[i].Item2 + 2f * density,
						radius: 5 * density,
						paint: paint);
				}

				if (points[i].Item4)
				{
					paint.Color = markerTextColor;
					canvas.DrawCircle(
						cx: points[i].Item1,
						cy: points[i].Item2,
						radius: 5 * density,
						paint: paint);
				}
			}

			// Draws marker text shadow
			// Draws marker text
			paint.Reset();
			paint.TextAlign = Paint.Align.Center;
			paint.TextSize = 16f * density;
			paint.SetTypeface(Typeface.Create(Typeface.Default, TypefaceStyle.Bold));
			paint.Color = markerTextShadowColor;
			for (int i = 0; i < points.Count; i++)
			{
				var text = points[i].Item3.ToString();

				if (points[i].Item4)
				{
					paint.Color = markerTextShadowColor;

					canvas.DrawText(
						text: text,
						x: points[i].Item1,
						y: points[i].Item2 - 8f * density,
						paint: paint);
					
					canvas.DrawText(
						text: text,
						x: points[i].Item1,
						y: points[i].Item2 - 8f * density,
						paint: paint);

					paint.Color = markerTextColor;

					canvas.DrawText(
						text: text,
						x: points[i].Item1,
						y: points[i].Item2 - 10f * density,
						paint: paint);
				}
			}
		}
	}
}
