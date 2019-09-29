using System;
using System.Diagnostics;

namespace Voting2019.Core
{
	[DebuggerDisplay("{Min} - {Max}")]
	public struct MinMaxInterval<TValue>
		where TValue : struct, IComparable<TValue>
	{
		private TValue? _min;
		private TValue? _max;

		public void UpdateInterval(TValue value)
		{
			if (_min.HasValue)
			{
				if (value.CompareTo(_min.Value) < 0)
				{
					_min = value;
				}
			}
			else
			{
				_min = value;
			}

			if (_max.HasValue)
			{
				if (value.CompareTo(_max.Value) > 0)
				{
					_max = value;
				}
			}
			else
			{
				_max = value;
			}


		}

		public TValue Min
		{
			get { return _min.Value; }
		}

		public TValue Max
		{
			get { return _max.Value; }
		}
	}
}
