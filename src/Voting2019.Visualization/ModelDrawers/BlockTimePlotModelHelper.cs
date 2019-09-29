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

		public BlockTimePlotModelHelper()
		{
			_plotModel = new PlotModel();
			var xAxis = new LinearAxis()
			{
				Position = AxisPosition.Bottom,
				Key = "x_axis"
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


		public void UpdateBlockTime(VotingResults votingResults)
		{
			lock (_plotModel.SyncRoot)
			{
				var xAxis=_plotModel.Axes.Where(x => x.Key == "x_axis").Single();
				xAxis.SetMinMaxBlocksToXAxis(votingResults);

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

				BlockTimeVisualHelper.LoadBlockTimesToPlot(_plotModel, votingResults, series, null, "y_axis", null);
			}
			_plotModel.InvalidatePlot(true);
		}

		
	}
}
