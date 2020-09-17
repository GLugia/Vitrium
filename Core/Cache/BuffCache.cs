using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;
using Terraria.Utilities;
using Vitrium.Buffs;

namespace Vitrium.Core.Cache
{
	public static class BuffCache
	{
		private static Mod Mod;
		private static WeightedRandom<VitriBuff> WeightedBuffs;
		private static IDictionary<string, int> VitriumBuffs, VanillaBuffs;
		private static IDictionary<int, Dictionary<string, int>> AllPlayerBuffs, AllPlayerBuffers, AllNPCBuffs, AllNPCBuffers;
		private static int NextPlayerID, NextNPCID;

		internal static void Load(Mod mod)
		{
			Stopwatch sw = Stopwatch.StartNew();
			Mod = mod;
			WeightedBuffs = new WeightedRandom<VitriBuff>();
			VitriumBuffs = new Dictionary<string, int>();
			VanillaBuffs = new Dictionary<string, int>();
			AllPlayerBuffs = new Dictionary<int, Dictionary<string, int>>();
			AllPlayerBuffers = new Dictionary<int, Dictionary<string, int>>();
			AllNPCBuffs = new Dictionary<int, Dictionary<string, int>>();
			AllNPCBuffers = new Dictionary<int, Dictionary<string, int>>();
			NextPlayerID = NextNPCID = int.MinValue;

			try
			{
				LoadVitriBuffs();
				LoadVanillaBuffs();
			}
			catch (Exception e)
			{
				Mod.Logger.Error("BuffCache.Load", e);
			}
			finally
			{
				sw.Stop();
				Mod.Logger.Debug(sw.Elapsed.TotalMilliseconds);
			}
		}

		internal static int ReservePlayer()
		{
			int id;
			do
			{
				if (NextPlayerID == int.MaxValue)
				{
					NextPlayerID = int.MinValue;
				}

				id = NextPlayerID;
				NextPlayerID++;
			}
			while (!AllPlayerBuffs.TryAdd(id, new Dictionary<string, int>()));
			AllPlayerBuffers.Add(id, new Dictionary<string, int>());
			return id;
		}

		internal static void DeletePlayer(int globalID)
		{
			AllPlayerBuffers.Remove(globalID);
			AllPlayerBuffs.Remove(globalID);
		}

		internal static int ReserveNPC()
		{
			int id;
			do
			{
				if (NextNPCID == int.MaxValue)
				{
					NextNPCID = int.MinValue;
				}

				id = NextNPCID;
				NextNPCID++;
			}
			while (!AllNPCBuffs.TryAdd(id, new Dictionary<string, int>()));
			AllNPCBuffers.Add(id, new Dictionary<string, int>());
			return id;
		}

		internal static void DeleteNPC(int globalID)
		{
			AllNPCBuffers.Remove(globalID);
			AllNPCBuffs.Remove(globalID);
		}

		internal static void ApplyAllBuffs()
		{
			// Add cached projectile buffs to their owner
			for (int i = 0; i < Main.projectile.Length; i++)
			{
				if (Main.projectile[i].active && Main.projectile[i].minion)
				{
					ProjCache.GetData(Main.projectile[i]).ApplyBuffs();
				}
			}

			// Then activate both npcs and players together
			for (int i = 0; i < Main.npc.Length; i++)
			{
				if (i < 255 && Main.player[i].active)
				{
					int pid = VPlayer.GetData(Main.player[i]).GlobalID;
					if (AllPlayerBuffers.TryGetValue(pid, out Dictionary<string, int> pbuffer)
						&& AllPlayerBuffs.TryGetValue(pid, out Dictionary<string, int> playerbuffs))
					{
						playerbuffs.Clear();
						playerbuffs.AddRange(pbuffer);
						pbuffer.Clear();

						if (!Main.player[i].dead && !Main.player[i].ghost)
						{
							foreach ((string buff, int duration) in playerbuffs)
							{
								int vanilla = TranslateVanilla(buff);
								Main.player[i].AddBuff(vanilla != -1 ? vanilla : TranslateVitri(buff).Type, duration);
							}
						}
					}
				}

				if (Main.npc[i].active)
				{
					int nid = VNPC.GetData(Main.npc[i]).GlobalID;
					if (AllNPCBuffers.TryGetValue(nid, out Dictionary<string, int> nbuffer)
						&& AllNPCBuffs.TryGetValue(nid, out Dictionary<string, int> npcbuffs))
					{
						npcbuffs.Clear();
						npcbuffs.AddRange(nbuffer);
						nbuffer.Clear();

						foreach ((string buff, int duration) in npcbuffs)
						{
							int vanilla = TranslateVanilla(buff);
							Main.npc[i].AddBuff(vanilla != -1 ? vanilla : TranslateVitri(buff).Type, duration);
						}
					}
				}
			}
		}

