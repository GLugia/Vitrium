using Terraria;
using Vitrium.Core;

namespace Vitrium.Buffs.Weapons
{
	public abstract class WeaponBuff : VitriBuff
	{
		public sealed override bool ApplicableTo(Item item)
		{
			return item.IsWeapon();
		}
	}
}
