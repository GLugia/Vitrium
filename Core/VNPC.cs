using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Vitrium.Buffs;

namespace Vitrium.Core
{
	public class VNPC : GlobalNPC
	{
		public static VNPC GetData(NPC npc)
		{
			var ret = npc.GetGlobalNPC<VNPC>();
			ret.npc = npc;
			return ret;
		}

		private List<(VitriBuff buff, int duration)> bufftuples;
		private List<(VitriBuff buff, int duration)> buffbuffer;
		internal IEnumerable<VitriBuff> buffs => bufftuples?.Select(a => a.buff) ?? Enumerable.Empty<VitriBuff>();
		public T GetBuff<T>() where T : VitriBuff => (T)buffs?.FirstOrDefault(a => a.GetType() == typeof(T));
		public NPC npc { get; private set; }

		public void AddBuff(VitriBuff buff, int duration = 2)
		{
			if (buff != null && !buffbuffer.Select(a => a.buff).Contains(buff))
			{
				buffbuffer.Add((buff, duration));
			}
		}

		internal void ApplyBuffs()
		{
			bufftuples.Clear();
			bufftuples.AddRange(buffbuffer);
			buffbuffer.Clear();

			foreach (var (buff, duration) in bufftuples)
			{
				var vanilla = Vitrium.GetVanillaBuff(buff.Name);

				if (vanilla != -1)
				{
					npc.AddBuff(vanilla, duration);
				}
				else
				{
					npc.AddBuff(buff.Type, duration);
				}
			}
		}

		public override bool InstancePerEntity => true;
		public override bool CloneNewInstances => true;

		public override void SetDefaults(NPC npc)
		{
			bufftuples = new List<(VitriBuff, int)>();
			buffbuffer = new List<(VitriBuff, int)>();
			this.npc = npc;
		}

		public override void ResetEffects(NPC npc)
		{
			foreach (var buff in buffs)
			{
				buff.ResetEffects(this);
			}
		}

		public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
		{
			var vp = VPlayer.GetData(player);
			foreach (var buff in vp.buffs)
			{
				buff.EditSpawnRate(vp, ref spawnRate, ref maxSpawns);
			}
		}

		public override void AI(NPC npc)
		{
			foreach (var buff in buffs)
			{
				buff.NPCAI(this);
			}
		}

		public override bool PreNPCLoot(NPC npc)
		{
			foreach (var buff in buffs)
			{
				if (!buff.PreNPCLoot(this))
				{
					return false;
				}
			}

			return base.PreNPCLoot(npc);
		}

		public override void UpdateLifeRegen(NPC npc, ref int damage)
		{
			foreach (var buff in buffs)
			{
				buff.UpdateLifeRegen(this, ref damage);
			}
		}
	}
}
