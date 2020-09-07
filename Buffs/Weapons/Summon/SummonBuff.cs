using Terraria;
using Vitrium.Core;

namespace Vitrium.Buffs.Weapons.Summon
{
	public abstract class SummonBuff : VitriBuff
	{
		public override bool ApplicableTo(Item item)
		{
			return item.IsSummon();
		}
	}
}
