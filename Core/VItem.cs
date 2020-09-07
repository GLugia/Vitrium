using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;
using Vitrium.Buffs;
using Vitrium.Core.Hacks;

namespace Vitrium.Core
{
	public class VItem : GlobalItem
	{
		public static VItem GetData(Item item) => item?.GetGlobalItem<VItem>() ?? null;
		public VitriBuff buff;
		public string Hash = "";

		public VitriBuff[] NewBuffs(Item item)
		{
			var wr = new WeightedRandom<VitriBuff>();

			foreach (var buff in Vitrium.VitriBuffs.Where(a => a.IApplicableTo(item)))
			{
				wr.Add(buff, buff.Weight);
				wr.Add(null, buff.Weight * 0.25f);
			}

			var ret = new VitriBuff[3];

			int j = 0;
			for (int i = 0; i < 3; i++)
			{
				var buff = wr.Get();

				ret[j] = wr.Get();

				if (ret[j] != null)
				{
					j++;
				}
			}

			return ret;
		}

		public override bool OnPickup(Item item, Player player) // @TODO only if null and has not rolled
		{
			var data = GetData(item);
			var buffs = NewBuffs(item);
			data.buff = buffs[Main.rand.Next(0, buffs.Length)];
			data.Hash = Main.rand.NextString();
			return base.OnPickup(item, player);
		}

		public override void PostReforge(Item item)
		{
			var data = GetData(item);
			var buffs = NewBuffs(item);
			data.buff = buffs[Main.rand.Next(0, buffs.Length)];
			data.Hash = Main.rand.NextString();
			base.PostReforge(item);
		}

		public override void OnCraft(Item item, Recipe recipe)
		{
			var data = GetData(item);
			var buffs = NewBuffs(item);
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

		public bool IsTheSameAs(VItem item) => Hash == item.Hash;

		public override bool InstancePerEntity => true;
		public override bool CloneNewInstances => true;

		public void Clone(VItem item)
		{
			Hash = item.Hash;
			buff = item.buff;
		}

		public override bool NeedsSaving(Item item) => buff != null && Hash != null;

		public override void Load(Item item, TagCompound tag)
		{
			Hash = tag.GetString("ItemHash");

			if (tag.ContainsKey("ItemBuff"))
			{
				buff = (VitriBuff)Vitrium.Instance.GetBuff(tag.GetString("ItemBuff"));
				buff.Load(item, tag);
			}
		}

		public override TagCompound Save(Item item)
		{
			var tag = new TagCompound()
			{
				{ "ItemHash", Hash }
			};

			if (buff != null)
			{
				tag.Add("ItemBuff", buff.UnderlyingName);
				buff.Save(item, tag);
			}

			return tag;
		}

		public override void NetSend(Item item, BinaryWriter writer)
		{
			writer.Write(Hash);
			writer.Write(buff?.UnderlyingName);
		}

		public override void NetReceive(Item item, BinaryReader reader)
		{
			Hash = reader.ReadString();
			var name = reader.ReadString();

			if (name != null)
			{
				buff = (VitriBuff)Vitrium.Instance.GetBuff(name);
				buff.NetReceive(item, reader);
			}
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			if (item.IsValid() && item.Enchantable())
			{
				TooltipHack.FixTooltips(item, tooltips);

				try
				{
					var data = GetData(item).buff;
					if (data != null)
					{
						var name = tooltips.FindIndex(a => a.mod == "Terraria" && a.Name == "ItemName");

						if (name != -1)
						{
							var old = tooltips.ElementAt(name);
							old.text = $"{new string(buff.Name.Replace("Aura", "").Replace("Buff", "").Replace("Debuff", "").Trim(' ').ToArray())} {old.text}";
							old.overrideColor = Main.DiscoColor;
							tooltips.RemoveAt(name);

							tooltips.Insert(name, old);
							tooltips.Insert(++name, new TooltipLine(Vitrium.Instance, "V:I:Bufftip", data.ItemTooltip) { overrideColor = Main.DiscoColor });
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
