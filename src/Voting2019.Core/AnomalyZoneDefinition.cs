using System;

namespace Voting2019.Core
{
	public sealed class AnomalyZoneDefinition
	{
		private int _startBlock;
		private int _endBlock;
		private TimeSpan _startTime;
		private TimeSpan _endTime;
		private string _name;

		internal AnomalyZoneDefinition(string name,int startBlock,int endBlock,TimeSpan startTime,TimeSpan endTime)
		{
			_name = name;
			_startBlock = startBlock;
			_endBlock = endBlock;
			_startTime = startTime;
			_endTime = endTime;
		}
		public string Name
		{
			get { return _name; }
		}

		public int StartBlock
		{
			get { return _startBlock; }
		}

		public int EndBlock
		{
			get { return _endBlock; }
		}

		public TimeSpan StartTime
		{
			get { return _startTime; }
		}

		public TimeSpan EndTime
		{
			get { return _endTime; }
		}
	}
}
