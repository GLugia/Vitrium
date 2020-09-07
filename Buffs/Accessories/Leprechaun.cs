using Terraria.ID;
using Vitrium.Core;

namespace Vitrium.Buffs.Accessories
{
	public class Leprechaun : AccessoryBuff
	{
		public override string Name => "Luck of the Irish";
		public override string Tooltip => "Follow the rainbow";
		public override string Texture => $"Terraria/Buff_{BuffID.Midas}";

		public override bool PreNPCLoot(VNPC npc) // @TODO weighted money rates
		{
			// 0.5, 30
			// 1, 50
			// 1.5, 15
			// 2, 5
			npc.npc.value *= 2;
			return base.PreNPCLoot(npc);
		}
	}
}
