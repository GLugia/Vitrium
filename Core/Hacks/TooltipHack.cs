using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace Vitrium.Core.Hacks
{
	internal static class TooltipHack
	{
		public static void FixTooltips(Item item, List<TooltipLine> tooltips)
		{
			TooltipLine[] vn = tooltips.Where(a => a.mod.Equals("Terraria")).ToArray();
			Item nitem = new Item();
			nitem.netDefaults(item.netID);

			Item pref = nitem.Clone();
			pref.Prefix(item.prefix);

			try
			{
				foreach (TooltipLine v in vn)
				{
					double on = 0d;
					string ntt = v.text;
					Color? col = v.overrideColor;
					string ftt = new string(v.text.Reverse().ToArray().TakeWhile(a => !char.IsDigit(a)).Reverse().ToArray());

					switch (v.Name)
					{
						case "PrefixDamage":
							{
								if (nitem.damage > 0)
								{
									ntt = GetPrefixNormString(nitem.damage, pref.damage, ref on, ref col);
								}
								else
								{
									ntt = GetPrefixNormString(pref.damage, nitem.damage, ref on, ref col);
								}

								break;
							}
						case "PrefixSpeed":
							{
								if (nitem.useAnimation <= 0)
								{
									ntt = GetPrefixNormString(nitem.useAnimation, pref.useAnimation, ref on, ref col);
								}
								else
								{
									ntt = GetPrefixNormString(pref.useAnimation, nitem.useAnimation, ref on, ref col);
								}

								break;
							}
						case "PrefixCritChance":
							{
								on = pref.crit - nitem.crit;
								float defcol = Main.mouseTextColor / 255f;
								int alpha = Main.mouseTextColor;
								ntt = "";

								if (on >= 0)
								{
									ntt += "+";
									col = new Color((byte)(120f * defcol), (byte)(190f * defcol), (byte)(120f * defcol), alpha);
								}
								else
								{
									col = new Color((byte)(190f * defcol), (byte)(120f * defcol), (byte)(120f * defcol), alpha);
								}

								ntt += on.ToString(CultureInfo.InvariantCulture);

								break;
							}
						case "PrefixUseMana":
							{
								if (nitem.mana != 0)
								{
									float defcol = Main.mouseTextColor / 255f;
									int alpha = Main.mouseTextColor;

									ntt = GetPrefixNormString(nitem.mana, pref.mana, ref on, ref col);

									if (pref.mana < nitem.mana)
									{
										col = new Color((byte)(120f * defcol), (byte)(190f * defcol), (byte)(120f * defcol), alpha);
									}
									else
									{
										col = new Color((byte)(190f * defcol), (byte)(120f * defcol), (byte)(120f * defcol), alpha);
									}
								}

								break;
							}
						case "PrefixSize":
							{
								if (nitem.scale > 0)
								{
									ntt = GetPrefixNormString(nitem.scale, pref.scale, ref on, ref col);
								}
								else
								{
									ntt = GetPrefixNormString(pref.scale, nitem.scale, ref on, ref col);
								}

								break;
							}
						case "PrefixShootSpeed":
							{
								if (nitem.shootSpeed > 0)
								{
									ntt = GetPrefixNormString(nitem.shootSpeed, pref.shootSpeed, ref on, ref col);
								}
								else
								{
									ntt = GetPrefixNormString(pref.shootSpeed, nitem.shootSpeed, ref on, ref col);
								}

								break;
							}
						case "PrefixKnockback":
							{
								if (nitem.knockBack > 0)
								{
									ntt = GetPrefixNormString(nitem.knockBack, pref.knockBack, ref on, ref col);
								}
								else
								{
									ntt = GetPrefixNormString(pref.knockBack, nitem.knockBack, ref on, ref col);
								}

								break;
							}
						default: continue;
					}

					int ttindex = tooltips.FindIndex(a => a.mod.Equals(v.mod) && a.Name.Equals(v.Name));

					if (ttindex != -1)
					{
						if (on == 0d)
						{
							tooltips.RemoveAt(ttindex);
						}
						else
						{
							tooltips[ttindex].text = $"{ntt}{ftt}";
							tooltips[ttindex].overrideColor = col ?? Color.White;
							tooltips[ttindex].isModifier = true;

							if (on < 0)
							{
								tooltips[ttindex].isModifierBad = true;
							}
							else
							{
								tooltips[ttindex].isModifierBad = false;
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				Main.NewTextMultiline(e.ToString());
			}
		}

		private static string GetPrefixNormString(float a, float b, ref double on, ref Color? col)
		{
			float defcol = Main.mouseTextColor / 255f;
			int alpha = Main.mouseTextColor;
			if (a == 0f && b != 0f)
			{
				if (b > 0f)
				{
					on = 1.0;
					col = new Color?(new Color((byte)(120f * defcol), (byte)(190f * defcol), (byte)(120f * defcol), alpha));
					return "+" + b.ToString(CultureInfo.InvariantCulture);
				}
				on = -1.0;
				col = new Color?(new Color((byte)(190f * defcol), (byte)(120f * defcol), (byte)(120f * defcol), alpha));
				return "-" + b.ToString(CultureInfo.InvariantCulture);
			}
			else
			{
				double diff = Math.Ceiling((b - a) / a * 100.0);
				on = diff;
				if (diff > 0.0)
				{
					col = new Color?(new Color((byte)(120f * defcol), (byte)(190f * defcol), (byte)(120f * defcol), alpha));
					return "+" + diff.ToString(CultureInfo.InvariantCulture);
				}
				col = new Color?(new Color((byte)(190f * defcol), (byte)(120f * defcol), (byte)(120f * defcol), alpha));
				return diff.ToString(CultureInfo.InvariantCulture);
			}
		}
	}
}
