using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Vitrium.Core;

namespace Vitrium.Buffs.Armor.Body
{
	public class DevilDebuff : VitriBuff
	{
		public override bool ApplicableTo(Item item) => false;

		public override string Name => "Curse of the Devil's Advocate";
		public override string BuffTooltip => "Quickly losing Life";

		public override void UpdateLifeRegen(VPlayer player)
		{
			player.player.lifeRegen -= (int)(player.player.statLifeMax2 * 0.001);
		}

		public override void UpdateLifeRegen(VNPC npc, ref int damage)
		{
			damage += 1000;
			npc.npc.lifeRegen -= 1000;
			Main.NewText(damage);
		}
	}

	public class DevilAura : BodyBuff
	{
		public override string Name => "Devil's Advocate";
		public override string BuffTooltip => "Everything dies";
		public override string ItemTooltip => "Go back to hell";

		public override void ResetEffects(VPlayer player)
		{
			player.player.buffImmune[Vitrium.GetBuff<DevilDebuff>().Type] = false;
		}

		public override void PostUpdate(VPlayer player)
		{
			var debuff = Vitrium.GetBuff<DevilDebuff>();
			player.player.buffImmune[debuff.Type] = true;

			foreach (var member in Main.player.Where(a => a.active && (a.team != player.player.team || a == player.player) && player.player.Distance(a.Center) <= 1600))
			{
				member.buffImmune[Terraria.ID.BuffID.BrokenArmor] = false;
				member.buffImmune[Terraria.ID.BuffID.Ichor] = false;
				member.AddBuff(Terraria.ID.BuffID.BrokenArmor, 1);
				member.AddBuff(Terraria.ID.BuffID.Ichor, 1);
			}

			foreach (var friend in Main.npc.Where(a => a.active && !a.townNPC && player.player.Distance(a.Center) <= 1600))
			{
				friend.buffImmune[Terraria.ID.BuffID.BrokenArmor] = false;
				friend.buffImmune[Terraria.ID.BuffID.Ichor] = false;
				friend.AddBuff(Terraria.ID.BuffID.BrokenArmor, 1);
				friend.AddBuff(Terraria.ID.BuffID.Ichor, 1);
			}
		}
	}
}
