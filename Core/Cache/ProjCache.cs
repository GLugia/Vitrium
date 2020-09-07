﻿using Terraria;
using Terraria.ModLoader;
using Vitrium.Buffs;

namespace Vitrium.Core.Cache
{
	public class ProjCache : GlobalProjectile
	{
		public static ProjCache GetData(Projectile proj)
		{
			ProjCache ret = proj.GetGlobalProjectile<ProjCache>();
			ret.projectile = proj;
			return ret;
		}

		private VitriBuff buff;
		public Projectile projectile { get; private set; }

		public override bool InstancePerEntity => true;
		public override bool CloneNewInstances => true;

		public override void PostAI(Projectile projectile)
		{
			if (projectile.minion && buff == null)
			{
				buff = VItem.GetData(Main.player[projectile.owner].inventory[Main.player[projectile.owner].selectedItem]).buff;
			}
		}

		internal void ApplyBuffs()
		{
			VPlayer.GetData(Main.player[projectile.owner]).AddBuff(buff);
		}
	}
}
