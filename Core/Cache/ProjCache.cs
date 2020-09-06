using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using Vitrium.Buffs;

namespace Vitrium.Core
{
	public class ProjCache : GlobalProjectile
	{
		public static VitriBuff buff;
		public override bool InstancePerEntity => false;
		public override bool CloneNewInstances => false;

		public override bool PreKill(Projectile projectile, int timeLeft)
		{
			buff = null;
			return base.PreKill(projectile, timeLeft);
		}

		public override void PostAI(Projectile projectile)
		{
			if (projectile.minion && buff == null)
			{
				try
				{
					var player = Main.player[projectile.owner];
					var item = player.inventory[player.selectedItem];

					if (item != null && item.IsValid())
					{
						var vi = VItem.GetData(item);
						buff = vi.buff;
					}
				}
				catch (Exception e)
				{
					Main.NewText(e.ToString(), Color.Red);
					Main.NewText("An error occurred when finding the buff in VProjectile.PreAI", Color.Red);
				}
			}

			if (buff != null)
			{
				try
				{
					VPlayer.GetData(Main.player[projectile.owner]).AddBuff(buff);
				}
				catch (Exception e)
				{
					Main.NewText(e.ToString(), Color.Red);
					Main.NewText("An error occurred when adding a buff in VProjectile.PreAI", Color.Red);
				}
			}

			base.PostAI(projectile);
		}
	}
}