		internal static void Unload()
		{
			VitriumBuffs = null;
			VanillaBuffs = null;
			AllPlayerBuffs = null;
			AllPlayerBuffers = null;
			AllNPCBuffs = null;
			AllNPCBuffers = null;
			NextNPCID = 0;
			NextPlayerID = 0;
			Mod = null;
		}

		private static void LoadVitriBuffs()
		{
			Type[] types = Mod.Code.GetTypes();

			foreach (Type type in types)
			{
				if (!type.IsAbstract && type.IsSubclassOf(typeof(VitriBuff)))
				{
					VitriBuff buff = TranslateVitri(type.Name);
					string name = type.Name.ToLower();
					if (!VitriumBuffs.ContainsKey(name))
					{
						Vitrium.Logger.Debug(name);
						VitriumBuffs.Add(name, buff.Type);
					}
				}
			}

			Vitrium.Logger.Debug("");

			foreach (string name in VitriumBuffs.Keys)
			{
				Vitrium.Logger.Debug(TranslateVitri(name).Name);
			}
		}

		private static void LoadVanillaBuffs()
		{
			Type[] types = Main.instance.GetType().Assembly.GetTypes();
			Type buffid = null;

			foreach (Type type in types)
			{
				if (type.Name.IsSimilarTo("buffid"))
				{
					buffid = type;
					break;
				}
			}

			if (buffid == null)
			{
				return;
			}

			IEnumerable<FieldInfo> idinfo = buffid.GetRuntimeFields();

			foreach (FieldInfo info in idinfo)
			{
				if (!info.Name.IsSimilarTo("search") && !info.Name.IsSimilarTo("count") && !VanillaBuffs.ContainsKey(info.Name))
				{
					try
					{
						VanillaBuffs.Add(info.Name.ToLower(), (int)info.GetValue(Main.instance));
					}
					catch (Exception e)
					{
						Mod.Logger.Error(info.Name, e);
					}
				}
			}
		}

		public static void BuildWeightedBuffsFor(Item item)
		{
			WeightedBuffs.Clear();
			foreach (string name in VitriumBuffs.Keys)
			{
				VitriBuff buff = TranslateVitri(name);

				if (buff.IApplicableTo(item))
				{
					WeightedBuffs.Add(buff, buff.Weight);
					WeightedBuffs.Add(null, buff.Weight * 0.25d);
				}
			}
		}

		public static VitriBuff GetRandomBuff(Item item)
		{
			VitriBuff ret;

			if (WeightedBuffs.elements.Count <= 0)
			{
				BuildWeightedBuffsFor(item);
			}

			ret = WeightedBuffs.Get();

			if (ret != null && !ret.IApplicableTo(item))
			{
				BuildWeightedBuffsFor(item);
				ret = WeightedBuffs.Get();
			}

			return ret;
		}

		public static void AddBuff(this Player player, string buffname, int duration = 2)
		{
			buffname = buffname.ToLower();
			int id = VPlayer.GetData(player).GlobalID;
			if (AllPlayerBuffers.TryGetValue(id, out Dictionary<string, int> map))
			{
				if (!map.ContainsKey(buffname))
				{
					int vanilla = TranslateVanilla(buffname);
					VitriBuff vitri = TranslateVitri(buffname);

					if ((vanilla != -1 && !player.buffImmune[vanilla])
						||
						(vitri != null && !player.buffImmune[vitri.Type]))
					{
						map.Add(buffname, duration);
					}
				}

				return;
			}

			throw new KeyNotFoundException($"Error in BuffCache.AddBuff: '{player.name}' was not given a buff cache!");
		}

