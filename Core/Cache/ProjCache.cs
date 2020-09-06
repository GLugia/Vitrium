using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using Vitrium.Buffs;

namespace Vitrium.Core
{
	public class ProjCache : GlobalProjectile
	{
		private VitriBuff buff;
		public override bool InstancePerEntity => true;
		public override bool CloneNewInstances => true;

		public override bool PreAI(Projectile projectile)
		{
			try
			{
				var player = Main.player[projectile.owner];

				if (buff == null)
				{
					var selected = player.inventory[player.selectedItem];
					buff = selected.GetItem().buff;
				}
				else
				{
					player.GetPlayer().AddBuff(buff);
				}
			}
			catch (Exception e)
			{
				Main.NewText(e.ToString(), Color.Red);
			}
			return base.PreAI(projectile);
		}

		public override GlobalProjectile Clone()
		{
			var clone = (ProjCache)base.Clone();
			clone.buff = buff;
			return clone;
		}
	}
}
