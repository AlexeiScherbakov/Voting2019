using System;

using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Voting2019.Core;

namespace Voting2019.Visualization
{
	public sealed class StatisticsPlotModelHelper
		: IPlotModelDrawer
	{
		private readonly PlotModel _plotModel;

		private readonly TimeSpanAxis _xAxis;

		public StatisticsPlotModelHelper()
		{
			_plotModel = new PlotModel();
			_xAxis = new TimeSpanAxis()
			{
				Position = AxisPosition.Bottom,
				Key = "x_axis",
				AbsoluteMinimum = TimeSpanAxis.ToDouble(TimeSpan.Zero),
				Minimum = TimeSpanAxis.ToDouble(TimeSpan.Zero)
			};
			_plotModel.Axes.Add(_xAxis);

			var yAxis = new LinearAxis()
			{
				Position = AxisPosition.Left,
				Key = "y_axis",
				AbsoluteMinimum = 0,
				Minimum = 0
			};
			_plotModel.Axes.Add(yAxis);

		}

		public PlotModel PlotModel
		{
			get { return _plotModel; }
		}

		public void Show(RemoteVotingStatistics statistics)
		{
			lock (_plotModel.SyncRoot)
			{
				_plotModel.Series.Clear();

				var series1 = new LineSeries()
				{
					CanTrackerInterpolatePoints = false,
					Title = "Перешло на страницу голосования"
				};
				var series2 = new LineSeries()
				{
					CanTrackerInterpolatePoints = false,
					Title = "Правильно ввели регистрационную СМС"
				};
				var series3 = new LineSeries()
				{
					CanTrackerInterpolatePoints = false,
					Title = "Выдано бюллететеней"
				};
				var series4 = new LineSeries()
				{
					CanTrackerInterpolatePoints = false,
					Title = "Проголосовало"
				};
				_plotModel.Series.Add(series1);
				_plotModel.Series.Add(series2);
				_plotModel.Series.Add(series3);
				_plotModel.Series.Add(series4);
				foreach(var item in statistics.Statistics)
				{
					series1.Points.Add(new DataPoint(TimeSpanAxis.ToDouble(item.Time), item.ComeToRegistrationPage));
					series2.Points.Add(new DataPoint(TimeSpanAxis.ToDouble(item.Time), item.ValidatedBySms));
					series3.Points.Add(new DataPoint(TimeSpanAxis.ToDouble(item.Time), item.Registered));
					series4.Points.Add(new DataPoint(TimeSpanAxis.ToDouble(item.Time), item.Voted));
				}
				_xAxis.SetAxisMinMax(statistics.StartTime, statistics.EndTime);
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
