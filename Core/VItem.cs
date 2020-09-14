using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Vitrium.Buffs;
using Vitrium.Core.Cache;
using Vitrium.Core.Hacks;

namespace Vitrium.Core
{
	public class VItem : GlobalItem
	{
		public static VItem GetData(Item item)
		{
			return item?.GetGlobalItem<VItem>() ?? null;
		}

		public VitriBuff buff;
		public string Hash = "";

		public VitriBuff[] NewBuffs(Item item)
		{
			VitriBuff[] ret = new VitriBuff[3];
			int i = 0;
			int j = 0;
			while (j < 3)
			{
				ret[i] = BuffCache.GetRandomBuff(item);
				if (ret[i] != null)
				{
					i++;
				}
				j++;
			}
			return ret;
		}

		public override bool OnPickup(Item item, Player player) // @TODO only if null and has not rolled
		{
			VitriBuff[] buffs = NewBuffs(item);
			VItem data = GetData(item);
			data.buff = buffs[Main.rand.Next(0, buffs.Length)];
			data.Hash = Main.rand.NextString();
			return base.OnPickup(item, player);
		}

		public override void PostReforge(Item item)
		{
			VitriBuff[] buffs = NewBuffs(item);
			VItem data = GetData(item);
			data.buff = buffs[Main.rand.Next(0, buffs.Length)];
			data.Hash = Main.rand.NextString();
			base.PostReforge(item);
		}

		public override void OnCraft(Item item, Recipe recipe)
		{
			VItem data = GetData(item);
			VitriBuff[] buffs = NewBuffs(item);
			data.buff = buffs[Main.rand.Next(0, buffs.Length)];
			data.Hash = Main.rand.NextString();
			base.OnCraft(item, recipe);
		}

		public override void UpdateInventory(Item item, Player player)
		{
			if (!item.IsValid() || (item.IsValid() && !item.Enchantable()))
			{
				Hash = null;
				buff = null;
				return;
			}
		}

		public bool IsTheSameAs(VItem item)
		{
			return Hash == item.Hash;
		}

		public override bool InstancePerEntity => true;
		public override bool CloneNewInstances => true;

		public void Clone(VItem item)
		{
			Hash = item.Hash;
			buff = item.buff;
		}

		public override bool NeedsSaving(Item item)
		{
			return buff != null && Hash != null;
		}

		public override void Load(Item item, TagCompound tag)
		{
			Hash = tag.GetString("ItemHash");
			buff = (VitriBuff)Vitrium.Instance.GetBuff(tag.GetString("ItemBuff"));
			buff?.Load(item, tag);
		}

		public override TagCompound Save(Item item)
		{
			TagCompound tag = new TagCompound()
			{
				{ "ItemHash", Hash },
				{ "ItemBuff", buff?.UnderlyingName }
			};

			buff?.Save(item, tag);

			return tag;
		}

		public override void NetSend(Item item, BinaryWriter writer)
		{
			writer.Write(Hash ?? "NULL");
			writer.Write(buff?.UnderlyingName ?? "NULL");
		}

		public override void NetReceive(Item item, BinaryReader reader)
		{
			Hash = reader.ReadString();
			if (Hash == "NULL")
			{
				Hash = null;
			}

			string buffname = reader.ReadString();
			buff = buffname == "NULL" ? null : (VitriBuff)Vitrium.Instance.GetBuff(buffname);
			buff?.NetReceive(item, reader);
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			if (item.IsValid() && item.Enchantable())
			{
				TooltipHack.FixTooltips(item, tooltips);

				try
				{
					VitriBuff data = GetData(item).buff;
					if (data != null)
					{
						int name = tooltips.FindIndex(a => a.mod == "Terraria" && a.Name == "ItemName");

						if (name != -1)
						{
							tooltips.Insert(++name, new TooltipLine(Vitrium.Instance, "V:I:Bufftip", data.Name) { overrideColor = Terraria.ID.Colors.RarityPurple });
						}
					}
				}
				catch (Exception e)
				{
					Main.NewTextMultiline(e.ToString());
				}
			}
		}
	}
}
