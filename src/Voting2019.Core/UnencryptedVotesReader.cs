using System;
using System.Text;
using TinyCsvParser;
using TinyCsvParser.Mapping;

namespace Voting2019.Core
{
	public sealed class UnencryptedVoteFileReader
	{
		private readonly CsvParser<UnencryptedVoteRecord> _csvParser;

		public UnencryptedVoteFileReader()
		{
			var csvParserOptions = new CsvParserOptions(true, ',');
			_csvParser = new CsvParser<UnencryptedVoteRecord>(csvParserOptions, new UnencryptedVoteRecordMap());
		}

		public VotingResults ReadFromFile(string fileName)
		{
			var records = _csvParser.ReadFromFile(fileName, Encoding.UTF8);


			VotingResults ret = new VotingResults();

			foreach (var record in records)
			{
				var result = record.Result;

				Vote vote = new Vote()
				{
					VoteNumber = result.Number,
					DistrictNumber = result.District,
					BlockNumber = int.Parse(result.BlockNumber.AsSpan().Slice(1)),
					BlockchainAddress = result.BlockchainAddress,
					Time = TimeSpan.Parse(result.Time),
					CandidateId = result.CandidateId
				};

				ret.AddVote(result.District, result.CandidateId, result.CandidateName, vote);
			}
			ret.CalculateBaseStatistics();
			return ret;
		}

		private sealed class UnencryptedVoteRecord
		{
			public int Number { get; set; }
			public int District { get; set; }
			public string BlockchainAddress { get; set; }
			public string BlockNumber { get; set; }
			public string Time { get; set; }
			public long CandidateId { get; set; }

			public string CandidateName { get; set; }
		}

		private sealed class UnencryptedVoteRecordMap
			: CsvMapping<UnencryptedVoteRecord>
		{
			public UnencryptedVoteRecordMap()
			{
				MapProperty(0, x => x.Number);
				MapProperty(1, x => x.District);
				MapProperty(2, x => x.BlockchainAddress);
				MapProperty(3, x => x.BlockNumber);
				MapProperty(4, x => x.Time);
				MapProperty(5, x => x.CandidateId);
				MapProperty(6, x => x.CandidateName);
			}
		}
	}
}
