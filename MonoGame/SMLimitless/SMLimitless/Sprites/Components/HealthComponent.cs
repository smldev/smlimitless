using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMLimitless.Sprites.Components
{
	public sealed class HealthComponent : SpriteComponent
	{
		public readonly int StartingHitPoints;
		public readonly string[] ImmuneTo;

		public int HitPoints { get; private set; }
		public event EventHandler<int> SpriteDamage;
		public event EventHandler<int> SpriteDeath;

		public HealthComponent(int startingHP, params string[] immuneTo)
		{
			StartingHitPoints = HitPoints = startingHP;
			ImmuneTo = immuneTo ?? new string[0];
		}

		public override void Update()
		{
		}

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
				OnSpriteDeath();
			}
			else
			{
				HitPoints -= hpAmount;
				OnSpriteDamaged();
			}
		}

		private void OnSpriteDamaged()
		{
			if (SpriteDamage != null)
			{
				SpriteDamage(this, HitPoints);
			}
		}

		private void OnSpriteDeath()
		{
			if (SpriteDeath != null)
			{
				SpriteDeath(this, 0);
			}
		}
	}
}
