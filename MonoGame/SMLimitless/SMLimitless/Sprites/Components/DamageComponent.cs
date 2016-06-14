using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMLimitless.Sprites.Components
{
	/// <summary>
	/// A component for a sprite that can damage other sprites.
	/// </summary>
	public sealed class DamageComponent : SpriteComponent
	{
		public void PerformDamage(Sprite sprite, string damageType, int hpAmount)
		{
			if (sprite == null) { throw new ArgumentNullException(nameof(sprite), "The provided sprite was null."); }
			if (string.IsNullOrEmpty(damageType)) { throw new ArgumentNullException(nameof(damageType), "The provided damage type was null or empty."); }
			if (hpAmount <= 0) { throw new ArgumentOutOfRangeException(nameof(hpAmount), $"The provided HP amount of {hpAmount} is out of range; it must be positive."); }

			HealthComponent spriteHealthComponent = sprite.GetComponent<HealthComponent>();
			if (spriteHealthComponent != null)
			{
				spriteHealthComponent.Damage(hpAmount, damageType);
			}
		}

		public override void Update()
		{
		}
	}
}
