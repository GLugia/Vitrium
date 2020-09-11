using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Vitrium.Core;

namespace Vitrium.Buffs.Armor.Body
{
	public class AngelBuff : VitriBuff
	{
		public override bool ApplicableTo(Item item)
		{
			return false;
		}

		public override string Name => "Angel's Aura";
		public override string Tooltip => "Heroes never die!";
		public override string Texture => $"Terraria/buff_{BuffID.PaladinsShield}";

		public override bool PreHurt(VPlayer player, bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
			if (player.player.statLife - (damage * (crit ? 2 : 1)) <= 0)
			{
				player.player.statLife = player.player.statLifeMax2 + damage;
				player.AddBuff(Vitrium.GetBuff<AngelDebuff>(), 108000);
			}

			return true;
		}
	}

	public class AngelDebuff : VitriBuff
	{
		public override bool ApplicableTo(Item item)
		{
			return false;
		}

		public override string Name => "Angel's Aura";
		public override string Tooltip => "Cooling down...";
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
		public override string Tooltip => "Heroes never die!";
		public override string Texture => $"Terraria/buff_{BuffID.PaladinsShield}";

		public override void ResetEffects(VPlayer player)
		{
			player.player.buffImmune[ModContent.BuffType<AngelBuff>()] = false;
		}

		public override void PostUpdate(VPlayer player)
		{
			AngelBuff buff = Vitrium.GetBuff<AngelBuff>();
			player.player.buffImmune[buff.Type] = true;

			foreach (Player member in Main.player.Where(a => a.active && a.team == player.player.team && player.player.Distance(a.Center) <= 1600 && !a.HasBuff(ModContent.BuffType<AngelDebuff>())))
			{
				VPlayer.GetData(member).AddBuff(buff);
			}

			foreach (NPC friend in Main.npc.Where(a => a.active && (a.friendly || a.townNPC) && player.player.Distance(a.Center) <= 1600 && !a.HasBuff(ModContent.BuffType<AngelDebuff>())))
			{
				VNPC.GetData(friend).AddBuff(buff);
			}
		}
	}
}
