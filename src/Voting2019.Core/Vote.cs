using System;

namespace Voting2019.Core
{
	public sealed class Vote
	{
		public int VoteNumber { get; set; }
		public int DistrictNumber { get; set; }
		public string BlockchainAddress { get; set; }
		public int BlockNumber { get; set; }
		public TimeSpan Time { get; set; }
		public long CandidateId { get; set; }
	}
}
