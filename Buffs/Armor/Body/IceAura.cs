using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Vitrium.Core;

namespace Vitrium.Buffs.Armor.Body
{
	public class IceAura : BodyBuff
	{
		public override string Name => "Blizzard";
		public override string Tooltip => "You give off quite the cold shoulder";
		public override string Texture => $"Terraria/buff_{BuffID.Frozen}";

		public override void PostUpdate(VPlayer player)
		{
			IEnumerable<NPC> npcs = Main.npc.Where(a => a.active && !a.friendly && !a.townNPC && Vector2.Distance(a.Center, player.player.Center) <= Main.spawnTileY / 1.5);
			IEnumerable<Player> players = Main.player.Where(a => a.active && a.team != player.player.team && a.whoAmI != player.player.whoAmI && Vector2.Distance(a.Center, player.player.Center) <= Main.spawnTileY / 1.5);

			foreach (NPC enemy in npcs)
			{
				if (Main.rand.NextFloat(1f) <= 0.05f / 60f)
				{
					enemy.buffImmune[BuffID.Frozen] = false;
					enemy.AddBuff(BuffID.Frozen, 300);
				}
				else
				{
					enemy.buffImmune[BuffID.Chilled] = false;
					enemy.AddBuff(BuffID.Chilled, 2);
				}
			}

			foreach (Player enemy in players)
			{
				if (Main.rand.NextFloat() <= 0.05f / 60f)
				{
					enemy.buffImmune[BuffID.Frozen] = false;
					enemy.AddBuff(BuffID.Frozen, 300);
				}
				else
				{
					enemy.buffImmune[BuffID.Chilled] = false;
					enemy.AddBuff(BuffID.Chilled, 2);
				}
			}
		}
	}
}
