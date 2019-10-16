using System;

namespace Voting2019.Visualization
{
	public readonly struct TimeGraphItem<T>
		: IEquatable<TimeGraphItem<T>>
		where T : struct, IEquatable<T>
	{
		public readonly TimeSpan Time;
		public readonly T Data;

		public TimeGraphItem(TimeSpan time, T data)
		{
			Time = time;
			Data = data;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Time, Data);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj)) return true;
			if (ReferenceEquals(null, obj)) return false;
			if (obj is TimeGraphItem<T> other)
			{
				return (this.Time == other.Time) && this.Data.Equals(other.Data);
			}
			return false;
		}

		public bool Equals(TimeGraphItem<T> other)
		{
			return (this.Time == other.Time) && this.Data.Equals(other.Data);
		}
	}
}
