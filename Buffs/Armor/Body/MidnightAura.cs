using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Vitrium.Core;

namespace Vitrium.Buffs.Armor.Body
{
	public class MidnightAura : BodyBuff
	{
		public override string Name => "Midnight";
		public override string Tooltip => "";
		public override string Texture => $"Terraria/buff_{BuffID.Darkness}";
		public override float Weight => 1f;

		public override void PostUpdate(VPlayer player)
		{
			player.player.aggro -= 50;

			for (int i = 0; i < Main.npc.Length; i++)
			{
				if (i < 255)
				{
					Player a = Main.player[i];
					if (a.active && !a.dead && !a.ghost && a.team != player.player.team && a.whoAmI != player.player.whoAmI && Vector2.Distance(player.player.Center, a.Center) <= Main.spawnTileY / 1.5)
					{
						a.AddBuff(BuffID.Darkness, 2);
					}
				}

				NPC npc = Main.npc[i];
				if (npc.active && npc.damage > 0 && !npc.friendly && !npc.townNPC && npc.type != NPCID.TargetDummy && Vector2.Distance(player.player.Center, npc.Center) <= Main.spawnTileY / 1.5)
				{
					npc.buffImmune[BuffID.Darkness] = false;
					npc.AddBuff(BuffID.Darkness, 2);
				}
			}
		}
	}
}
