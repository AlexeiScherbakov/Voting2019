using System;

namespace Voting2019.Core
{
	public sealed class StatisticsItem
	{
		public readonly TimeSpan Time;
		public readonly int ComeToRegistrationPage;
		public readonly int ValidatedBySms;
		public readonly int Registered;
		public readonly int Voted;

		public StatisticsItem(TimeSpan time, int comeToRegistrationPage, int validatedBySms, int registered, int voted)
		{
			Time = time;
			ComeToRegistrationPage = comeToRegistrationPage;
			ValidatedBySms = validatedBySms;
			Registered = registered;
			Voted = voted;
		}
	}
}
