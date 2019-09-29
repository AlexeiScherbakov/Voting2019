
using System;
using System.Linq;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Voting2019.Core;

namespace Voting2019.Visualization
{
	internal static class TotalCumulativeVotedByTimeHelper
	{
		public static LineSeries CreateSeries( VotingResults votingResults, Func<Vote, bool> filter,out int total)
		{
			LineSeries series = new LineSeries()
			{
				CanTrackerInterpolatePoints = false
			};

			var points = votingResults.Votes
				.Where(x => filter(x))
				.GroupBy(x => x.Time, (key, votes) => (time: key, votesCount: votes.Count()))
				.OrderBy(x => x.time)
				.ToArray();

			int totalVotedByTime = 0;
			for (int i = 0; i < points.Length; i++)
			{
				totalVotedByTime += points[i].votesCount;
				series.Points.Add(new DataPoint(TimeSpanAxis.ToDouble(points[i].time), totalVotedByTime));
			}
			total = totalVotedByTime;
			return series;
		}
	}
}
