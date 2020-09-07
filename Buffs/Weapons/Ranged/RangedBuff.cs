using Terraria;
using Vitrium.Core;

namespace Vitrium.Buffs.Weapons.Ranged
{
	public abstract class RangedBuff : VitriBuff
	{
		public sealed override bool ApplicableTo(Item item)
		{
			return item.IsRanged();
		}
	}
}
