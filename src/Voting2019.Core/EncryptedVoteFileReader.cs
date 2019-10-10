using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using TinyCsvParser;
using TinyCsvParser.Mapping;

namespace Voting2019.Core
{
	public sealed class EncryptedVoteFileReader
	{
		private readonly CsvParser<EncryptedVoteRecord> _csvParser;

		public EncryptedVoteFileReader()
		{
			var csvParserOptions = new CsvParserOptions(true, ';');
			_csvParser = new CsvParser<EncryptedVoteRecord>(csvParserOptions, new EncryptedVoteRecordMap());
		}

		public IEnumerable<EncryptedVoteRecord> ReadRecordFromFile(string fileName)
		{
			var records = _csvParser.ReadFromFile(fileName, Encoding.UTF8);

			foreach (var record in records)
			{
				yield return record.Result;
			}
		}


		public sealed class EncryptedVoteRecord
		{
			public int VoteNumber { get; set; }
			public int District { get; set; }
			public string BlockchainAddress { get; set; }
			public string BlockNumber { get; set; }
			public TimeSpan Time { get; set; }
			public string EncryptedVote
			{
				get { return EncryptedVoteObject == null ? null : System.Text.Json.JsonSerializer.Serialize(EncryptedVoteObject); }
				set { EncryptedVoteObject = System.Text.Json.JsonSerializer.Deserialize<EncryptedVote>(value); }
			}

			public EncryptedVote EncryptedVoteObject { get; set; }
		}

		public sealed class EncryptedVote
		{
			[JsonPropertyName("encryptedA")]
			public string EncryptedA { get; set; }
			[JsonPropertyName("encryptedB")]
			public string EncryptedB { get; set; }
		}

		private sealed class EncryptedVoteRecordMap
			: CsvMapping<EncryptedVoteRecord>
		{
			public EncryptedVoteRecordMap()
			{
				MapProperty(0, x => x.VoteNumber);
				MapProperty(1, x => x.District);
				MapProperty(2, x => x.BlockchainAddress);
				MapProperty(3, x => x.BlockNumber);
				MapProperty(4, x => x.Time);
				MapProperty(5, x => x.EncryptedVote);
			}
		}
	}
}
