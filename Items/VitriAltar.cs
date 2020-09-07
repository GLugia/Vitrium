using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Vitrium.Tiles;

namespace Vitrium.Items
{
	public class VitriAltar : ModItem
	{
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();
			DisplayName.SetDefault("Vitri Altar");
		}

		public override void SetDefaults()
		{
			item.width = 58;
			item.height = 32;
			item.maxStack = 999;
			item.useTurn = true;
			item.autoReuse = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.consumable = true;
			item.value = Item.sellPrice(0, 10, 0, 0);
			item.rare = ItemRarityID.Red;
			item.createTile = ModContent.TileType<VitriAltarTile>();
			item.placeStyle = 0;
		}
	}
}
