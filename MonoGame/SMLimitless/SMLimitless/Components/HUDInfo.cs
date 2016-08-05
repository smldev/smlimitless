using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace SMLimitless.Components
{
	public sealed class HUDInfo
	{
		private const long MaximumScore = 999999999999999999L; // 1 quintillion minus one, the largest possible number of all nines a long can hold
		private const int MaximumTotalCoins = 999999999; // 1 billion minus one, the largest possible number of all nines an int can hold
		public readonly int CoinsPerWraparound;

		public int[] PlayerLives { get; private set; }
		public long Score { get; private set; }
		public int TotalCoins { get; private set; }
		public int Coins { get; private set; }

		public EventHandler<CoinCounterWrappedAroundEventArgs> CoinCounterWrappedAround;

		public HUDInfo(int coinsPerWraparound)
		{
			CoinsPerWraparound = coinsPerWraparound;

			// TODO: add support for multiple players
			PlayerLives = new int[1];
		}

		public void AddScore(int amount)
		{
			long sum = Score + amount;
			if (sum < 0) { Score = 0; }
			else if (sum > MaximumScore) { Score = MaximumScore; }
			else { Score = sum; }
		}

		public void AddScore(int amount, Vector2 effectPosition)
		{
			long oldScore = Score;

			AddScore(amount);

			// Create a score effect for however much score we added
			int scoreAdded = (int)(Score - oldScore);
			SpawnScoreEffect(scoreAdded, effectPosition);
		}

		public void AddCoins(int amount)
		{
			int newCoinAmount = Coins + amount;
			int wrapArounds = newCoinAmount / CoinsPerWraparound;
			int finalCoinAmount = newCoinAmount % CoinsPerWraparound;

			Coins = newCoinAmount;

			int newTotalCoinAmount = TotalCoins + amount;
			if (TotalCoins > MaximumTotalCoins) { TotalCoins = MaximumTotalCoins; }
			else { TotalCoins = newTotalCoinAmount; }
		}

		private void SpawnScoreEffect(int amount, Vector2 effectPosition)
		{
			// TODO: implement a score effect that shows up when the score is increased
		}

		private void OnCoinCounterWrappedAround(int wrapArounds)
		{
			if (CoinCounterWrappedAround != null)
			{
				CoinCounterWrappedAround(this, new CoinCounterWrappedAroundEventArgs(wrapArounds));
			}
		}
	}

	public sealed class CoinCounterWrappedAroundEventArgs : EventArgs
	{
		public int WrapAroundsOccurred { get; private set; }

		public CoinCounterWrappedAroundEventArgs(int wrapAroundsOccurred)
		{
			WrapAroundsOccurred = wrapAroundsOccurred;
		}
	}
}
