using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities.Encoders;

namespace Voting2019.Core
{
	public sealed class DatasetsValidator
	{
		private readonly VotingResults _votingResults;
		private readonly EncryptedVoteFileReader.EncryptedVoteRecord[] _encryptedVotes;
		private readonly ElGamalDecryptor _elGamalDecryptor;

		private List<string> _validationErrors = new List<string>();

		private int _validationStarted;

		private HashSet<string> _abCounter = new HashSet<string>();

		private List<int> _invalidVotes = new List<int>();

		private TimeSpan? _elapsed;

		private bool? _multithreaded;

		public DatasetsValidator(VotingResults votingResults,EncryptedVoteFileReader.EncryptedVoteRecord[] encryptedVotes,ElGamalDecryptor decryptor)
		{
			_votingResults = votingResults;
			_encryptedVotes = encryptedVotes;
			_elGamalDecryptor = decryptor;
		}

		public int UniqueAbCount
		{
			get { return _abCounter.Count; }
		}

		public ReadOnlyCollection<int> InvalidVotes
		{
			get { return _invalidVotes.AsReadOnly(); }
		}

		public TimeSpan ElapsedTime
		{
			get { return _elapsed.Value; }
		}

		public bool Multithreaded
		{
			get { return _multithreaded.Value; }
		}

		private void CheckNotStarted()
		{
			var started = Interlocked.Exchange(ref _validationStarted, 1);
			if (started != 0)
			{
				throw new InvalidOperationException();
			}
		}

		public Task ValidateParallelAsync()
		{
			CheckNotStarted();
			return Task.Factory.StartNew(() => ValidateParallel(), TaskCreationOptions.LongRunning);
		}

		private void ValidateParallel()
		{
			var stopwatch = Stopwatch.StartNew();
			_multithreaded = true;
			Parallel.For(0, _encryptedVotes.Length, (index) =>
			{
				try
				{
					ValidateVote(index, _votingResults.Votes[index], _encryptedVotes[index]);
				}
				catch(Exception e)
				{
					lock (_invalidVotes)
					{
						_invalidVotes.Add(index);
					}
				}	
			});
			_elapsed = stopwatch.Elapsed;
		}

		public Task ValidateSeqAsync()
		{
			CheckNotStarted();
			return Task.Factory.StartNew(() => ValidateSeq(), TaskCreationOptions.LongRunning);
		}

		private void ValidateSeq()
		{
			_multithreaded = false;
			var stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < _votingResults.Votes.Count; i++)
			{
				try
				{
					ValidateVote(i, _votingResults.Votes[i], _encryptedVotes[i]);
				}
				catch(Exception e)
				{
					lock (_invalidVotes)
					{
						_invalidVotes.Add(i);
					}
				}
			}
			_elapsed = stopwatch.Elapsed;
		}


		private void ValidateVote(int index, Vote decryptedVote, EncryptedVoteFileReader.EncryptedVoteRecord encryptedVote)
		{
			var aHex = encryptedVote.EncryptedVoteObject.EncryptedA.Substring(2);
			var bHex = encryptedVote.EncryptedVoteObject.EncryptedB.Substring(2);
			lock (_abCounter)
			{
				_abCounter.Add(aHex + bHex);
			}
			byte[] a = Hex.Decode(aHex);
			byte[] b = Hex.Decode(bHex);
			var aa = new BigInteger(1, a);
			var bb = new BigInteger(1, b);

			var number = _elGamalDecryptor.Decrypt(aa, bb);

			bool isInvalid = (number.LongValue != decryptedVote.CandidateId)
				|| (decryptedVote.Time != encryptedVote.Time)
				|| (decryptedVote.VoteNumber != encryptedVote.VoteNumber)
				|| (decryptedVote.BlockNumber != int.Parse(encryptedVote.BlockNumber.AsSpan().Slice(1)));

			if (isInvalid)
			{
				lock (_invalidVotes)
				{
					_invalidVotes.Add(index);
				}
			}
		}
	}
}
