using System;
using Terraria;
using Terraria.ModLoader;
using Vitrium.Buffs;

namespace Vitrium.Core.Cache
{
	public class PlayerCache : ModPlayer
	{
		private int selected;
		private Item mouse;
		private Item[] equips;

		public override void Initialize()
		{
			selected = player.selectedItem;
			mouse = Main.mouseItem;
			equips = new Item[8 + player.extraAccessorySlots];
		}

		public override void PostUpdate()
		{
			if (Main.mouseItem != mouse)
			{
				mouse = Main.mouseItem; // cache mouse item
			}

			if (player.selectedItem != selected)
			{
				selected = player.selectedItem; // cache selected item
			}

			if (mouse != null && mouse.IsValid() && mouse.Enchantable() && (mouse.IsWeapon() || mouse.IsTool()))
			{
				VitriBuff data = VItem.GetData(mouse).buff;

				if (data != null)
				{
					player.AddBuff(data.GetType().Name);
				}
			}
			else if (player.inventory[selected] != null
				&& player.inventory[selected].IsValid()
				&& player.inventory[selected].Enchantable()
				&& (player.inventory[selected].IsWeapon() || player.inventory[selected].IsTool()))
			{
				VitriBuff data = VItem.GetData(player.inventory[selected]).buff;

				if (data != null)
				{
					player.AddBuff(data.GetType().Name);
				}
			}

			if (equips.Length < 8 + player.extraAccessorySlots)
			{
				Array.Resize(ref equips, 8 + player.extraAccessorySlots); // account for dark heart and similar items
			}

			for (int i = 0; i < 8 + player.extraAccessorySlots; i++)
			{
				if (equips[i] != player.armor[i])
				{
					equips[i] = player.armor[i]; // cache player armor and accessories that aren't vanity or dyes
												 //Main.NewText($"Equipped {equips[i]?.Name}");
				}

				if (equips[i] != null && equips[i].IsValid() && equips[i].Enchantable())
				{
					VitriBuff data = VItem.GetData(equips[i]).buff;

					if (data != null)
					{
						player.AddBuff(data.GetType().Name);
					}
				}
			}
		}
	}
}
