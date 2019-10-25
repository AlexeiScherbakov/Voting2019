using System;

namespace Voting2019.Core
{
	public sealed class StatisticsItem
	{
		private readonly TimeSpan _time;
		private readonly int _comeToRegistrationPage;
		private readonly int _validatedBySms;
		private readonly int _registered;
		private readonly int _voted;

		public StatisticsItem(TimeSpan time, int comeToRegistrationPage, int validatedBySms, int registered, int voted)
		{
			_time = time;
			_comeToRegistrationPage = comeToRegistrationPage;
			_validatedBySms = validatedBySms;
			_registered = registered;
			_voted = voted;
		}

		public TimeSpan Time
		{
			get { return _time; }
		}

		public int ComeToRegistrationPage
		{
			get { return _comeToRegistrationPage; }
		}

		public int ValidatedBySms
		{
			get { return _validatedBySms; }
		}

		public int Registered
		{
			get { return _registered; }
		}

		public int Voted
		{
			get { return _voted; }
		}
	}
}
