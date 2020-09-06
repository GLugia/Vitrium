using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Vitrium.Core;

namespace Vitrium.Buffs.Armor.Body
{
	public class AngelBuff : VitriBuff
	{
		public override bool ApplicableTo(Item item) => false;

		public override string Name => "Angel's Aura";
		public override string BuffTooltip => "You are affected by a party member's aura";
		public override string Texture => $"Terraria/buff_{BuffID.PaladinsShield}";

		public override bool PreHurt(VPlayer player, bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
			if (player.player.statLife - (damage * (crit ? 2 : 1)) <= 0)
			{
				damage = 0;
				crit = false;
				player.player.statLife = player.player.statLifeMax2;
				player.AddBuff(Vitrium.GetBuff<AngelDebuff>(), 108000);
				return false;
			}

			return true;
		}
	}

	public class AngelDebuff : VitriBuff
	{
		public override bool ApplicableTo(Item item) => false;
		public override string Name => "Angel's Aura";
		public override string BuffTooltip => "Cooling down...";
		public override string Texture => $"Terraria/buff_{BuffID.Cursed}";

		public override void SetDefaults()
		{
			base.SetDefaults();
			longerExpertDebuff = true;
		}
	}

	public class AngelAura : BodyBuff
	{
		public override string Name => "Angel's Aura";
		public override string BuffTooltip => $"Nearby party members are ressurected once every {(Main.expertMode ? "60m" : "30m")}";
		public override string ItemTooltip => "Heroes never die!";
		public override string Texture => $"Terraria/buff_{BuffID.PaladinsShield}";

		public override void PostUpdate(VPlayer player)
		{
			var buff = Vitrium.GetBuff<AngelBuff>();
			player.player.buffImmune[buff.Type] = true;

			foreach (var member in Main.player.Where(a => a.active && a.team == player.player.team && player.player.Distance(a.Center) <= 1600 && !a.HasBuff(Vitrium.GetBuff<AngelDebuff>().Type)))
			{
				VPlayer.GetData(member).AddBuff(buff);
			}

			foreach (var friend in Main.npc.Where(a => a.active && (a.friendly || a.townNPC) && player.player.Distance(a.Center) <= 1600 && !a.HasBuff(Vitrium.GetBuff<AngelDebuff>().Type)))
			{
				VNPC.GetData(friend).AddBuff(buff);
			}
		}
	}
}
