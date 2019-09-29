using System;
using System.Collections.Generic;

namespace Voting2019.Core
{
	public sealed class VotingResults
	{
		private readonly Dictionary<long, Candidate> _candidates = new Dictionary<long, Candidate>();

		private readonly List<Vote> _votes = new List<Vote>();

		private VotingStatistics _statistics = new VotingStatistics();

		private MinMaxInterval<int> _blockNumbers;

		private MinMaxInterval<TimeSpan> _times;

		public VotingResults()
		{

		}

		public void AddVote(long district, long candidateId, string candidateName, Vote vote)
		{
			_blockNumbers.UpdateInterval(vote.BlockNumber);
			_times.UpdateInterval(vote.Time);

			if (!_candidates.TryGetValue(candidateId, out Candidate candidate))
			{
				candidate = new Candidate(candidateId, district, candidateName);
				_candidates.Add(candidateId, candidate);
			}
			else
			{
				if (candidate.District != district)
				{
					throw new InvalidOperationException();
				}
			}
			_votes.Add(vote);
		}


		public IReadOnlyDictionary<long, Candidate> Candidates
		{
			get { return _candidates; }
		}

		public IReadOnlyList<Vote> Votes
		{
			get { return _votes; }
		}

		public VotingStatistics Statistics
		{
			get { return _statistics; }
		}

		public void CalculateBaseStatistics()
		{
			_statistics.TotalVotes = _votes.Count;
			_statistics.TotalBlocks = MaxBlockNumber - MinBlockNumber + 1;
			_statistics.TotalTime = EndTime - StartTime;
		}

		public int MinBlockNumber
		{
			get { return _blockNumbers.Min; }
		}

		public int MaxBlockNumber
		{
			get { return _blockNumbers.Max; }
		}

		public TimeSpan StartTime
		{
			get { return _times.Min; }
		}

		public TimeSpan EndTime
		{
			get { return _times.Max; }
		}
	}
}
