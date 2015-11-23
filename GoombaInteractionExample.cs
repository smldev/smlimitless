namespace SMLimitless.Sprites.Components
{
	public sealed class DamageComponent : SpriteComponent
	{
		private Sprite owner;
		private Dictionary<string, Action<Sprite>> killActions = new Dictionary<string, Action<Sprite>>();
		
		public int HitPoints { get; private set; }
		public string[] ImmuneTo { get; }
		
		public DamageComponent(Sprite owner, int hitPoints, params string[] immuneTo)
		{
			// ...
			
			killActions.Add("stomp", s =>
				{
					s.Graphics.State = "flattened"; // or whatever
					s.AddTimer(seconds: 2.0f, action: sp => sp.Owner.RemoveSprite(sp));
				});
			
			killActions.Add("knock_off", s =>
			{
				s.Graphics.Orientation = GraphicsOrientation.UpsideDown;
				s.Velocity = GameServices.ISOStandardKnockOffVelocity;
				s.State = SpriteState.Inert;
			});
			
			killActions.Add("vaporize", s =>
			{
				s.Owner.RemoveSprite(s);
				s.Owner.CreateEffect("fireballPuff", s.Position);
				SoundManager.PlaySoundEffect("vaporize");
			});
			
			killActions.Add("default", s => s.Owner.RemoveSprite(s));
		}
		
		public void Damage(string damageType, string killAction, int damageAmount)
		{
			if (Array.Contains(immuneTo, damageType)) return;
			HitPoints -= damageAmount;
			
			if (damageAmount < 0)
			{
				killActionKey = (killActions.ContainsKey(killAction)) ? killAction : "default";
				killActions[killActionKey](owner);
			}
		}
		
		public void Kill(string damageType, string killAction)
		{
			HitPoints = 1; 	// guard against overflow lol
			Damage(damageType, killAction, HitPoints + 1);
		}
	}
}

namespace SmlSprites.SMB.Enemies
{
	public sealed class Goomba : Sprite
	{
		// ...imagine core sprite methods here...
		
		public Goomba(Section owner)
		{
			// ...
			Components.Add(new DamageComponent(1, new string[] {}));
			Components.Add(new WalkerComponent(turnOnCliffs: false));
		}
		
		// ...
	}
}

namespace SmlSprites.Default.Tiles
{
	public sealed class LavaTile : Tile
	{
		// this will probably be something like FatalTile at some point
		// ...imagine core tile methods here...
		
		public void OnSpriteCollision(Sprite sprite, Vector2 intersectDepth)
		{
			DamageComponent dmg = sprite.GetComponent<DamageComponent>();
			if (dmg != null) { dmg.Kill("lava", "vaporize"); }
		}
	}
}