using Terraria.ID;
using Vitrium.Core;

namespace Vitrium.Buffs.Armor.Helm
{
	public class Spikey : HelmBuff
	{
		public override string Name => "Spikey";
		public override string BuffTooltip => "Hugging you is the worst";
		public override string ItemTooltip => "+50% thorns";
		public override string Texture => $"Terraria/Buff_{BuffID.Thorns}";

		public override void PostUpdateEquips(VPlayer player)
		{
			player.player.thorns += 0.5f;
		}
	}
}
