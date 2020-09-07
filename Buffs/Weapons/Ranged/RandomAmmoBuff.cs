using System.Linq;
using Terraria;
using Terraria.ID;
using Vitrium.Core;

namespace Vitrium.Buffs.Weapons.Ranged
{
	// @TODO Change wooden arrows to random arrow types
	public class RandomAmmoBuff : RangedBuff
	{
		public override string Name => "Alternating Ammo";
		public override string Tooltip => "Are you sure you're using the right ammo?";
		public override string Texture => $"Terraria/buff_{BuffID.Archery}";

		public override void OnConsumeAmmo(VPlayer player, Item weapon, Item ammo)
		{
			System.Reflection.FieldInfo[] ids = Main.instance.GetType().Assembly.GetTypes().FirstOrDefault(a => a.Name.IsSimilarTo("projectileid")).GetFields();

			if (ammo.ammo == AmmoID.Arrow)
			{
				System.Reflection.FieldInfo[] arrows = ids.Where(a => a.Name.Contains("Arrow") && !a.Name.Contains("Hostile") && !a.Name.IsSimilarTo("FlamingArrow")).ToArray();
				System.Reflection.FieldInfo selected = arrows[Main.rand.Next(0, arrows.Length)];
				ammo.shoot = (short)selected.GetValue(Main.instance);

			}
			else if (ammo.ammo == AmmoID.Bullet)
			{
				System.Reflection.FieldInfo[] bullets = ids.Where(a => a.Name.Contains("Bullet")).ToArray();
				System.Reflection.FieldInfo selected = bullets[Main.rand.Next(0, bullets.Length)];
				ammo.shoot = (short)selected.GetValue(Main.instance);
			}
			else if (weapon.Name != "Rocket Launcher" && ammo.ammo == AmmoID.Rocket)
			{
				// @TODO probably need an offset for certain weapons for some reason
				System.Reflection.FieldInfo[] rockets = ids.Where(a => a.Name.Contains("Rocket")).ToArray();
				System.Reflection.FieldInfo selected = rockets[Main.rand.Next(0, rockets.Length)];
				ammo.shoot = (short)selected.GetValue(Main.instance);
			}
		}
	}
}
