using Terraria;
using Vitrium.Core;

namespace Vitrium.Buffs.Armor.Helm
{
	public abstract class HelmBuff : VitriBuff
	{
		public sealed override bool ApplicableTo(Item item) => item.IsHelm();
	}
}
