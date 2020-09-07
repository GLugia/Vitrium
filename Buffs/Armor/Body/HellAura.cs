using System.Linq;
using Terraria;
using Vitrium.Core;

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
			player.player.buffImmune[Vitrium.GetBuff<HellDebuff>().Type] = false;
		}

		public override void PostUpdate(VPlayer player)
		{
			HellDebuff debuff = Vitrium.GetBuff<HellDebuff>();
			player.player.buffImmune[debuff.Type] = true;

			foreach (Player member in Main.player.Where(a => a.active && a.team != player.player.team && player.player.Distance(a.Center) <= Main.spawnTileX))
			{
				VPlayer.GetData(member).AddBuff(debuff);
			}

			foreach (NPC friend in Main.npc.Where(a => a.active && !a.townNPC && player.player.Distance(a.Center) <= 1600))
			{
				VNPC.GetData(friend).AddBuff(debuff);
			}
		}
	}
}
