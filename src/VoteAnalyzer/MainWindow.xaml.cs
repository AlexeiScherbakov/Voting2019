using System.Linq;
using System.Windows;

using Voting2019.Core;
using Voting2019.Visualization;

namespace VoteAnalyzer
{
	/// <summary>
	/// Main Window
	/// </summary>
	public partial class MainWindow
		: Window
	{

		private readonly BlockTimePlotModelHelper _blockTimePlotModelHelper = new BlockTimePlotModelHelper();
		private readonly BlockTimePlotModelHelper _blockStartTimeModelHelper = new BlockTimePlotModelHelper();
		private readonly TotalVotesByBlockDistribution _totalVotes = new TotalVotesByBlockDistribution();
		private readonly VoteByTimeCandidateDistributionModelHelper _plotModelHelperDistrict1, _plotModelHelperDistrict10, _plotModelHelperDistrict30;
		private readonly TotalCumulativeVotedByTime _plotTotalVotesByTime = new TotalCumulativeVotedByTime();
		private readonly TotalCumulativeVotedByTime _plotTotalVotesByTimeDistrict1, _plotTotalVotesByTimeDistrict10, _plotTotalVotesByTimeDistrict30;


		public MainWindow()
		{
			InitializeComponent();

			_plotModelHelperDistrict1 = new VoteByTimeCandidateDistributionModelHelper();
			_plotModelHelperDistrict10 = new VoteByTimeCandidateDistributionModelHelper();
			_plotModelHelperDistrict30 = new VoteByTimeCandidateDistributionModelHelper();

			_plotTotalVotesByTimeDistrict1 = new TotalCumulativeVotedByTime();
			_plotTotalVotesByTimeDistrict10 = new TotalCumulativeVotedByTime();
			_plotTotalVotesByTimeDistrict30 = new TotalCumulativeVotedByTime();

			this.plotBlockTime.Model = _blockTimePlotModelHelper.PlotModel;
			this.plotBlockStartTime.Model = _blockStartTimeModelHelper.PlotModel;
			this.plotTotalVotes.Model = _totalVotes.PlotModel;
			this.plotTotalVotesByTime.Model = _plotTotalVotesByTime.PlotModel;

			this.plotDistrict1.Model = _plotModelHelperDistrict1.PlotModel;
			this.plotDistrict10.Model = _plotModelHelperDistrict10.PlotModel;
			this.plotDistrict30.Model = _plotModelHelperDistrict30.PlotModel;

			this.plotDistrict1Cumulative.Model = _plotTotalVotesByTimeDistrict1.PlotModel;
			this.plotDistrict10Cumulative.Model = _plotTotalVotesByTimeDistrict10.PlotModel;
			this.plotDistrict30Cumulative.Model = _plotTotalVotesByTimeDistrict30.PlotModel;
			this.Loaded += MainWindow_Loaded;
		}

		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			UnencryptedVoteFileReader reader = new UnencryptedVoteFileReader();

			var votingResults = reader.ReadFromFile("ballots_decrypted_2019-09-08.csv");

			// анализ времен вычисления блоков по блокчейну
			_blockTimePlotModelHelper.ShowBlockTime(votingResults);
			_blockStartTimeModelHelper.ShowBlockStartTime(votingResults);

			// распределение всех голосов
			_totalVotes.Show(votingResults);

			_plotTotalVotesByTime.Show(votingResults);
			
			var districts = votingResults.Votes.Select(x => x.DistrictNumber).Distinct().ToArray();
			// 1 район
			_plotModelHelperDistrict1.ShowByBlockDistributionMultiAxis(votingResults, x => x.DistrictNumber == 1);
			_plotTotalVotesByTimeDistrict1.ShowByCandidates(votingResults, x => x.DistrictNumber == 1);
			// 10 район	
			_plotModelHelperDistrict10.ShowByBlockDistributionMultiAxis(votingResults, x => x.DistrictNumber == 10);
			_plotTotalVotesByTimeDistrict10.ShowByCandidates(votingResults, x => x.DistrictNumber == 10);
			// 30 район
			_plotModelHelperDistrict30.ShowByBlockDistributionMultiAxis(votingResults, x => x.DistrictNumber == 30);
			_plotTotalVotesByTimeDistrict30.ShowByCandidates(votingResults, x => x.DistrictNumber == 30);



			// общая статистика
			stats.SelectedObject = votingResults.Statistics;
		}
	}
}
