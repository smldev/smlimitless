﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace SMLimitless.Sprites.Components
{
	/// <summary>
	///   A component for a sprite that has health and can be damaged.
	/// </summary>
	public sealed class HealthComponent : SpriteComponent
	{
		/// <summary>
		///   The maximum number of hit points that this sprite can have.
		/// </summary>
		public readonly int MaximumHitPoints;

		/// <summary>
		///   The number of hit points that this sprite starts with.
		/// </summary>
		public readonly int StartingHitPoints;

		/// <summary>
		///   A collection of damage type names that this sprite is immune to.
		/// </summary>
		public List<string> ImmuneTo;

		/// <summary>
		///   Gets the current number of hit points this sprite has.
		/// </summary>
		public int HitPoints { get; private set; }

		/// <summary>
		///   An event that is raised when this sprite is damaged.
		/// </summary>
		public event EventHandler<SpriteDamagedEventArgs> SpriteDamage;

		/// <summary>
		///   An event that is raised when this sprite is healed.
		/// </summary>
		public event EventHandler<SpriteHealedEventArgs> SpriteHealed;
		/// <summary>
		///   An event that is raised when this sprite is killed (hit points
		///   become 0).
		/// </summary>
		public event EventHandler<SpriteDamagedEventArgs> SpriteKilled;

		/// <summary>
		///   Initializes a new instance of the <see cref="HealthComponent" /> class.
		/// </summary>
		/// <param name="maximumHP">
		///   The maximum number of hit points that this sprite can have.
		/// </param>
		/// <param name="startingHP">
		///   The number of hit points that this sprite should start with.
		/// </param>
		/// <param name="immuneTo">
		///   A collection of damage type names that this sprite is immune to.
		/// </param>
		public HealthComponent(int maximumHP, int startingHP, params string[] immuneTo)
		{
			MaximumHitPoints = maximumHP;
			StartingHitPoints = HitPoints = startingHP;
			ImmuneTo = immuneTo.ToList() ?? new List<string>();
		}

		/// <summary>
		///   Damages this sprite.
		/// </summary>
		/// <param name="hpAmount">
		///   The amount of hit points to remove from the sprite.
		/// </param>
		/// <param name="damageType">The name of the damage type.</param>
		public void Damage(int hpAmount, string damageType)
		{
			if (string.IsNullOrEmpty(damageType)) { throw new ArgumentNullException("damageType", "The damage type cannot be null or empty."); }
			if (ImmuneTo.Contains(damageType) || hpAmount <= 0)
			{
				return;
			}

			if (hpAmount >= HitPoints)
			{
				HitPoints = 0;
				OnSpriteDeath(damageType);
			}
			else
			{
				HitPoints -= hpAmount;
				OnSpriteDamaged(damageType);
			}
		}

		/// <summary>
		///   Heals this sprite, restoring hit points.
		/// </summary>
		/// <param name="hpAmount"></param>
		public void Heal(int hpAmount)
		{
			if (hpAmount <= 0)
			{
				return;
			}
			else if (hpAmount + HitPoints > MaximumHitPoints)
			{
				hpAmount = MaximumHitPoints;
			}

			HitPoints += hpAmount;
			OnSpriteHealed();
		}

		/// <summary>
		///   Updates this component.
		/// </summary>
		public override void Update()
		{
		}

		private void OnSpriteDamaged(string damageType)
		{
            SpriteDamage?.Invoke(this, new SpriteDamagedEventArgs(damageType, HitPoints));
        }

		private void OnSpriteDeath(string damageType)
		{
            SpriteKilled?.Invoke(this, new SpriteDamagedEventArgs(damageType, 0));
        }

		private void OnSpriteHealed()
		{
            SpriteHealed?.Invoke(this, new SpriteHealedEventArgs(HitPoints));
        }
	}

    /// <summary>
    /// Contains event data for when a sprite is healed.
    /// </summary>
    public sealed class SpriteHealedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the number of remaining hit points on the healed sprite.
        /// </summary>
        public int RemainingHitPoints { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteHealedEventArgs"/> class. 
        /// </summary>
        /// <param name="remainingHitPoints">
        /// The number of remaining hit points on the healed sprite.
        /// </param>
        public SpriteHealedEventArgs(int remainingHitPoints)
        {
            RemainingHitPoints = remainingHitPoints;
        }
    }

	/// <summary>
	///   Contains event data for when a sprite or tile damages another sprite.
	/// </summary>
	public sealed class SpriteDamagedEventArgs : EventArgs
	{
		/// <summary>
		///   The type of damage.
		/// </summary>
		public string DamageType { get; private set; }

		/// <summary>
		///   The number of remaining hit points on the damaged sprite.
		/// </summary>
		public int RemainingHitPoints { get; private set; }

		/// <summary>
		///   Initializes a new instance of the <see
		///   cref="SpriteDamagedEventArgs" /> class.
		/// </summary>
		/// <param name="damageType">The type of damage.</param>
		/// <param name="remainingHitPoints">
		///   The number of remaining hit points on the damaged sprite.
		/// </param>
		public SpriteDamagedEventArgs(string damageType, int remainingHitPoints)
		{
			DamageType = damageType;
			RemainingHitPoints = remainingHitPoints;
		}
	}
}
