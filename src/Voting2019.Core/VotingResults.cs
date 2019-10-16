using System;
using System.Collections.Generic;
using System.Linq;

namespace Voting2019.Core
{
	public sealed class VotingResults
	{
		private readonly Dictionary<long, Candidate> _candidates;
		private readonly Vote[] _votes;
		private readonly VotingStatistics _votingStatistics;
		private readonly MinMaxInterval<int> _blockNumbers;
		private readonly MinMaxInterval<TimeSpan> _times;

		private List<AnomalyZoneDefinition> _anomalyZones = new List<AnomalyZoneDefinition>();

		internal VotingResults(Dictionary<long,Candidate> candidates,IList<Vote> votes,VotingStatistics votingStatistics, MinMaxInterval<int> blockNumbers, MinMaxInterval<TimeSpan> times)
		{
			_candidates = candidates;
			_votes = votes.ToArray();
			_votingStatistics = votingStatistics;
			_blockNumbers = blockNumbers;
			_times = times;
		}

		


		public IReadOnlyDictionary<long, Candidate> Candidates
		{
			get { return _candidates; }
		}

		public IReadOnlyList<Vote> Votes
		{
			get { return _votes; }
		}

		public IReadOnlyList<AnomalyZoneDefinition> Anomalies
		{
			get { return _anomalyZones; }
		}

		public VotingStatistics Statistics
		{
			get { return _votingStatistics; }
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

		public int GetBlockNumberNearestToTime(TimeSpan time)
		{
			// мне лень оптимизировать этот код
			if (time <= _votes[0].Time)
			{
				return _votes[0].BlockNumber;
			}
			if (time >= _votes[_votes.Length - 1].Time)
			{
				return _votes[_votes.Length - 1].BlockNumber;
			}

			for (int i = 0; i < _votes.Length -2; i++)
			{
				TimeSpan startTime = _votes[i].Time;
				TimeSpan endTime = _votes[i + 1].Time;
				if ((time > startTime) && (time < endTime))
				{
					var nearestTime = time.MapToNearestTime(startTime, endTime);
					return (nearestTime == startTime) ? _votes[i].BlockNumber : _votes[i + 1].BlockNumber;
				}
			}
			throw new InvalidOperationException();
		}

		public TimeSpan GetKnownBlockWriteTime(int blockNumber)
		{
			if (blockNumber <= MinBlockNumber)
			{
				return StartTime;
			}
			if (blockNumber >= MaxBlockNumber)
			{
				return EndTime;
			}
			for (int i=0;i < _votes.Length - 2; i++)
			{
				if (_votes[i+1].BlockNumber == blockNumber)
				{
					return _votes[i+1].Time;
				}
				if ((blockNumber > _votes[i].BlockNumber) && (blockNumber < _votes[i + 1].BlockNumber))
				{
					return _votes[i].Time;
				}
			}
			throw new InvalidOperationException();
		}


		public void DefineAnomalyZoneByBlocks(string name,int startBlock,int endBlock)
		{
			_anomalyZones.Add(new AnomalyZoneDefinition(name, startBlock, endBlock, GetKnownBlockWriteTime(startBlock), GetKnownBlockWriteTime(endBlock)));
		}
	}
}
