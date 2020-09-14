using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Vitrium.Core;

namespace Vitrium.Buffs.Armor.Body
{
	public class IceAura : BodyBuff
	{
		public override string Name => "Blizzard";
		public override string Tooltip => "You give off quite the cold shoulder";
		public override string Texture => $"Terraria/buff_{BuffID.Frozen}";
		public override float Weight => 1f;

		public override void PostUpdate(VPlayer player)
		{
			for (int i = 0; i < Main.npc.Length; i++)
			{
				if (i < 255)
				{
					Player a = Main.player[i];
					if (a.active && !a.dead && !a.ghost && (a.team == (int)Team.None || a.team != player.player.team) && a.whoAmI != player.player.whoAmI && Vector2.Distance(player.player.Center, a.Center) <= Main.spawnTileY / 1.5f)
					{
						if (Main.rand.NextFloat() <= 0.05f / 60f)
						{
							a.buffImmune[BuffID.Frozen] = false;
							a.AddBuff(BuffID.Frozen, 300);
						}
						else
						{
							a.buffImmune[BuffID.Chilled] = false;
							a.AddBuff(BuffID.Chilled, 2);
						}
					}
				}

				NPC npc = Main.npc[i];
				if (npc.active && !npc.friendly && !npc.townNPC && Vector2.Distance(player.player.Center, npc.Center) <= Main.spawnTileY / 1.5f)
				{
					if (Main.rand.NextFloat() <= 0.05f / 120f)
					{
						npc.buffImmune[BuffID.Frozen] = false;
						npc.AddBuff(BuffID.Frozen, 300);
					}
					else
					{
						npc.buffImmune[BuffID.Chilled] = false;
						npc.AddBuff(BuffID.Chilled, 2);
					}
				}
			}
		}
	}
}
