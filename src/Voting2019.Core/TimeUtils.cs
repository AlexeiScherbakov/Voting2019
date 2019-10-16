using System;
using System.Diagnostics.Contracts;

namespace Voting2019.Core
{
	internal static class TimeUtils
	{
		public static TimeSpan MapToNearestTime(this TimeSpan @this,TimeSpan start,TimeSpan end)
		{
			Contract.Requires(start <= @this);
			Contract.Requires(@this <= end);

			var delta1 = start - @this;
			var delta2 = @end - @this;

			return delta1 <= delta2 ? start : end;
		}
	}
}
