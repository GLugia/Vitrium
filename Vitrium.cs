using log4net;
using Microsoft.Xna.Framework;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using Vitrium.Buffs;
using Vitrium.Buffs.Armor.Body;
using Vitrium.Core;
using Vitrium.Core.Cache;
using Vitrium.UI;

namespace Vitrium
{
	public class Vitrium : Mod
	{
		public override uint ExtraPlayerBuffSlots => 33;
		public override string Name => "Vitrium";
		internal static new ILog Logger => ((Mod)Instance)?.Logger;
		public static Vitrium Instance { get; private set; }
		internal UserInterface UI;
		internal AltarUI UIState;
		private GameTime lastUpdateGameTime;

		internal static IEnumerable<VitriBuff> VitriBuffs;
		internal static IEnumerable<FieldInfo> VanillaBuffs;

		public override void Load()
		{
			VPlayer.GetData(Main.LocalPlayer).buffbuffer;
			Instance = this;

			AutoBuild.Load();

			Main.instance.Exiting += ForceUnload;

			if (!Main.dedServ)
			{
				VitriBuffs = Code.GetTypes().Where(a => !a.IsAbstract && a.IsSubclassOf(typeof(VitriBuff))).Select(a => GetBuff(a.Name) as VitriBuff).Distinct();
				VanillaBuffs = Main.instance.GetType().Assembly.GetTypes().FirstOrDefault(a => a.Name == "BuffID").GetRuntimeFields();

				UI = new UserInterface();

				UIState = new AltarUI();
				//UIState.Activate();
			}
		}

		public override void UpdateUI(GameTime gameTime)
		{
			lastUpdateGameTime = gameTime;

			if (UI?.CurrentState != null)
			{
				UI.Update(gameTime);
			}
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			if (UI != null && UI.CurrentState != null)
			{
				int index = layers.FindIndex(a => a.Name.Equals("Vanilla: Mouse Text"));

				if (index != -1)
				{
					layers.Insert(index, new LegacyGameInterfaceLayer
						(
							"Vitrium: UI",
							delegate
							{
								UI.Draw(Main.spriteBatch, lastUpdateGameTime);
								return true;
							},
							InterfaceScaleType.UI
						));
				}
			}
		}

		internal void ToggleAltarUI()
		{
			if (UI?.CurrentState != null)
			{
				UI?.SetState(null);
			}
			else
			{
				UI?.SetState(UIState);
			}
		}

		private void ForceUnload(object sender, EventArgs e)
		{
			Unload();

			AutoBuild.Save();
		}

		public override void Unload()
		{
			VitriBuffs = null;
			VanillaBuffs = null;
			UI = null;
			UIState?.Deactivate();
			UIState = null;
			Instance = null;
		}

		public override void PreUpdateEntities()
		{
			foreach (var proj in Main.projectile.Where(a => a != null && a.active && a.minion))
			{
				ProjCache.GetData(proj).ApplyBuffs();
			}

			foreach (var player in Main.player.Where(a => a != null && a.active))
			{
				VPlayer.GetData(player).ApplyBuffs();
			}

			foreach (var npc in Main.npc.Where(a => a != null && a.active))
			{
				VNPC.GetData(npc).ApplyBuffs();
			}
		}

		public static int GetVanillaBuff(string name)
		{
			if (VanillaBuffs != null)
			{
				var first = VanillaBuffs.FirstOrDefault(a => CultureInfo.InvariantCulture.CompareInfo.IndexOf(a.Name, name) >= 0);

				if (first != null)
				{
					return (int)first.GetValue(Main.instance);
				}
			}

			return -1;
		}

		public static new T GetBuff<T>() where T : VitriBuff => (T)BuffLoader.GetBuff(ModContent.BuffType<T>());
	}
}