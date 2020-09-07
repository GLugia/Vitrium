using Terraria;
using Terraria.ID;
using Vitrium.Core;

namespace Vitrium.Buffs.Accessories
{
	public class MightyPen : AccessoryBuff
	{
		public override string Name => "Champion's Pen";
		public override string Tooltip => "Now the pen really is mightier than the sword";
		public override string Texture => $"Terraria/buff_{BuffID.Sharpened}";

		public override void ModifyWeaponDamage(VPlayer player, Item item, ref float add, ref float mult, ref float flat)
		{
			if (item.IsTool())
			{
				add *= 1.5f;
				mult *= 1.5f;
				item.useTime /= 2;
				item.useAnimation /= 2;
				item.autoReuse = true;
				item.useTurn = true;
			}
			else if (item.IsWeapon())
			{
				item.autoReuse = false;
				item.useTurn = false;
				item.useTime *= 2;
				item.useAnimation *= 2;
				add /= 1.5f;
				mult /= 1.5f;
			}

			//flat += 10f;
			//mult += 5f;
		}
	}
}
