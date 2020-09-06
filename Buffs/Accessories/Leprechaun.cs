using Terraria.ID;
using Vitrium.Core;

namespace Vitrium.Buffs.Accessories
{
	public class Leprechaun : AccessoryBuff
	{
		public override string BuffTooltip => "Luck of the Irish";
		public override string ItemTooltip => "Lucky coins";
		public override string Texture => $"Terraria/Buff_{BuffID.Midas}";

		public override bool PreNPCLoot(VNPC npc)
		{
			npc.npc.value *= 2;
			return base.PreNPCLoot(npc);
		}
	}
}
