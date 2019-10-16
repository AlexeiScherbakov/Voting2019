using System;
using System.Collections.Generic;

namespace Voting2019.Core
{
	public sealed class RemoteVotingStatisticsBuilder
	{
		private readonly List<StatisticsItem> _statistics = new List<StatisticsItem>();

		public IReadOnlyList<StatisticsItem> Statistics
		{
			get { return _statistics; }
		}

		public RemoteVotingStatisticsBuilder AddStatisticsItem(TimeSpan time, int comeToRegistrationPage, int validatedBySms, int registered, int voted)
		{
			_statistics.Add(new StatisticsItem(time, comeToRegistrationPage, validatedBySms, registered, voted));
			return this;
		}
		public RemoteVotingStatisticsBuilder AddStatisticsItem(string time, int comeToRegistrationPage, int validatedBySms, int registered, int voted)
		{
			var timeSpan = TimeSpan.Parse(time);
			_statistics.Add(new StatisticsItem(timeSpan, comeToRegistrationPage, validatedBySms, registered, voted));
			return this;
		}

		public RemoteVotingStatistics Create()
		{
			return new RemoteVotingStatistics(_statistics);
		}
	}
}
