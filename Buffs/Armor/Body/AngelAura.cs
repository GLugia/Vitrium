using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Vitrium.Core;
using Vitrium.Core.Cache;

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
				player.player.AddBuff("angeldebuff", 108000);

				if (Main.netMode != NetmodeID.SinglePlayer)
				{
					NetMessage.SendData(MessageID.PlayerHealth, -1, -1, null, player.player.whoAmI, player.player.statLife, player.player.statLifeMax, player.player.statLifeMax2);
				}
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
			player.player.MakeWeakTo("angelbuff");
		}

		public override void PostUpdate(VPlayer player)
		{
			player.player.MakeImmuneTo("angelbuff");

			for (int i = 0; i < Main.npc.Length; i++)
			{
				if (i < 255)
				{
					Player member = Main.player[i];
					if (member.active && !member.dead && !member.ghost && member.team == player.player.team && player.player.Distance(member.Center) <= Main.spawnTileX / 2 && !member.HasBuff(ModContent.BuffType<AngelDebuff>()))
					{
						member.AddBuff("angelbuff");
					}
				}

				NPC friend = Main.npc[i];
				if (friend.active && (friend.friendly || friend.townNPC) && player.player.Distance(friend.Center) <= Main.spawnTileX / 2 && !friend.HasBuff(ModContent.BuffType<AngelDebuff>()))
				{
					Main.npc[i].AddBuff("angelbuff");
				}
			}
		}
	}
}
