using Terraria;
using Vitrium.Core;

namespace Vitrium.Buffs.Tools
{
	public abstract class ToolBuff : VitriBuff
	{
		public override bool ApplicableTo(Item item)
		{
			return item.IsTool();
		}
	}
}
