using System;
using System.Collections.Generic;

namespace Voting2019.Core
{
	internal sealed class VotingResultsBuilder
	{
		private readonly Dictionary<long, Candidate> _candidates = new Dictionary<long, Candidate>();

		private readonly List<Vote> _votes = new List<Vote>();

		private VotingStatistics _statistics = new VotingStatistics();

		private MinMaxInterval<int> _blockNumbers;

		private MinMaxInterval<TimeSpan> _times;


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

		private void CalculateBaseStatistics()
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


		public VotingResults Create()
		{
			CalculateBaseStatistics();
			return new VotingResults(_candidates, _votes, _statistics, _blockNumbers, _times);
		}
	}
}
