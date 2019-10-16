using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

using Voting2019.Core;

namespace Voting2019.Visualization
{
	internal static class BlockTimeVisualHelper
	{
		public static TimeSpan LoadBlockTimesToSeries(VotingResults votingResults, LineSeries averageBlockTimeSeries, LineSeries blockStartTimeSeries)
		{
			var blocks = votingResults.Votes.Select(x => new BlockGraphItem<TimeSpan>(x.BlockNumber, x.Time))
				.Distinct()
				.OrderBy(x => x.BlockNumber)
				.ToArray();


			TimeSpan lastBlockStartTime = blocks[0].Data;
			int lastBlockNumber = blocks[0].BlockNumber;
			TimeSpan maxBlockTime = TimeSpan.Zero;

			for (int i = 1; i < blocks.Length; i++)
			{
				ref BlockGraphItem<TimeSpan> currentBlock = ref blocks[i];


				var blocksTime = currentBlock.Data - lastBlockStartTime;
				var numberOfBlocks = currentBlock.BlockNumber - lastBlockNumber;

				var averageBlockTime = blocksTime / numberOfBlocks;

				for (int point = lastBlockNumber; point < currentBlock.BlockNumber; point++)
				{
					if (blockStartTimeSeries != null)
						blockStartTimeSeries.Points.Add(new DataPoint(point, TimeSpanAxis.ToDouble(currentBlock.Data)));
					if (averageBlockTimeSeries != null)
						averageBlockTimeSeries.Points.Add(new DataPoint(point, TimeSpanAxis.ToDouble(averageBlockTime)));
				}

				lastBlockStartTime = currentBlock.Data;
				lastBlockNumber = currentBlock.BlockNumber;

				if (averageBlockTime > maxBlockTime)
				{
					maxBlockTime = averageBlockTime;
				}
			}

			return maxBlockTime;
		}

	}
}
