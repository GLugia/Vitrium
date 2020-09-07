﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;

namespace Vitrium.Core
{
	public static class Utils
	{
		public static bool Enchantable(this Item item)
		{
			return item.IsTool() || item.IsAccessory() || item.IsArmor() || item.IsWeapon();
		}

		public static bool IsArmor(this Item item)
		{
			return item.maxStack <= 1 && (item.headSlot > 0 || item.bodySlot > 0 || item.legSlot > 0) && !item.vanity;
		}

		public static bool IsHelm(this Item item)
		{
			return item.maxStack <= 1 && item.headSlot > 0 && !item.vanity;
		}

		public static bool IsBody(this Item item)
		{
			return item.maxStack <= 1 && item.bodySlot > 0 && !item.vanity;
		}

		public static bool IsLegs(this Item item)
		{
			return item.maxStack <= 1 && item.legSlot > 0 && !item.vanity;
		}

		public static bool IsWeapon(this Item item)
		{
			return item.maxStack <= 1 && item.damage > 0 && item.ammo <= 0 && !item.IsTool();
		}

		public static bool IsSummon(this Item item)
		{
			return item.maxStack <= 1 && item.damage > 0 && item.summon;
		}

		public static bool IsMagic(this Item item)
		{
			return item.magic && item.IsWeapon();
		}

		public static bool IsMelee(this Item item)
		{
			return item.melee && item.IsWeapon();
		}

		public static bool IsRanged(this Item item)
		{
			return item.ranged && item.IsWeapon();
		}

		public static bool IsTool(this Item item)
		{
			return item.maxStack <= 1 && (item.pick > 0 || item.hammer > 0 || item.axe > 0 || item.fishingPole > 0);
		}

		public static bool IsAccessory(this Item item)
		{
			return item.maxStack <= 1 && (item.accessory || item.wingSlot > 0) && !item.vanity;
		}

		public static bool IsValid(this Item item)
		{
			return item != null && item.active && !item.IsAir && item.type > ItemID.None;
		}

		public static bool IsTheSameAs_(this Item item, Item item2)
		{
			return item.IsValid() && item2.IsValid() && item.IsTheSameAs(item2) && VItem.GetData(item).IsTheSameAs(VItem.GetData(item2));
		}

		public static VItem GetItem(this Item item)
		{
			return item != null && item.IsValid() ? VItem.GetData(item) : null;
		}

		public static VPlayer GetPlayer(this Player player)
		{
			return player != null ? VPlayer.GetData(player) : null;
		}

		public static string NextString(this UnifiedRandom rand, int length = 20)
		{
			string ret = "";

			for (int i = 0; i < length; i++)
			{
				ret += (char)('A' + rand.Next(0, 63));
			}

			return ret;
		}

		public static string AutoSpace(this string text)
		{
			string ret = "";
			bool first = true;

			foreach (char c in text)
			{
				if (char.IsUpper(c) && !first)
				{
					ret += " " + c;
				}
				else
				{
					ret += c;
					first = false;
				}
			}

			return ret;
		}

		public static bool IsSimilarTo(this string a, string b)
		{
			return CultureInfo.InvariantCulture.CompareInfo.IndexOf(a, b, CompareOptions.IgnoreCase) >= 0;
		}

		public static void Deconstruct<T1, T2>(this KeyValuePair<T1, T2> tuple, out T1 key, out T2 value)
		{
			key = tuple.Key;
			value = tuple.Value;
		}

		public static ICollection<T> With<T>(this ICollection<T> left, ICollection<T> right)
		{
			ICollection<T> temp = left;

			foreach (T obj in right)
			{
				temp.Add(obj);
			}

			return temp;
		}

		public static bool IsEquipped(this Item item, Player player)
		{
			return ((item.IsWeapon() || item.IsTool()) && (item.IsTheSameAs_(player.inventory[player.selectedItem]) || item.IsTheSameAs_(Main.mouseItem)))
|| ((item.IsArmor() || item.IsAccessory()) && item.IsEquippedArmor(player, out int _));
		}

		public static bool IsEquippedArmor(this Item item, Player player, out int index)
		{
			Item[] armor = player.armor.Take(8 + player.extraAccessorySlots).ToArray();
			index = Array.IndexOf(armor, item);
			return index != -1;
		}

		public static bool IsEquippedVanity(this Item item, Player player, out int index)
		{
			Item[] vanity = player.armor.Skip(8 + player.extraAccessorySlots).Take(8 + player.extraAccessorySlots).ToArray();
			int i = Array.IndexOf(vanity, item);
			index = i + 8 + (player.extraAccessorySlots * 2);
			return i != -1;
		}

		public static bool IsInInventory(this Item item, Player player, out int index)
		{
			index = Array.IndexOf(player.inventory, item);
			return index != -1;
		}

		public static bool TryFindOwner(this Item item, out Player player, out Item[] location, out int index)
		{
			player = Main.player.FirstOrDefault(a => a != null && (item.IsInInventory(a, out int _) || item.IsEquippedArmor(a, out int _) || item.IsEquippedVanity(a, out int _)));

			if (player != null)
			{
				location = player.inventory;
				int id = Array.IndexOf(player.inventory, item);

				if (id == -1)
				{
					location = player.armor;
					id = Array.IndexOf(player.armor, item);
				}

				index = id;
			}
			else
			{
				location = null;
				index = -1;
			}

			return index != -1 && location != null && player != null;
		}

		public static Item GetEquip(this Player player, Func<Item, bool> predicate)
		{
			return player.armor.Take(8 + player.extraAccessorySlots).FirstOrDefault(predicate);
		}
	}
}
