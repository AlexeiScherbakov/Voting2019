using System;

namespace Voting2019.Core
{
	public sealed class VotingStatistics
	{
		public int TotalVotes { get; set; }

		public int TotalBlocks { get; set; }

		public TimeSpan TotalTime { get; set; }
	}
}
