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
		private readonly VoteByBlockCandidateDistributionModelHelper _plotModelHelperDistrict1, _plotModelHelperDistrict10, _plotModelHelperDistrict30;
		private readonly TotalCumulativeVotedByTime _plotTotalCumulativeVotesByTime = new TotalCumulativeVotedByTime();
		private readonly TotalCumulativeVotedByTime _totalCumulativeVotes30 = new TotalCumulativeVotedByTime();
		private readonly TotalCumulativeVotedByTime _plotTotalVotesByTimeDistrict1, _plotTotalVotesByTimeDistrict10, _plotTotalVotesByTimeDistrict30;
		private readonly StatisticsPlotModelHelper _statistics30PlotModelHelper = new StatisticsPlotModelHelper();

		private VotingResults _votingResults;

		private readonly List<IPlotModelDrawer> _plotModelDrawers = new List<IPlotModelDrawer>();

		public MainWindow()
		{
			InitializeComponent();
			_plotModelHelperDistrict1 = new VoteByBlockCandidateDistributionModelHelper();
			_plotModelHelperDistrict10 = new VoteByBlockCandidateDistributionModelHelper();
			_plotModelHelperDistrict30 = new VoteByBlockCandidateDistributionModelHelper();

			_plotTotalVotesByTimeDistrict1 = new TotalCumulativeVotedByTime();
			_plotTotalVotesByTimeDistrict10 = new TotalCumulativeVotedByTime();
			_plotTotalVotesByTimeDistrict30 = new TotalCumulativeVotedByTime();

			_blockTimePlotModelHelper.SetTitle("Время вычисления блока");
			_blockStartTimeModelHelper.SetTitle("Время добавления блока");
			_totalVotes.SetTitle("Полное распределение голосов по блокам");
			_plotTotalCumulativeVotesByTime.SetTitle("Общее число проголосовавших нарастающим итогом");

			_plotModelHelperDistrict1.SetTitle("Округ 1: Распределение голосов кандидатов по блокам");
			_plotModelHelperDistrict10.SetTitle("Округ 10: Распределение голосов кандидатов по блокам");
			_plotModelHelperDistrict30.SetTitle("Округ 30: Распределение голосов кандидатов по блокам");

			_plotTotalVotesByTimeDistrict1.SetTitle("Округ 1: Общее число проголосовавших по кандидатам нарастающим итогом");
			_plotTotalVotesByTimeDistrict10.SetTitle("Округ 10: Общее число проголосовавших по кандидатам нарастающим итогом");
			_plotTotalVotesByTimeDistrict30.SetTitle("Округ 30: Общее число проголосовавших по кандидатам нарастающим итогом");
			_totalCumulativeVotes30.SetTitle("Округ 30: Общее число проголосовавших нарастающим итогом");

			_statistics30PlotModelHelper.SetTitle("Данные с экранов по округу 30");

			this.plotBlockTime.Model = _blockTimePlotModelHelper.PlotModel;
			this.plotBlockStartTime.Model = _blockStartTimeModelHelper.PlotModel;
			this.plotTotalVotes.Model = _totalVotes.PlotModel;
			this.plotTotalVotesByTime.Model = _plotTotalCumulativeVotesByTime.PlotModel;

			this.plotDistrict1.Model = _plotModelHelperDistrict1.PlotModel;
			this.plotDistrict10.Model = _plotModelHelperDistrict10.PlotModel;
			this.plotDistrict30.Model = _plotModelHelperDistrict30.PlotModel;

			this.plotDistrict1Cumulative.Model = _plotTotalVotesByTimeDistrict1.PlotModel;
			this.plotDistrict10Cumulative.Model = _plotTotalVotesByTimeDistrict10.PlotModel;
			this.plotDistrict30Cumulative.Model = _plotTotalVotesByTimeDistrict30.PlotModel;

			plotDistrict30Total.Model = _totalCumulativeVotes30.PlotModel;

			statsFromPhoto30Plot.Model = _statistics30PlotModelHelper.PlotModel;

			_plotModelDrawers.AddRange(
				new IPlotModelDrawer[]
				{
								_blockTimePlotModelHelper,
								_blockStartTimeModelHelper,
								_totalVotes,
								_plotModelHelperDistrict1,
								_plotModelHelperDistrict10,
								_plotModelHelperDistrict30,
								_plotTotalCumulativeVotesByTime,
								_totalCumulativeVotes30,
								_plotTotalVotesByTimeDistrict1,
								_plotTotalVotesByTimeDistrict10,
								_plotTotalVotesByTimeDistrict30,
								_statistics30PlotModelHelper
				});

			this.Loaded += MainWindow_Loaded;
		}

		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			UnencryptedVoteFileReader reader = new UnencryptedVoteFileReader();

			_votingResults = reader.ReadFromFile("ballots_decrypted_2019-09-08.csv");

			// установленные в процессе анализа аномальные зоны
			_votingResults.DefineAnomalyZoneByBlocks("Зона 1А", 2046, 2525);
			_votingResults.DefineAnomalyZoneByBlocks("Зона 1Б", 2525, 2651);
			_votingResults.DefineAnomalyZoneByBlocks("Зона 2", 2818, 2956);
			_votingResults.DefineAnomalyZoneByBlocks("Зона 3", 3543, 4803);
			var statistics30 = GetStatistics();

			// анализ времен вычисления блоков по блокчейну
			_blockTimePlotModelHelper.ShowBlockTime(_votingResults);
			_blockStartTimeModelHelper.ShowBlockStartTime(_votingResults);

			// распределение всех голосов
			_totalVotes.Show(_votingResults);

			// распределение голосов в 30-ом
			_totalCumulativeVotes30.Show(_votingResults, x => x.DistrictNumber == 30);
			_totalCumulativeVotes30.AddCustomData("Проголосовало по статистике", statistics30.Statistics.Select(x => new TimeGraphItem<int>(x.Time, x.Voted)));

			_plotTotalCumulativeVotesByTime.Show(_votingResults, x => true);

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

			_statistics30PlotModelHelper.Show(statistics30);

			// общая статистика
			stats.SelectedObject = _votingResults.Statistics;

			foreach(var drawer in _plotModelDrawers)
			{
				drawer.DrawAnomalyZones(_votingResults.Anomalies);
			}
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


		private static RemoteVotingStatistics GetStatistics()
		{
			// эта статистика получена из фотографий
			RemoteVotingStatisticsBuilder builder = new RemoteVotingStatisticsBuilder();
			builder
				.AddStatisticsItem("20:00", 8581, 3077, 2525, 2376)
				.AddStatisticsItem("19:45", 8503, 3064, 2512, 2358)
				.AddStatisticsItem("19:30", 8394, 3035, 2495, 2325)
				.AddStatisticsItem("19:15", 8287, 2996, 2469, 2292)
				.AddStatisticsItem("19:00", 8199, 2973, 2451, 2275)
				.AddStatisticsItem("18:45", 8091, 2951, 2434, 2256)
				.AddStatisticsItem("18:30", 8005, 2932, 2418, 2235)
				.AddStatisticsItem("18:15", 7920, 2908, 2405, 2214)
				.AddStatisticsItem("18:00", 7839, 2885, 2386, 2184)
				.AddStatisticsItem("17:45", 7731, 2850, 2361, 2153)
				.AddStatisticsItem("17:30", 7622, 2817, 2340, 2127)
				.AddStatisticsItem("17:15", 7500, 2787, 2317, 2096)
				.AddStatisticsItem("17:00", 7388, 2755, 2293, 2069)
				.AddStatisticsItem("16:45", 7260, 2717, 2259, 2028)
				.AddStatisticsItem("16:30", 7161, 2683, 2230, 2000)
				.AddStatisticsItem("16:15", 5337, 2381, 2095, 1838)
				.AddStatisticsItem("16:00", 5337, 2262, 2095, 1838)
				.AddStatisticsItem("15:45", 5337, 2143, 2095, 1838)
				.AddStatisticsItem("15:30", 5337, 2024, 2095, 1838)
				.AddStatisticsItem("15:15", 5153, 1905, 2045, 1784)
				.AddStatisticsItem("15:01", 4464, 1786, 1992, 1731)
				.AddStatisticsItem("14:46", 4451, 1783, 1926, 1669)
				.AddStatisticsItem("14:31", 4451, 1783, 1846, 1582)
				.AddStatisticsItem("14:16", 4451, 1783, 1700, 1165)
				.AddStatisticsItem("14:01", 4451, 1783, 1548, 1138)
				.AddStatisticsItem("13:46", 4524, 1588, 1460, 1114)
				.AddStatisticsItem("13:31", 4436, 1495, 1393, 1085)
				.AddStatisticsItem("13:16", 4147, 1420, 1352, 1045)
				.AddStatisticsItem("13:00", 3866, 1321, 1295, 973)
				.AddStatisticsItem("12:45", 3245, 1274, 1213, 868)
				.AddStatisticsItem("12:30", 2887, 1178, 1103, 759)
				.AddStatisticsItem("12:15", 2787, 1104, 1079, 748)
				.AddStatisticsItem("12:00", 2787, 1104, 1079, 748)
				.AddStatisticsItem("11:45", 2787, 1104, 1079, 748)
				.AddStatisticsItem("11:30", 2787, 1104, 1079, 748)
				.AddStatisticsItem("11:15", 2675, 1090, 1045, 736)
				.AddStatisticsItem("11:00", 1898, 1007, 957, 710)
				.AddStatisticsItem("10:45", 1898, 1007, 957, 710)
				.AddStatisticsItem("10:30", 1898, 1007, 957, 710)
				.AddStatisticsItem("10:15", 1898, 1007, 957, 710)
				.AddStatisticsItem("10:00", 1898, 1007, 957, 710)
				.AddStatisticsItem("9:45", 1825, 831, 783, 710)
				.AddStatisticsItem("9:30", 1825, 831, 783, 710)
				.AddStatisticsItem("8:45", 858, 428, 406, 372)
				.AddStatisticsItem("8:30", 575, 289, 274, 236);
			return builder.Create();
		}
	}
}
