using Terraria;
using Terraria.ID;
using Vitrium.Core;

namespace Vitrium.Buffs.Weapons.Summon
{
	public class TestDamageBuff : SummonBuff
	{
		public override string Name => "TESTING";
		public override string Tooltip => "This is a test buff";
		public override string Texture => $"Terraria/buff_{BuffID.Inferno}";

		public override void ModifyWeaponDamage(VPlayer player, Item item, ref float add, ref float mult, ref float flat)
		{
			add += 100f;
		}
	}
}
