using System;
using System.Collections.Generic;
using System.Linq;

using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

using Voting2019.Core;

namespace Voting2019.Visualization
{
	public static class VisualData
	{
		public static VisualData<int> CreateBlockBasedVoteVisualData(VotingResults votingResults, Func<Vote, bool> filter)
		{
			var votes = new Dictionary<long, Dictionary<int, int>>();

			int minBlock = votingResults.MinBlockNumber;
			int maxBlock = votingResults.MaxBlockNumber;

			var candidateNames = new Dictionary<long, string>();
			foreach (var vote in votingResults.Votes)
			{
				if (!filter(vote))
				{
					continue;
				}

				Dictionary<int, int> candidateVotesTimeDistribution = null;
				if (!votes.TryGetValue(vote.CandidateId, out candidateVotesTimeDistribution))
				{
					candidateVotesTimeDistribution = new Dictionary<int, int>();
					votes.Add(vote.CandidateId, candidateVotesTimeDistribution);
				}

				if (!candidateNames.ContainsKey(vote.CandidateId))
				{
					candidateNames.Add(vote.CandidateId, votingResults.Candidates[vote.CandidateId].Name);
				}

				if (!candidateVotesTimeDistribution.TryGetValue(vote.BlockNumber, out int value))
				{
					candidateVotesTimeDistribution.Add(vote.BlockNumber, 1);
				}
				else
				{
					candidateVotesTimeDistribution[vote.BlockNumber] = value + 1;
				}
			}

			VisualData<int> ret = new VisualData<int>()
			{
				XMin = minBlock,
				XMax = maxBlock,
				CandidateNames = candidateNames,
				VotesDistribution = votes
			};
			return ret;
		}
	}

	public sealed class VisualData<TXAxisValue>
	{
		public TXAxisValue XMin;
		public TXAxisValue XMax;

		public Dictionary<long, string> CandidateNames;

		public Dictionary<long, Dictionary<TXAxisValue, int>> VotesDistribution;
	}

	public static class VisualDataExtension
	{
		private static void PrepareAxis(PlotModel plotModel,VisualData<int> data)
		{
			plotModel.Series.Clear();
			var xAxis = plotModel.Axes.Where(x => x.Position == AxisPosition.Bottom).Single();
			xAxis.SetAxisMinMax(data.XMin, data.XMax);
			var removeAxis = plotModel.Axes.Where(x => x.Position == AxisPosition.Left).ToArray();
			foreach (var axis in removeAxis)
			{
				plotModel.Axes.Remove(axis);
			}
		}

		private static void AddSeriesToPlotModel(PlotModel plotModel,VisualData<int> data,bool addKeys)
		{
			foreach (var candidatePair in data.VotesDistribution)
			{
				var series = new LineSeries()
				{
					Title = data.CandidateNames[candidatePair.Key],
					CanTrackerInterpolatePoints = false
				};

				Axis yAxis = null;
				if (addKeys)
				{
					var axisKey= "y_axis" + candidatePair.Key;
					series.YAxisKey = axisKey;
					yAxis = plotModel.Axes.Where(x => x.Key == axisKey).Single();
				}
				else
				{
					yAxis=plotModel.Axes.Where(x => x.Position == AxisPosition.Left).Single();
				}
				int yMax = 0;
				for (int block = data.XMin; block <= data.XMax; block++)
				{
					if (candidatePair.Value.TryGetValue(block, out int voteCount))
					{
						series.Points.Add(new DataPoint(block, voteCount));
					}
					else
					{
						series.Points.Add(new DataPoint(block, 0));
					}
					if (voteCount > yMax)
					{
						yMax = voteCount;
					}
				}
				yAxis.Maximum = yMax;
				yAxis.AbsoluteMaximum = yMax;
				plotModel.Series.Add(series);
			}
		}

		public static void LoadToPlotModel(this VisualData<int> @this, PlotModel plotModel)
		{
			lock (plotModel.SyncRoot)
			{
				PrepareAxis(plotModel, @this);

				var yAxis = new LinearAxis()
				{
					Position = AxisPosition.Left,
					AbsoluteMinimum = 0,
					Minimum = 0
				};
				plotModel.Axes.Add(yAxis);

				AddSeriesToPlotModel(plotModel, @this, false);
			}
			plotModel.InvalidatePlot(true);
		}

		public static void LoadToPlotModelMultiAxis(this VisualData<int> @this,PlotModel plotModel)
		{
			lock (plotModel.SyncRoot)
			{
				PrepareAxis(plotModel, @this);

				const double GapSize = 0.05;
				int AxisCount = @this.VotesDistribution.Count;
				double size = (1 - GapSize * (AxisCount - 1)) / AxisCount;
				double start = 0;

				int i = 0;
				foreach(var pair in @this.CandidateNames)
				{
					var key = "y_axis" + pair.Key;
					var yAxis = new LinearAxis()
					{
						AbsoluteMinimum=0,
						Minimum=0,
						StartPosition = start,
						EndPosition = start + size,
						Key = key,
						Position = AxisPosition.Left,
						Title = pair.Value,
						ToolTip = pair.Value,
						MajorGridlineColor = OxyColors.DarkGray,
						MinorGridlineColor = OxyColors.Gray,
						MajorGridlineStyle = LineStyle.Solid,
						MinorGridlineStyle = LineStyle.Dot,
					};
					plotModel.Axes.Add(yAxis);
					start += size + GapSize;
					i++;
				}

				AddSeriesToPlotModel(plotModel, @this, true);
			}
			plotModel.InvalidatePlot(true);
		}
	}
}
