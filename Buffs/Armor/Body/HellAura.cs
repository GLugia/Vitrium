using Terraria;
using Vitrium.Core;
using Vitrium.Core.Cache;

namespace Vitrium.Buffs.Armor.Body
{
	public class HellDebuff : VitriBuff
	{
		public override bool ApplicableTo(Item item)
		{
			return false;
		}

		public override string Name => "Hell's Fury";
		public override string Tooltip => "I hurt myself... Today...";

		public override void PreUpdate(VPlayer player)
		{
			player.player.AddBuff(Terraria.ID.BuffID.OnFire, 2);
			player.player.AddBuff(Terraria.ID.BuffID.Burning, 2);
		}

		public override bool PreAI(VNPC npc)
		{
			npc.npc.AddBuff(Terraria.ID.BuffID.OnFire, 2);
			npc.npc.AddBuff(Terraria.ID.BuffID.Burning, 2);
			return base.PreAI(npc);
		}
	}

	public class HellAura : BodyBuff
	{
		public override string Name => "Hell's Fury";
		public override string Tooltip => "I fell down into a burning ring of fire...";

		public override void ResetEffects(VPlayer player)
		{
			player.player.MakeWeakTo("helldebuff");
		}

		public override void PostUpdate(VPlayer player)
		{
			player.player.MakeImmuneTo("helldebuff");

			for (int i = 0; i < Main.npc.Length; i++)
			{
				if (i < 255)
				{
					Player member = Main.player[i];
					if (member.active && !member.dead && !member.ghost && member.team == player.player.team && player.player.Distance(member.Center) <= Main.spawnTileY / 1.5)
					{
						Main.player[i].AddBuff("helldebuff");
					}
				}

				NPC friend = Main.npc[i];
				if (friend.active && (!friend.friendly || !friend.townNPC) && player.player.Distance(friend.Center) <= Main.spawnTileY / 1.5)
				{
					Main.npc[i].AddBuff("helldebuff");
				}
			}
		}
	}
}
