﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
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
		EventHandler<TouchEventArgs> handler;
		int touchXCoordinate = 0;
		float[] tempHSV = new float[3];

		~GraphViewRenderer()
		{
			this.Touch -= handler;
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

			if (e.PropertyName == GraphView.DataProperty.PropertyName
				|| e.PropertyName == GraphView.GraphOptionsProperty.PropertyName
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
			var options = ((GraphView)Element).GraphOptions;

			DrawPlot(canvas, options, data, touchXCoordinate);
		}

		// Make color darker by altering the third HSV value
		void Darken(ref Color color)
		{
			Color.ColorToHSV(color, tempHSV);
			tempHSV[2] *= .5f;
			color = Color.HSVToColor(tempHSV);
		}

		void DrawPlot(Canvas canvas, GraphOptions options, IEnumerable<GraphData> items, int xSelect)
		{
			var density = Resources.DisplayMetrics.Density;

			var backgroundColor = options.BackgroundColor.ToAndroid();
			var bandsColor = options.BandColor.ToAndroid();
			var lineShadowColor = options.LineShadowColor.ToAndroid();
			var lineColor = options.LineColor.ToAndroid();
			var markerTextShadowColor = options.MarkerTextShadowColor.ToAndroid();
			var markerTextColor = options.MarkerTextColor.ToAndroid();
			var xAxisLabelOffset = options.XAxisLabelOffset * density;
			var yAxisLabelOffset = options.YAxisLabelOffset * density;
			var sectionHeight = options.SectionHeight;
			var lineStrokeWidth = options.LineStrokeWidth * density;
			var axisStrokeWidth = options.AxisStrokeWidth * density;
			var markerTextSize = options.MarkerTextSize * density;
			var markerTextOffset = options.MarkerTextOffset * density;
			var markerShadowTextOffset = options.MarkerShadowTextOffset * density;
			var markerDefaultRadius = options.MarkerDefaultRadius * density;
			var markerSelectedRadius = options.MarkerSelectedRadius * density;
			var labelTextSize = options.LabelTextSize * density;
			var sectionCount = 4;

			/*****************************************
			 * 
			 *  PART 1 - Draw background and bands
			 * 
			 *****************************************/

			// Draws background
			paint.Color = Color.ParseColor("#2CBCEB");
			canvas.DrawRect(new Rect(0, 0, this.Width, this.Height), paint);

			// Set text size to measure text
			paint.TextSize = labelTextSize;

			var ceilingValue = Math.Ceiling(items.Max(i => i.Y) / 50.0) * 50.0;

			// Computes plot boundaries
			// - Left: left padding + maximum text lenght + 2dp
			// - Bottom: bottom padding + text size
			var plotBoundaries = new PlotBoundaries
			{
				Left = options.Padding.Left * density + paint.MeasureText(ceilingValue.ToString()) + yAxisLabelOffset,
				Right = this.Width - options.Padding.Right * density,
				Top = options.Padding.Top * density,
				Bottom = this.Height - options.Padding.Bottom * density - paint.TextSize - xAxisLabelOffset
			};
			var plotWidth = plotBoundaries.Right - plotBoundaries.Left;
			var plotHeight = plotBoundaries.Bottom - plotBoundaries.Top;

			var verticalSection = new Section
			{
				Width = plotWidth / items.Count(),
				Count = items.Count()
			};
			var horizontalSection = new Section
			{
				Max = (float)ceilingValue,
				Count = sectionCount,
				Width = plotHeight / sectionCount
			};

			// Special function not present in the tutorial
			// Darken plot if xSelect within boundaries
			if (plotBoundaries.Left <= xSelect && xSelect <= plotBoundaries.Right)
			{
				Darken(ref backgroundColor);
				Darken(ref bandsColor);
				Darken(ref lineShadowColor);
				Darken(ref lineColor);
			}

			// Draws horizontal bands
			paint.Reset();
			paint.Color = bandsColor;
			for (int i = horizontalSection.Count - 1; i >= 0; i = i - 2)
			{
				var y = plotBoundaries.Bottom - horizontalSection.Width * i;

				canvas.DrawRect(
					left: plotBoundaries.Left,
					top: y - horizontalSection.Width,
					right: plotBoundaries.Right,
					bottom: y,
					paint: paint);
			}

			/*****************************************
			 * 
			 *  PART 2 - Draw axis and labels
			 * 
			 *****************************************/

			// Draws X and Y axis lines
			paint.Reset();
			paint.StrokeWidth = axisStrokeWidth;
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


			//// Calculates all the data coordinates
			var points = new List<Tuple<float, float, string, double, bool>>();
			foreach (var l in items.Select((l, index) => Tuple.Create(l.X, l.Y, index)))
			{
				var x = verticalSection.Width * (l.Item3 + 0.5f) + plotBoundaries.Left;
				var y = (float)l.Item2 * plotHeight / horizontalSection.Max;

				points.Add(
					Tuple.Create(
						x,
						plotBoundaries.Bottom - y,
						l.Item1,
						l.Item2,
						plotBoundaries.Left + l.Item3 * verticalSection.Width <= xSelect
						&& xSelect < plotBoundaries.Left + (l.Item3 + 1) * verticalSection.Width
					));
			}

			// Draws X axis labels
			paint.Reset();
			paint.TextAlign = Paint.Align.Center;
			paint.TextSize = labelTextSize;
			foreach (var p in points)
			{
				if (p.Item5)
				{
					paint.Color = markerTextColor;

					canvas.DrawText(
						text: p.Item3,
						x: p.Item1,
						y: plotBoundaries.Bottom + paint.TextSize + xAxisLabelOffset,
						paint: paint);
				}
				else
				{
					paint.Color = lineColor;

					canvas.DrawText(
						text: p.Item3,
						x: p.Item1,
						y: plotBoundaries.Bottom + paint.TextSize + xAxisLabelOffset,
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
			for (int i = 0; i < horizontalSection.Count; i++)
			{
				var y = plotBoundaries.Bottom - horizontalSection.Width * i;

				canvas.DrawText(
					text: (i * sectionHeight).ToString(),
					x: plotBoundaries.Left - yAxisLabelOffset,
					y: y - (paint.Ascent() / 2f + 1.5f * density),
					paint: paint);
			}


			/*****************************************
			 * 
			 *  PART 3 - Draw lines and markers
			 * 
			 *****************************************/

			// Draws line shadow
			paint.Reset();
			paint.StrokeWidth = lineStrokeWidth;
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
					radius: markerDefaultRadius,
					paint: paint);
			}

			//// Draws main line
			paint.Reset();
			paint.StrokeWidth = lineStrokeWidth;
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
					radius: markerDefaultRadius,
					paint: paint);
			}


			/*****************************************
			 * 
			 *  PLUS - Draw text
			 * 
			 *****************************************/

			// Draws markers
			paint.Reset();
			for (int i = 0; i < points.Count; i++)
			{
				if (points[i].Item5)
				{
					paint.Color = markerTextShadowColor;
					canvas.DrawCircle(
						cx: points[i].Item1,
						cy: points[i].Item2 + 2f * density,
						radius: markerSelectedRadius,
						paint: paint);
				}

				if (points[i].Item5)
				{
					paint.Color = markerTextColor;
					canvas.DrawCircle(
						cx: points[i].Item1,
						cy: points[i].Item2,
						radius: markerSelectedRadius,
						paint: paint);
				}
			}

			// Draws marker text shadow
			// Draws marker text
			paint.Reset();
			paint.TextAlign = Paint.Align.Center;
			paint.TextSize = markerTextSize;
			paint.SetTypeface(Typeface.Create(Typeface.Default, TypefaceStyle.Bold));
			paint.Color = markerTextShadowColor;
			for (int i = 0; i < points.Count; i++)
			{
				var text = points[i].Item4.ToString();

				if (points[i].Item5)
				{
					paint.Color = markerTextShadowColor;

					// Prevent markers from being drawn out of Y axis
					var position = points[i].Item2 - markerTextOffset - paint.TextSize < 0 ? -1 : 1;

					canvas.DrawText(
						text: text,
						x: points[i].Item1,
						y: points[i].Item2 - markerShadowTextOffset * position,
						paint: paint);

					paint.Color = markerTextColor;

					canvas.DrawText(
						text: text,
						x: points[i].Item1,
						y: points[i].Item2 - markerTextOffset * position,
						paint: paint);
				}
			}
		}


		class PlotBoundaries
		{
			public float Left { get; set; }
			public float Right { get; set; }
			public float Top { get; set; }
			public float Bottom { get; set; }
		}

		class Section
		{
			public int Count { get; set; }
			public float Width { get; set; }
			public float Max { get; set; }
		}
	}
}

