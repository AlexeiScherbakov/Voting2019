using System;
using System.Collections.Generic;

namespace Voting2019.Core
{
	public sealed class RemoteVotingStatistics
	{
		private readonly StatisticsItem[] _statistics;

		internal RemoteVotingStatistics(List<StatisticsItem> statistics)
		{
			_statistics = statistics.ToArray();

			Array.Sort(_statistics, (x, y) => x.Time.CompareTo(y.Time));
		}

		public IReadOnlyList<StatisticsItem> Statistics
		{
			get { return _statistics; }
		}

		public TimeSpan StartTime
		{
			get { return _statistics[0].Time; }
		}

		public TimeSpan EndTime
		{
			get { return _statistics[_statistics.Length - 1].Time; }
		}
	}
}
