using System.Linq;
using Terraria;
using Terraria.ID;
using Vitrium.Core;

namespace Vitrium.Buffs.Accessories
{
	public class Jet : AccessoryBuff
	{
		public override string Name => "Jet's";
		public override string BuffTooltip => "Speed is all you need";
		public override string ItemTooltip => "Gotta go fast";
		public override string Texture => $"Terraria/buff_{BuffID.Mining}";

		public override void ModifyWeaponDamage(VPlayer player, Item item, ref float add, ref float mult, ref float flat)
		{
			item.useTime /= 2;
			item.useAnimation /= 2;
		}
	}
}
