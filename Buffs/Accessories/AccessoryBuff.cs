using Terraria;
using Vitrium.Core;

namespace Vitrium.Buffs.Accessories
{
	public abstract class AccessoryBuff : VitriBuff
	{
		public sealed override bool ApplicableTo(Item item)
		{
			return item.IsAccessory();
		}
	}
}
