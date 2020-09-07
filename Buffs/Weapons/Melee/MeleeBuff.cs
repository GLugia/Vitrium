using Terraria;
using Vitrium.Core;

namespace Vitrium.Buffs.Weapons.Melee
{
	public abstract class MeleeBuff : VitriBuff
	{
		public sealed override bool ApplicableTo(Item item)
		{
			return item.IsMelee();
		}
	}
}
