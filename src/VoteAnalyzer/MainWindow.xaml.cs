using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities.Encoders;
using Voting2019.Core;
using Voting2019.Visualization;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Collections.Generic;

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

		private VotingResults _votingResults;

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
			
			_votingResults = reader.ReadFromFile("ballots_decrypted_2019-09-08.csv");

			

			// анализ времен вычисления блоков по блокчейну
			_blockTimePlotModelHelper.ShowBlockTime(_votingResults);
			_blockStartTimeModelHelper.ShowBlockStartTime(_votingResults);

			// распределение всех голосов
			_totalVotes.Show(_votingResults);

			_plotTotalVotesByTime.Show(_votingResults);
			
			var districts = _votingResults.Votes.Select(x => x.DistrictNumber).Distinct().ToArray();
			// 1 район
			_plotModelHelperDistrict1.ShowByBlockDistributionMultiAxis(_votingResults, x => x.DistrictNumber == 1);
			_plotTotalVotesByTimeDistrict1.ShowByCandidates(_votingResults, x => x.DistrictNumber == 1);
			// 10 район	
			_plotModelHelperDistrict10.ShowByBlockDistributionMultiAxis(_votingResults, x => x.DistrictNumber == 10);
			_plotTotalVotesByTimeDistrict10.ShowByCandidates(_votingResults, x => x.DistrictNumber == 10);
			// 30 район
			_plotModelHelperDistrict30.ShowByBlockDistributionMultiAxis(_votingResults, x => x.DistrictNumber == 30);
			_plotTotalVotesByTimeDistrict30.ShowByCandidates(_votingResults, x => x.DistrictNumber == 30);



			// общая статистика
			stats.SelectedObject = _votingResults.Statistics;
		}

		private async void ValidateSeqButtonClick(object sender, RoutedEventArgs args)
		{
			busy.IsBusy = true;
			try
			{
				(var encryptedVotes, var elGamal) = GetDataForValidation();
				DatasetsValidator validator = new DatasetsValidator(_votingResults, encryptedVotes, elGamal);
				await validator.ValidateSeqAsync();
				DisplayValidationResults(validator);
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
			}

			busy.IsBusy = false;
		}
		private async void ValidateParallelButtonClick(object sender, RoutedEventArgs args)
		{
			busy.IsBusy = true;
			try
			{
				(var encryptedVotes, var elGamal) = GetDataForValidation();
				DatasetsValidator validator = new DatasetsValidator(_votingResults, encryptedVotes, elGamal);
				await validator.ValidateParallelAsync();
				DisplayValidationResults(validator);
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
			}

			busy.IsBusy = false;
		}

		private (EncryptedVoteFileReader.EncryptedVoteRecord[] encryptedVotes, ElGamalDecryptor decryptor) GetDataForValidation()
		{
			var modulo = "165369015881747654263169334048437027341589578831388076203669679751228103730581853004239937620390662340697170254714963999712574103938632344301147612469870940462347496583243202336840686434045187170457457390530900726932209309694946086372519269610850329270398550413793758739702497764674603008447480576049325649379";
			BigInteger p = new BigInteger(modulo, 10);
			var generator = "108983589125857988887799079374111523867903227291665711778924429892396584160937414307590973620804320110833766711915181183354451775894143782144562429539439662246154120627476661161620394032143652279821376660116687871113836507709308914754296752851754595271290675931295353918333037008650334703641521924401778561481";
			BigInteger g = new BigInteger(generator, 10);
			var publicKey = "95037962141997278598444295323416959687203828673622505601323330341875332219187996223109547161059313685845388548002721668698176360799317237039717937000841182698936793113519146616557801775110314932631443735725757529075168762329138239011228280320957777931011352874682630080877486558428265880823122590580770387505";
			var privateKey = "121059313385762190379394539131175223970663702958273675041364938406427795349299390592172678615857213838053328843754657568685431221069308928897897579696879704086254410696320593023100923661192940619744327349942556208570556929626169358881256636833104492234991097614322051649000547092241681421625589582664828742232";

			BigInteger pubKey = new BigInteger(publicKey, 10);
			BigInteger privKey = new BigInteger(privateKey, 10);

			var elGamal = new ElGamalDecryptor(p, privKey);
			EncryptedVoteFileReader readerEn = new EncryptedVoteFileReader();
			var encryptedVotes = readerEn.ReadRecordFromFile("ballots_encrypted_2019-09-08T20_30_00.csv").ToArray();

			if (encryptedVotes.Length != _votingResults.Votes.Count)
			{
				MessageBox.Show("Encrypted and decrypted votes count mismatch");
				return (null, null);
			}

			return (encryptedVotes, elGamal);
		}


		private void DisplayValidationResults(DatasetsValidator validator)
		{
			if (validator.UniqueAbCount != _votingResults.Votes.Count)
			{
				MessageBox.Show("Duplicate (a,b) pairs detected");
				return;
			}

			if (validator.InvalidVotes.Count > 0)
			{
				MessageBox.Show(string.Join(',', validator.InvalidVotes), "Invalid votes detected");
				return;
			}

			MessageBox.Show(string.Format("Validation passed. Time - {0} (MT={1})", validator.ElapsedTime, validator.Multithreaded));
		}
	}
}
