using System;
using System.Collections.Generic;
using System.Linq;

using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

using Voting2019.Core;

namespace Voting2019.Visualization
{
	public sealed class TotalVotesByBlockDistribution
		: IPlotModelDrawer
	{
		private readonly PlotModel _plotModel;
		private readonly LinearAxis _xAxis;
		private readonly LinearAxis _yAxis;


		public TotalVotesByBlockDistribution()
		{
			_plotModel = new PlotModel();
			_xAxis = new LinearAxis()
			{
				Position = AxisPosition.Bottom,
				Key = "x_axis"
			};
			_plotModel.Axes.Add(_xAxis);
			_yAxis = new LinearAxis()
			{
				Position = AxisPosition.Left,
				Key = "y_axis",
				AbsoluteMinimum = 0,
				AbsoluteMaximum = 0
			};
			_plotModel.Axes.Add(_yAxis);
		}


		public PlotModel PlotModel
		{
			get { return _plotModel; }
		}


		public void Show(VotingResults votingResults)
		{
			Show(votingResults, x => true);
		}

		public void Show(VotingResults votingResults,Func<Vote,bool> filter)
		{
			lock (_plotModel.SyncRoot)
			{
				_xAxis.SetMinMaxBlocksToXAxis(votingResults);

				_plotModel.Series.Clear();

				var series = new LineSeries()
				{
					CanTrackerInterpolatePoints = false
				};

				_plotModel.Series.Add(series);

				var points = votingResults.Votes
					.Where(x => filter(x))
					.GroupBy(x => x.BlockNumber, (key, votes) => (blockNumber: key, votesCount: votes.Count()))
					.OrderBy(x => x.blockNumber)
					.ToArray();

				ref var lastPoint = ref points[0];
				series.Points.Add(new DataPoint(lastPoint.blockNumber, lastPoint.votesCount));
				int maxVotes = lastPoint.votesCount;
				for (int i = 1; i < points.Length; i++)
				{
					ref var currentPoint = ref points[i];
					for (int point = lastPoint.blockNumber + 1; point < currentPoint.blockNumber; point++)
					{
						series.Points.Add(new DataPoint(point, 0));
					}
					if (currentPoint.votesCount > maxVotes)
					{
						maxVotes = currentPoint.votesCount;
					}
					series.Points.Add(new DataPoint(currentPoint.blockNumber, currentPoint.votesCount));
					lastPoint = currentPoint;
				}
				_yAxis.SetAxisMax(maxVotes);
			}
			_plotModel.InvalidatePlot(true);
		}

		public void AddTimeSeries(string tag,IEnumerable<BlockGraphItem<int>> points)
		{
			lock (_plotModel.SyncRoot)
			{
				var series = (LineSeries) _plotModel.Series.Where(x => ((string) x.Tag) == tag).SingleOrDefault();
				if (series == null)
				{
					series = new LineSeries()
					{
						CanTrackerInterpolatePoints = false,
						Tag = tag
					};
					_plotModel.Series.Add(series);
				}
				else
				{
					series.Points.Clear();
				}
				foreach(var point in points)
				{
					series.Points.Add(new DataPoint(point.BlockNumber, point.Data));
				}
			}

			_plotModel.InvalidatePlot(true);
		}

		public string GetTimeAxisKey()
		{
			return null;
		}

		public string GetBlockNumberAxisKey()
		{
			 return "x_axis";
		}
	}
}
