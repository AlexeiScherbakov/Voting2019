using System;
using System.Collections.Generic;
using OxyPlot;
using OxyPlot.Axes;
using Voting2019.Core;

namespace Voting2019.Visualization
{
	public class VoteByTimeCandidateDistributionModelHelper
		: IPlotModelDrawer
	{
		private readonly PlotModel _plotModel;
		private readonly LinearAxis _xAxis;


		public VoteByTimeCandidateDistributionModelHelper()
		{
			_plotModel = new PlotModel();
			_xAxis = new LinearAxis()
			{
				Position = AxisPosition.Bottom,
				Key = "x_axis"
			};
			_plotModel.Axes.Add(_xAxis);
		}


		public PlotModel PlotModel
		{
			get { return _plotModel; }
		}


		public void ShowByBlockDistribution(VotingResults votingResults, Func<Vote, bool> filter)
		{
			var visualData = VisualData.CreateBlockBasedVoteVisualData(votingResults, filter);

			visualData.LoadToPlotModel(_plotModel);
		}

		public void ShowByBlockDistributionMultiAxis(VotingResults votingResults, Func<Vote, bool> filter)
		{
			var visualData = VisualData.CreateBlockBasedVoteVisualData(votingResults, filter);

			visualData.LoadToPlotModelMultiAxis(_plotModel);
		}
	}
}
