using System.Diagnostics;

namespace Voting2019.Core
{
	[DebuggerDisplay("{" + nameof(Id) + "} - {" + nameof(Name) + "}")]
	public sealed class Candidate
	{
		private readonly long _id;
		private readonly long _district;
		private readonly string _name;

		public Candidate(long id, long district, string name)
		{
			_id = id;
			_district = district;
			_name = name;
		}

		public long Id
		{
			get { return _id; }
		}

		public long District
		{
			get { return _district; }
		}

		public string Name
		{
			get { return _name; }
		}
	}
}
