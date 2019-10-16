
using System;
using System.Linq;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Voting2019.Core;

namespace Voting2019.Visualization
{
	internal static class TotalCumulativeVotedByTimeUtils
	{
		public static LineSeries CreateSeries( VotingResults votingResults, Func<Vote, bool> filter,out int total)
		{
			LineSeries series = new LineSeries()
			{
				CanTrackerInterpolatePoints = false
			};

			var points = votingResults.Votes
				.Where(x => filter(x))
				.GroupBy(x => x.Time, (key, votes) => new TimeGraphItem<int>(key, votes.Count()))
				.OrderBy(x => x.Time)
				.ToArray();

			int totalVotedByTime = 0;
			for (int i = 0; i < points.Length; i++)
			{
				totalVotedByTime += points[i].Data;
				series.Points.Add(new DataPoint(TimeSpanAxis.ToDouble(points[i].Time), totalVotedByTime));
			}
			total = totalVotedByTime;
			return series;
		}
	}
}
