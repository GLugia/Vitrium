using System.Linq;
using Terraria;
using Vitrium.Core;

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
		public override string Texture => "Vitrium/Buffs/Armor/Body/LifeFlat";

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
		public override string Texture => "Vitrium/Buffs/Armor/Body/LifeFlat";

		public override void ResetEffects(VPlayer player)
		{
			player.player.buffImmune[Vitrium.GetBuff<RegenBuff>().Type] = false;
		}

		public override void PostUpdate(VPlayer player)
		{
			RegenBuff buff = Vitrium.GetBuff<RegenBuff>();
			player.player.buffImmune[buff.Type] = true;

			foreach (Player member in Main.player.Where(a => a.active && a.team == player.player.team && player.player.Distance(a.Center) <= 1600))
			{
				VPlayer.GetData(member).AddBuff(buff);
			}

			foreach (NPC npc in Main.npc.Where(a => a.active && (a.townNPC || a.friendly) && player.player.Distance(a.Center) <= 1600))
			{
				VNPC.GetData(npc).AddBuff(buff);
			}
		}
	}
}
