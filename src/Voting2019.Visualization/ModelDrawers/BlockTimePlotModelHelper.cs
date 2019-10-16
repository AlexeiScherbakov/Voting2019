using System;
using System.Linq;

using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

using Voting2019.Core;

namespace Voting2019.Visualization
{
	public sealed class BlockTimePlotModelHelper
		: IPlotModelDrawer
	{
		private readonly PlotModel _plotModel;

		private const string XAxisKey = "x_axis";

		public BlockTimePlotModelHelper()
		{
			_plotModel = new PlotModel();
			var xAxis = new LinearAxis()
			{
				Position = AxisPosition.Bottom,
				Key = XAxisKey
			};
			_plotModel.Axes.Add(xAxis);

			var yAxis = new TimeSpanAxis()
			{
				Position = AxisPosition.Left,
				Key = "y_axis",
				AbsoluteMinimum = TimeSpanAxis.ToDouble(TimeSpan.Zero),
				Minimum =  TimeSpanAxis.ToDouble(TimeSpan.Zero)
			};
			_plotModel.Axes.Add(yAxis);
		}

		public PlotModel PlotModel
		{
			get { return _plotModel; }
		}

		private LineSeries PrepareSeries()
		{
			LineSeries series = null;
			if (_plotModel.Series.Count == 1)
			{
				series = (LineSeries) _plotModel.Series[0];
				series.Points.Clear();
			}
			else
			{
				_plotModel.Series.Clear();
				series = new LineSeries()
				{
					CanTrackerInterpolatePoints = false
				};
				_plotModel.Series.Add(series);
			}
			return series;
		}

		public void ShowBlockTime(VotingResults votingResults)
		{
			lock (_plotModel.SyncRoot)
			{
				var xAxis=_plotModel.Axes.Where(x => x.Key == XAxisKey).Single();
				xAxis.SetMinMaxBlocksToXAxis(votingResults);

				var series = PrepareSeries();

				var maxTime=BlockTimeVisualHelper.LoadBlockTimesToSeries( votingResults, series, null);
				var yAxis = _plotModel.Axes.Where(x => x.Key == "y_axis").Single();
				yAxis.SetAxisMax(TimeSpanAxis.ToDouble(maxTime));
			}
			_plotModel.InvalidatePlot(true);
		}

		public void ShowBlockStartTime(VotingResults votingResults)
		{
			lock (_plotModel.SyncRoot)
			{
				var xAxis = _plotModel.Axes.Where(x => x.Key == XAxisKey).Single();
				xAxis.SetMinMaxBlocksToXAxis(votingResults);

				var series = PrepareSeries();

				BlockTimeVisualHelper.LoadBlockTimesToSeries(votingResults, null, series);
				var yAxis = _plotModel.Axes.Where(x => x.Key == "y_axis").Single();
				yAxis.SetAxisMax(TimeSpanAxis.ToDouble(votingResults.Votes.Select(x => x.Time).Max()));
			}
			_plotModel.InvalidatePlot(true);
		}

		public string GetTimeAxisKey()
		{
			return null;
		}

		public string GetBlockNumberAxisKey()
		{
			return XAxisKey;
		}
	}
}
