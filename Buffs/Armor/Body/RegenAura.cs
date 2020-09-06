using System.Linq;
using Terraria;
using Vitrium.Core;

namespace Vitrium.Buffs.Armor.Body
{
	public sealed class RegenBuff : VitriBuff
	{
		public override bool ApplicableTo(Item item) => false;

		public override string Name => "Regeneration Aura";
		public override string BuffTooltip => "You are affected by a party member's aura";
		public override string Texture => "Vitrium/Buffs/Armor/Body/LifeFlat";

		public override void UpdateLifeRegen(VPlayer player)
		{
			player.player.lifeRegen += 10;
		}

		public override void UpdateLifeRegen(VNPC npc, ref int damage)
		{
			npc.npc.lifeRegen += 10;
		}
	}

	public class RegenAura : BodyBuff
	{
		public override string Name => "Healer's";
		public override string ItemTooltip => "Some call you \"Tulslaeh\"";
		public override string BuffTooltip => "Nearby party regenerate more Life";
		public override string Texture => "Vitrium/Buffs/Armor/Body/LifeFlat";

		public override void ResetEffects(VPlayer player)
		{
			player.player.buffImmune[Vitrium.GetBuff<RegenBuff>().Type] = false;
		}

		public override void PostUpdate(VPlayer player)
		{
			var buff = Vitrium.GetBuff<RegenBuff>();
			player.player.buffImmune[buff.Type] = true;

			foreach (var member in Main.player.Where(a => a.active && a.team == player.player.team && player.player.Distance(a.Center) <= 1600))
			{
				VPlayer.GetData(member).AddBuff(buff);
			}

			foreach (var npc in Main.npc.Where(a => a.active && (a.townNPC || a.friendly) && player.player.Distance(a.Center) <= 1600))
			{
				VNPC.GetData(npc).AddBuff(buff);
			}
		}
	}
}
