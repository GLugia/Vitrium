using Terraria;
using Vitrium.Core;

namespace Vitrium.Buffs.Weapons.Magic
{
	public abstract class MagicBuff : VitriBuff
	{
		public sealed override bool ApplicableTo(Item item)
		{
			return item.IsMagic();
		}
	}
}
