
using System.Collections.Generic;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using Voting2019.Core;

namespace Voting2019.Visualization
{
	public interface IPlotModelDrawer
	{
		PlotModel PlotModel { get; }

		string GetTimeAxisKey();
		string GetBlockNumberAxisKey();
	}



	public static class PlotModelDrawerExtensions
	{
		public static void SetTitle(this IPlotModelDrawer @this,string title)
		{
			@this.PlotModel.Title = title;
		}

		private static RectangleAnnotation CreateAnomalyZoneForAxis(Axis axis, double start,double end,string name)
		{
			RectangleAnnotation annotation = new RectangleAnnotation
			{
				Stroke = OxyColors.Transparent,
				StrokeThickness = 0,
				ClipByYAxis = false,
				ClipByXAxis = false
			};

			if ((axis.Position == AxisPosition.Bottom) || (axis.Position == AxisPosition.Top))
			{
				annotation.MinimumX = start;
				annotation.MaximumX = end;
				annotation.TextRotation = -90;
				annotation.XAxisKey = axis.Key;
			}
			else
			{
				annotation.MinimumY = start;
				annotation.MaximumY = end;
				annotation.YAxisKey = axis.Key;
			}
			annotation.Text = name;

			return annotation;
		}

		public static void DrawAnomalyZones(this IPlotModelDrawer @this,IEnumerable<AnomalyZoneDefinition> anomalyZones)
		{
			OxyColor[] anomalyZoneColors = new OxyColor[]
			{
				// каждый охотник желает знать где сидит фазан
				OxyColor.FromAColor(100, OxyColors.Red),
				OxyColor.FromAColor(100, OxyColors.Orange),
				OxyColor.FromAColor(100, OxyColors.Yellow),
				OxyColor.FromAColor(100,OxyColors.Violet)
			};
			int counter = 0;
			lock (@this.PlotModel.SyncRoot)
			{
				foreach (var anomalyZone in anomalyZones)
				{
					var color = anomalyZoneColors[counter % anomalyZoneColors.Length];

					var timeAxisKey = @this.GetTimeAxisKey();
					if (timeAxisKey != null)
					{
						double start = TimeSpanAxis.ToDouble(anomalyZone.StartTime);
						double end = TimeSpanAxis.ToDouble(anomalyZone.EndTime);
						var timeAxis = @this.PlotModel.GetAxis(timeAxisKey);

						var annotation = CreateAnomalyZoneForAxis(timeAxis, start, end, anomalyZone.Name);
						annotation.Fill = color;
						@this.PlotModel.Annotations.Add(annotation);
					}
					var blockNumberAxisKey = @this.GetBlockNumberAxisKey();
					if (blockNumberAxisKey != null)
					{
						var blockNumberAxis = @this.PlotModel.GetAxis(blockNumberAxisKey);
						var annotation = CreateAnomalyZoneForAxis(blockNumberAxis, anomalyZone.StartBlock, anomalyZone.EndBlock, anomalyZone.Name);
						annotation.Fill = color;
						@this.PlotModel.Annotations.Add(annotation);
					}
					counter++;
				}
			}
			@this.PlotModel.InvalidatePlot(false);
		}
	}
}
