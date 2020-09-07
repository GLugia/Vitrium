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
			VNPC ret = npc.GetGlobalNPC<VNPC>();
			ret.npc = npc;
			return ret;
		}

		private List<(VitriBuff buff, int duration)> bufftuples;
		private List<(VitriBuff buff, int duration)> buffbuffer;
		internal IEnumerable<VitriBuff> buffs => bufftuples?.Select(a => a.buff) ?? Enumerable.Empty<VitriBuff>();
		public T GetBuff<T>() where T : VitriBuff
		{
			return (T)buffs?.FirstOrDefault(a => a.GetType() == typeof(T));
		}

		public NPC npc { get; private set; }

		public void AddBuff(VitriBuff buff, int duration = 2)
		{
			if (buff != null && !buffbuffer.Select(a => a.buff).Contains(buff) && !npc.buffImmune[buff.Type])
			{
				buffbuffer.Add((buff, duration));
			}
		}

		internal void ApplyBuffs()
		{
			bufftuples.Clear();
			bufftuples.AddRange(buffbuffer);
			buffbuffer.Clear();

			foreach ((VitriBuff buff, int duration) in bufftuples)
			{
				int vanilla = Vitrium.GetVanillaBuff(buff.Name);

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
			foreach (VitriBuff buff in buffs)
			{
				buff.ResetEffects(this);
			}
		}

		public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
		{
			VPlayer vp = VPlayer.GetData(player);
			foreach (VitriBuff buff in vp.buffs)
			{
				buff.EditSpawnRate(vp, ref spawnRate, ref maxSpawns);
			}
		}

		public override bool PreAI(NPC npc)
		{
			bool b = base.PreAI(npc);

			foreach (VitriBuff buff in buffs)
			{
				b &= buff.PreAI(this);
			}

			return b;
		}

		public override void AI(NPC npc)
		{
			foreach (VitriBuff buff in buffs)
			{
				buff.AI(this);
			}
		}

		public override bool PreNPCLoot(NPC npc)
		{
			foreach (VitriBuff buff in buffs)
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
			foreach (VitriBuff buff in buffs)
			{
				buff.UpdateLifeRegen(this, ref damage);
			}
		}
	}
}
