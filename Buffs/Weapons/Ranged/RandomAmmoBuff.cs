using System;
using System.Collections.Generic;
using System.Reflection;
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

		public override bool ConsumeAmmo(VPlayer player, Item weapon, Item ammo)
		{
			return base.ConsumeAmmo(player, weapon, ammo);
		}

		public override void OnConsumeAmmo(VPlayer player, Item weapon, Item ammo)
		{
			// make it so the player chooses which ammo types to pick from by scanning their inventory and adding ammo items to a list
			// probably do this in Shoot
			Type[] types = Main.instance.GetType().Assembly.GetTypes();
			FieldInfo[] projectiles = null;

			for (int i = 0; i < types.Length; i++)
			{
				if (types[i].Name.IsSimilarTo("projectileid"))
				{
					projectiles = types[i].GetFields();
					break;
				}
			}

			if (projectiles == null)
			{
				return;
			}

			List<FieldInfo> fields = new List<FieldInfo>();

			for (int i = 0; i < projectiles.Length; i++)
			{
				FieldInfo projInfo = projectiles[i];
				bool Contains(string val)
				{
					return projInfo.Name.Contains(val);
				}

				if (ammo.ammo == AmmoID.Arrow
					&& Contains("Arrow")
					&& !Contains("Hostile")
					&& !projInfo.Name.IsSimilarTo("FlamingArrow"))
				{
					fields.Add(projInfo);
				}
				else if (ammo.ammo == AmmoID.Bullet
					&& Contains("Bullet"))
				{
					fields.Add(projInfo);
				}
				else if (ammo.ammo == AmmoID.Rocket
					&& Contains("Rocket"))
				{
					fields.Add(projInfo);
				}
			}

			FieldInfo selected = fields[Main.rand.Next(0, fields.Count)];
			ammo.shoot = (int)selected.GetValue(Main.instance);
		}
	}
}
