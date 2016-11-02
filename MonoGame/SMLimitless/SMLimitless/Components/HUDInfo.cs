using System;
using Microsoft.Xna.Framework;

namespace SMLimitless.Components
{
	/// <summary>
	///   Arguments for the <see cref="HUDInfo.CoinCounterWrappedAround" /> event.
	/// </summary>
	public sealed class CoinCounterWrappedAroundEventArgs : EventArgs
	{
		/// <summary>
		///   Gets the number of times a wraparound occured.
		/// </summary>
		/// <example>
		///   If the coin counter displays 99, and the player earns 300 coins,
		///   the number of wraparounds is 3.
		/// </example>
		public int WrapAroundsOccurred { get; private set; }

		/// <summary>
		///   Initializes a new instance of the <see
		///   cref="CoinCounterWrappedAroundEventArgs" /> class.
		/// </summary>
		/// <param name="wrapAroundsOccurred">
		///   The number of times a wraparound occured.
		/// </param>
		public CoinCounterWrappedAroundEventArgs(int wrapAroundsOccurred)
		{
			WrapAroundsOccurred = wrapAroundsOccurred;
		}
	}

	/// <summary>
	///   Game information to be displayed onscreen.
	/// </summary>
	public sealed class HUDInfo
	{
		/// <summary>
		///   The number of coins needed to wrap the coin counter around,
		///   assuming the coin counter starts at 0.
		/// </summary>
		public readonly int CoinsPerWraparound;

		/// <summary>
		///   An event fired when the coin counter wraps around.
		/// </summary>
		public EventHandler<CoinCounterWrappedAroundEventArgs> CoinCounterWrappedAround;

		private const long MaximumScore = 999999999999999999L; // 1 quintillion minus one, the largest possible number of all nines a long can hold
		private const int MaximumTotalCoins = 999999999; // 1 billion minus one, the largest possible number of all nines an int can hold

		/// <summary>
		///   Gets the number of coins to display. Subject to wraparound.
		/// </summary>
		public int Coins { get; private set; }

		/// <summary>
		///   Gets an array containing the number of lives each player has.
		/// </summary>
		/// <remarks>
		///   If a player has 0 lives, a game over has occured. Life #1 is always
		///   the last.
		/// </remarks>
		public int[] PlayerLives { get; private set; }

		/// <summary>
		///   Gets the current score.
		/// </summary>
		public long Score { get; private set; }

		/// <summary>
		///   Gets the total number of coins collected.
		/// </summary>
		public int TotalCoins { get; private set; }

		/// <summary>
		///   Initializes a new instance of the <see cref="HUDInfo" /> class.
		/// </summary>
		/// <param name="coinsPerWraparound">
		///   The number of coins needed to wrap the coin counter around.
		/// </param>
		public HUDInfo(int coinsPerWraparound)
		{
			CoinsPerWraparound = coinsPerWraparound;

			// TODO: add support for multiple players
			PlayerLives = new int[1];
		}

		/// <summary>
		///   Adds coins to the coin counter and the total coin counter.
		/// </summary>
		/// <param name="amount">The number of coins to add.</param>
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

		/// <summary>
		///   Adds points to the score.
		/// </summary>
		/// <param name="amount">The number of points to add.</param>
		public void AddScore(int amount)
		{
			long sum = Score + amount;
			if (sum < 0) { Score = 0; }
			else if (sum > MaximumScore) { Score = MaximumScore; }
			else { Score = sum; }
		}

		/// <summary>
		///   Adds points to the score, creating a small graphic displaying the
		///   score onscreen.
		/// </summary>
		/// <param name="amount">The number of points to add.</param>
		/// <param name="effectPosition">
		///   The onscreen position to display the score graphic.
		/// </param>
		public void AddScore(int amount, Vector2 effectPosition)
		{
			long oldScore = Score;

			AddScore(amount);

			// Create a score effect for however much score we added
			int scoreAdded = (int)(Score - oldScore);
			SpawnScoreEffect(scoreAdded, effectPosition);
		}

		private void OnCoinCounterWrappedAround(int wrapArounds)
		{
			CoinCounterWrappedAround?.Invoke(this, new CoinCounterWrappedAroundEventArgs(wrapArounds));
		}

		private void SpawnScoreEffect(int amount, Vector2 effectPosition)
		{
			// TODO: implement a score effect that shows up when the score is increased
		}
	}
}
