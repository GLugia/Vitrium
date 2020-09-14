using Terraria;
using Vitrium.Core;
using Vitrium.Core.Cache;

namespace Vitrium.Buffs.Armor.Body
{
	public sealed class RegenBuff : VitriBuff
	{
		public override bool ApplicableTo(Item item)
		{
			return false;
		}

		public override string Name => "Heaven's Resort";
		public override string Tooltip => "Would you like Tulslaeh to massage you?";
		public override string Texture => "LifeFlat";

		public override void UpdateLifeRegen(VPlayer player)
		{
			player.player.lifeRegen += 25;
		}

		public override void UpdateLifeRegen(VNPC npc, ref int damage)
		{
			npc.npc.lifeRegen += 25;
		}
	}

	public class RegenAura : BodyBuff
	{
		public override string Name => "Heaven's Resort";
		public override string Tooltip => "Some call you \"Tulslaeh\"";
		public override string Texture => "LifeFlat";

		public override void ResetEffects(VPlayer player)
		{
			player.player.MakeWeakTo("regenbuff");
		}

		public override void PostUpdate(VPlayer player)
		{
			player.player.MakeImmuneTo("regenbuff");

			for (int i = 0; i < Main.npc.Length; i++)
			{
				if (i < 255)
				{
					Player member = Main.player[i];
					if (member.active && !member.dead && !member.ghost && member.team == player.player.team && player.player.Distance(member.Center) <= Main.spawnTileX / 2)
					{
						Main.player[i].AddBuff("regenbuff");
					}
				}

				NPC friend = Main.npc[i];
				if (friend.active && (friend.friendly || friend.townNPC) && player.player.Distance(friend.Center) <= Main.spawnTileX / 2)
				{
					Main.player[i].AddBuff("regenbuff");
				}
			}
		}
	}
}
