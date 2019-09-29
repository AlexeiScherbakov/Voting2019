using System;
using System.Linq;

using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

using Voting2019.Core;

namespace Voting2019.Visualization
{
	internal static class BlockTimeVisualHelper
	{
		private sealed class BlockStartTime
			: IEquatable<BlockStartTime>
		{
			public readonly int BlockNumber;
			public readonly TimeSpan Time;

			public BlockStartTime(int number, TimeSpan time)
			{
				BlockNumber = number;
				Time = time;
			}

			public bool Equals(BlockStartTime other)
			{
				return (BlockNumber == other.BlockNumber) && (Time == other.Time);
			}

			public override bool Equals(object obj)
			{
				if (obj is BlockStartTime bst)
				{
					return Equals(bst);
				}
				return false;
			}

			public override int GetHashCode()
			{
				return HashCode.Combine(BlockNumber, Time);
			}
		}

		public static void LoadBlockTimesToPlot(PlotModel plotModel, VotingResults votingResults,LineSeries averageBlockTimeSeries,LineSeries blockStartTimeSeries,string averageBlockTimeAxisKey,string blockStartAxisTimeKey)
		{
			var blocks = votingResults.Votes.Select(x => new BlockStartTime(x.BlockNumber, x.Time))
				.Distinct()
				.OrderBy(x => x.BlockNumber)
				.ToArray();


			TimeSpan lastBlockStartTime = blocks[0].Time;
			int lastBlockNumber = blocks[0].BlockNumber;
			TimeSpan maxBlockTime = TimeSpan.Zero;

			for (int i = 1; i < blocks.Length; i++)
			{
				ref BlockStartTime currentBlock = ref blocks[i];

				if (blockStartTimeSeries != null)
				{
					blockStartTimeSeries.Points.Add(new DataPoint(currentBlock.BlockNumber, TimeSpanAxis.ToDouble(currentBlock.Time)));
				}

				var blocksTime = currentBlock.Time - lastBlockStartTime;
				var numberOfBlocks = currentBlock.BlockNumber - lastBlockNumber;

				var averageBlockTime = blocksTime / numberOfBlocks;

				if (averageBlockTimeSeries != null)
				{
					for (int point = lastBlockNumber; point < currentBlock.BlockNumber; point++)
					{
						averageBlockTimeSeries.Points.Add(new DataPoint(point, TimeSpanAxis.ToDouble(averageBlockTime)));
					}
				}
				
				lastBlockStartTime = currentBlock.Time;
				lastBlockNumber = currentBlock.BlockNumber;

				if (averageBlockTime > maxBlockTime)
				{
					maxBlockTime = averageBlockTime;
				}
				if (averageBlockTimeAxisKey != null)
				{
					var yAxis = plotModel.Axes.Where(x => x.Key == averageBlockTimeAxisKey).Single();
					yAxis.Maximum = yAxis.AbsoluteMaximum = TimeSpanAxis.ToDouble(maxBlockTime);
				}
				
			}
		}

	}
}
