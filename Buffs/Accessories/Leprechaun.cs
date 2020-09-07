using Terraria.ID;
using Terraria.Utilities;
using Vitrium.Core;

namespace Vitrium.Buffs.Accessories
{
	public class Leprechaun : AccessoryBuff
	{
		public override string Name => "Luck of the Irish";
		public override string Tooltip => "Follow the rainbow";
		public override string Texture => $"Terraria/Buff_{BuffID.LeafCrystal}";

		public override bool PreNPCLoot(VNPC npc) // @TODO weighted money rates
		{
			WeightedRandom<double> wr = new WeightedRandom<double>();
			wr.Add(0.5, 30f);
			wr.Add(1, 50f);
			wr.Add(1.5, 15f);
			wr.Add(2, 5f);
			npc.npc.value = (int)(npc.npc.value * wr.Get());
			return base.PreNPCLoot(npc);
		}
	}
}
