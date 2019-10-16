using System;
using System.Collections.Generic;
using System.Linq;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Voting2019.Core;

namespace Voting2019.Visualization
{
	public sealed class TotalCumulativeVotedByTime
		: IPlotModelDrawer
	{
		private readonly PlotModel _plotModel;
		private readonly TimeSpanAxis _xAxis;
		private readonly LinearAxis _yAxis;

		public TotalCumulativeVotedByTime()
		{
			_plotModel = new PlotModel();
			_xAxis = new TimeSpanAxis()
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
			};
			_plotModel.Axes.Add(_yAxis);
		}

		public PlotModel PlotModel
		{
			get { return _plotModel; }
		}

		public void Show(VotingResults votingResults, Func<Vote, bool> filter)
		{
			lock (_plotModel.SyncRoot)
			{
				_xAxis.SetAxisMinMax(TimeSpanAxis.ToDouble(votingResults.StartTime), TimeSpanAxis.ToDouble(votingResults.EndTime));

				_plotModel.Series.Clear();

				LineSeries series = TotalCumulativeVotedByTimeUtils.CreateSeries(votingResults, filter, out var max);
				series.Title = "Всего проголосовало";
				_yAxis.SetAxisMax(max);
				_plotModel.Series.Add(series);
			}
			_plotModel.InvalidatePlot(true);
		}

		public void AddCustomData(string title, IEnumerable<TimeGraphItem<int>> data)
		{
			
			lock (_plotModel.SyncRoot)
			{
				LineSeries series = new LineSeries()
				{
					CanTrackerInterpolatePoints = false,
					Title = title
				};

				foreach(var item in data)
				{
					series.Points.Add(new DataPoint(TimeSpanAxis.ToDouble(item.Time), item.Data));
				}

				_plotModel.Series.Add(series);
			}
			_plotModel.InvalidatePlot(true);
		}

		public void ShowByCandidates(VotingResults votingResults, Func<Vote,bool> filter)
		{
			lock (_plotModel.SyncRoot)
			{
				_xAxis.SetAxisMinMax(TimeSpanAxis.ToDouble(votingResults.StartTime), TimeSpanAxis.ToDouble(votingResults.EndTime));

				_plotModel.Series.Clear();

				var candidates = votingResults.Votes.Where(x => filter(x)).Select(x => x.CandidateId).Distinct();

				int max=0;
				foreach(var candidate in candidates)
				{
					LineSeries series = TotalCumulativeVotedByTimeUtils.CreateSeries(votingResults, x => filter(x) && (x.CandidateId == candidate), out int total);
					if (total > max)
					{
						max = total;
					}
					series.Title = votingResults.Candidates[candidate].Name;
					_plotModel.Series.Add(series);
				}
				_yAxis.SetAxisMax(max);
			}
			_plotModel.InvalidatePlot(true);
		}

		public string GetTimeAxisKey()
		{
			return "x_axis";
		}

		public string GetBlockNumberAxisKey()
		{
			return null;
		}
	}
}
