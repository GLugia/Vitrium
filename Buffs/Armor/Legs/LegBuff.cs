using Terraria;
using Vitrium.Core;

namespace Vitrium.Buffs.Armor.Legs
{
	public abstract class LegBuff : VitriBuff
	{
		public sealed override bool ApplicableTo(Item item) => item.IsLegs();
	}
}
