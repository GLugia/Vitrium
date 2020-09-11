using Terraria;
using Terraria.ID;
using Vitrium.Core;

namespace Vitrium.Buffs.Weapons
{
	public class ExplosivoBuff : WeaponBuff
	{
		public override string Name => "Explosivo";
		public override string Tooltip => "Wait for it... BOOM!";
		public override string Texture => $"Terraria/buff_{BuffID.Rage}";

		public override void ModifyHitNPC(VPlayer player, NPC target, Item item, ref int damage, ref float knockback, ref bool crit)
		{
			TryAttach(player.player, target, damage, crit);
		}

		public override void ModifyHitNPCWithProj(VPlayer player, NPC target, Projectile proj, ref int damage, ref float knockback, ref bool crit, ref int direction)
		{
			if (proj.type != ProjectileID.StickyGrenade)
			{
				TryAttach(player.player, target, damage, crit);
			}
		}

		public override void ModifyHitPvp(VPlayer player, Player target, Item item, ref int damage, ref bool crit)
		{
			TryAttach(player.player, target, damage, crit);
		}

		public override void ModifyHitPvpWithProj(VPlayer player, Player target, Projectile proj, ref int damage, ref bool crit)
		{
			if (proj.type != ProjectileID.StickyGrenade)
			{
				TryAttach(player.player, target, damage, crit);
			}
		}

		private void TryAttach(Player player, Entity target, int damage, bool crit)
		{
			damage *= crit ? 2 : 1;
			// @TODO create a new projectile that follows the target, doesn't proc on-hit effects, and doesn't destroy blocks
		}
	}
}
