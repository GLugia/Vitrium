using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Vitrium.Items;

namespace Vitrium.Tiles
{
	public class VitriAltarTile : ModTile
	{
		public override void SetDefaults()
		{
			Main.tileFrameImportant[Type] = true;

			TileObjectData.newTile.FullCopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidBottom, TileObjectData.newTile.Width, 0);
			TileObjectData.addTile(Type);
			Main.tileNoAttach[Type] = true;
			Main.tileNoFail[Type] = true;
			Main.tileSpelunker[Type] = true;
			Main.tileValue[Type] = 300;
			Main.tileShine[Type] = 250;
			Main.tileShine2[Type] = true;
			Main.tileLighted[Type] = true;

			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Vitri Altar");
			AddMapEntry(Main.mouseTextColorReal, name);

			drop = ModContent.ItemType<VitriAltar>();
			dustType = DustID.AncientLight;
			soundType = SoundID.Shatter;
			soundStyle = 1;
			mineResist = 4;
			minPick = 25;
		}

		public override bool NewRightClick(int i, int j)
		{
			Vitrium.Instance.ToggleAltarUI();
			return true;
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			r = Main.DiscoR / 10000f;
			g = Main.DiscoG / 10000f;
			b = Main.DiscoB / 10000f;
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 3 : 5;
		}

		public sealed override bool Slope(int i, int j) => false;
	}
}
