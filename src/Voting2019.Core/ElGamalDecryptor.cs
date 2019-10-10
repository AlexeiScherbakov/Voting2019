using Org.BouncyCastle.Math;

namespace Voting2019.Core
{
	public sealed class ElGamalDecryptor
	{
		private static readonly BigInteger Four = new BigInteger("4", 10);

		private readonly BigInteger _module;
		private readonly BigInteger _privateKey;

		public ElGamalDecryptor(BigInteger module, BigInteger privateKey)
		{
			_module = module;
			_privateKey = privateKey;
		}


		public BigInteger Decrypt(BigInteger a, BigInteger b)
		{
			var c = a.ModInverse(_module).ModPow(_privateKey, _module);
			var squared = b.Multiply(c).Remainder(_module);
			//var criterion = squared.ModPow(_module.Subtract(BigInteger.One).Divide(BigInteger.Two), _module);
			var positive = squared.ModPow(_module.Add(BigInteger.One).Divide(Four), _module);
			var negative = _module.Subtract(positive);

			if (negative.CompareTo(positive) > 0)
			{
				return positive;
			}
			else
			{
				return negative;
			}
		}
	}
}
