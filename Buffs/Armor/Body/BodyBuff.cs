using Terraria;
using Vitrium.Core;

namespace Vitrium.Buffs.Armor.Body
{
	public abstract class BodyBuff : VitriBuff
	{
		public sealed override bool ApplicableTo(Item item)
		{
			return item.IsBody();
		}
	}
}
