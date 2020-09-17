using log4net;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using Vitrium.Core;
using Vitrium.Core.Cache;
using Vitrium.Testing;
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
		private readonly Stopwatch sw;

		public Vitrium() : base()
		{
			sw = Stopwatch.StartNew();
			Instance = this;
		}

		public override void Load()
		{
			AutoBuild.Load();
			TestEnchantGenerator.Build();

			Main.instance.Exiting += ForceUnload;
			BuffCache.Load(this);

			if (!Main.dedServ)
			{
				UI = new UserInterface();

				UIState = new AltarUI();
				//UIState.Activate();
			}

			sw.Stop();
			Logger.Debug($"Initializing took {sw.Elapsed.TotalMilliseconds}ms");
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
			BuffCache.Unload();
			UI = null;
			UIState?.Deactivate();
			UIState = null;
			Instance = null;
		}

		public override void PreUpdateEntities()
		{
			TestEnchantGenerator.MakeTag();
			BuffCache.ApplyAllBuffs();
		}
	}
}