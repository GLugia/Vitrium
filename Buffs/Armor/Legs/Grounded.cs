using System.Linq;
using Terraria;
using Terraria.ID;
using Vitrium.Core;

namespace Vitrium.Buffs.Armor.Legs
{
	public class Grounded : LegBuff
	{
		public override string BuffTooltip => "The sky is your enemy";
		public override string ItemTooltip => "Faster feet, weak wings";
		public override string Texture => $"Terraria/Buff_{BuffID.Swiftness}";

		public override void ResetEffects(VPlayer player)
		{
			var wings = player.player.GetEquip(a => a.wingSlot > 0 && !a.vanity);

			if (wings != null)
			{
				if (player.player.wingTime == wings.useTime)
				{
					player.player.wingTime = (int)(player.player.wingTime * 0.1f);
				}
			}
		}

		public override void PostUpdate(VPlayer player)
		{
			player.player.jump = (int)(player.player.jump * 0.1f);
		}

		public override void PostUpdateRunSpeeds(VPlayer player)
		{
			player.player.maxRunSpeed += 4f;
			player.player.accRunSpeed += 4f;
			player.player.runAcceleration *= 1.5f;
			player.player.runSlowdown *= 1.5f;
		}
	}
}
