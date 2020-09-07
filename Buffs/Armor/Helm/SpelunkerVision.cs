using Terraria.ID;

namespace Vitrium.Buffs.Armor.Helm
{
	public class SpelunkerVision : HelmBuff
	{
		public override string Name => "Spelunker";
		public override string Tooltip => "Time to get some diamonds";
		public override string Texture => $"Terraria/buff_{BuffID.Spelunker}";
	}
}