		public static void AddBuff(this NPC npc, string buffname, int duration = 2)
		{
			buffname = buffname.ToLower();
			int id = VNPC.GetData(npc).GlobalID;

			if (AllNPCBuffers.TryGetValue(id, out Dictionary<string, int> map))
			{
				if (!map.ContainsKey(buffname))
				{
					int vanilla = TranslateVanilla(buffname);
					VitriBuff vitri = TranslateVitri(buffname);

					if ((vanilla != -1 && !npc.buffImmune[vanilla])
						|| (vitri != null && !npc.buffImmune[vitri.Type]))
					{
						map.Add(buffname, duration);
					}
				}

				return;
			}

			throw new KeyNotFoundException($"Error in BuffCache.AddBuff: '{npc.FullName}' was not given a buff cache!");
		}

		public static bool HasBuff(this Player player, string buffname)
		{
			buffname = buffname.ToLower();
			int id = VPlayer.GetData(player).GlobalID;
			if (AllPlayerBuffs.TryGetValue(id, out Dictionary<string, int> map))
			{
				return map.ContainsKey(buffname);
			}

			if (AllPlayerBuffers.TryGetValue(id, out Dictionary<string, int> map2))
			{
				return map2.ContainsKey(buffname);
			}

			return false;
		}

		public static bool HasBuff(this NPC npc, string buffname)
		{
			buffname = buffname.ToLower();
			int id = VNPC.GetData(npc).GlobalID;
			if (AllNPCBuffs.TryGetValue(id, out Dictionary<string, int> map))
			{
				return map.ContainsKey(buffname);
			}

			if (AllNPCBuffers.TryGetValue(id, out Dictionary<string, int> map2))
			{
				return map2.ContainsKey(buffname);
			}

			return false;
		}

		public static bool ImmuneTo(this Player player, string name)
		{
			if (VitriumBuffs.TryGetValue(name, out int id))
			{
				return player.buffImmune[id];
			}

			throw new KeyNotFoundException($"ID: {name} returned no results");
		}

		public static void MakeImmuneTo(this Player player, string name)
		{
			if (VitriumBuffs.TryGetValue(name, out int id))
			{
				player.buffImmune[id] = true;
				return;
			}

			throw new KeyNotFoundException($"ID: {name} returned no results");
		}

		public static void MakeWeakTo(this Player player, string name)
		{
			if (VitriumBuffs.TryGetValue(name, out int id))
			{
				player.buffImmune[id] = false;
				return;
			}

			throw new KeyNotFoundException($"ID: {name} returned no results");
		}

		public static bool ImmuneTo(this NPC npc, string name)
		{
			if (VitriumBuffs.TryGetValue(name, out int id))
			{
				return npc.buffImmune[id];
			}

			throw new KeyNotFoundException($"ID: {name} returned no results");
		}

		public static void MakeImmuneTo(this NPC npc, string name)
		{
			if (VitriumBuffs.TryGetValue(name, out int id))
			{
				npc.buffImmune[id] = true;
				return;
			}

			throw new KeyNotFoundException($"ID: {name} returned no results");
		}

		public static void MakeWeakTo(this NPC npc, string name)
		{
			if (VitriumBuffs.TryGetValue(name, out int id))
			{
				npc.buffImmune[id] = false;
				return;
			}

			throw new KeyNotFoundException($"ID: {name} returned no results");
		}

		public static IEnumerable<VitriBuff> GetPlayerBuffs(int globalID)
		{
			if (AllPlayerBuffs.TryGetValue(globalID, out Dictionary<string, int> map))
			{
				foreach (string name in map.Keys)
				{
					yield return TranslateVitri(name);
				}
			}
		}

		public static IEnumerable<VitriBuff> GetNPCBuffs(int globalID)
		{
			if (AllNPCBuffs.TryGetValue(globalID, out Dictionary<string, int> map))
			{
				foreach (string name in map.Keys)
				{
					yield return TranslateVitri(name);
				}
			}
		}

		public static VitriBuff TranslateVitri(string name)
		{
			name = name.ToLower();
			VitriBuff ret = null;

			if (VitriumBuffs.TryGetValue(name, out int id))
			{
				ret = (VitriBuff)ModContent.GetModBuff(id);
			}

			if (ret == null)
			{
				ret = (VitriBuff)Mod.GetBuff(name);
			}

			return ret;
		}

		public static int TranslateVanilla(string name)
		{
			if (VanillaBuffs.TryGetValue(name.ToLower(), out int id))
			{
				return id;
			}

			return -1;
		}
	}
}
