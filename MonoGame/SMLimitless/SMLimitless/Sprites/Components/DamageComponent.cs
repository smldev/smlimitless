using System;

namespace SMLimitless.Sprites.Components
{
	/// <summary>
	///   A component for a sprite that can damage other sprites.
	/// </summary>
	public sealed class DamageComponent : SpriteComponent
	{
		/// <summary>
		///   Performs damage to sprite with a <see cref="HealthComponent" /> instance.
		/// </summary>
		/// <param name="sprite">The sprite to damage.</param>
		/// <param name="damageType">The type of damage to perform.</param>
		/// <param name="hpAmount">How many hit points to damage for.</param>
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

		/// <summary>
		///   Updates this component.
		/// </summary>
		public override void Update()
		{
		}
	}
}
