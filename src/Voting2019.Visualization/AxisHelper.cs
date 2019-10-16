using System;
using OxyPlot.Axes;

using Voting2019.Core;

namespace Voting2019.Visualization
{
	internal static class AxisHelper
	{
		public static void SetMinMaxBlocksToXAxis(this Axis axis, VotingResults votingResults)
		{
			axis.AbsoluteMinimum = axis.Minimum = votingResults.MinBlockNumber;
			axis.AbsoluteMaximum = axis.Maximum = votingResults.MaxBlockNumber;
		}

		public static void SetAxisMinMax(this Axis axis,double min,double max)
		{
			axis.AbsoluteMinimum = axis.Minimum = min;
			axis.AbsoluteMaximum = axis.Maximum = max;
		}

		public static void SetAxisMinMax(this TimeSpanAxis axis,TimeSpan min,TimeSpan max)
		{
			axis.AbsoluteMinimum = axis.Minimum = TimeSpanAxis.ToDouble(min);
			axis.AbsoluteMaximum = axis.Maximum = TimeSpanAxis.ToDouble(max);
		}

		public static void SetAxisMax(this Axis axis,double max)
		{
			axis.AbsoluteMaximum = axis.Maximum = max;
		}
	}
}
