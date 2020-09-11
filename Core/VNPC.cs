using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
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

		public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Color drawColor)
		{
			bool b = base.PreDraw(npc, spriteBatch, drawColor);

			foreach (VitriBuff buff in buffs)
			{
				b &= buff.PreDraw(this, spriteBatch, drawColor);
			}

			return b;
		}

		public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Color drawColor)
		{
			foreach (VitriBuff buff in buffs)
			{
				buff.PostDraw(this, spriteBatch, drawColor);
			}

			if (npc.HasBuff(BuffID.Frozen))
			{
				Vector2 position = new Vector2((int)(npc.position.X - Main.screenPosition.X + (npc.width / 2f)), (int)(npc.position.Y - Main.screenPosition.Y + (npc.height / 2f)));
				Color color = npc.GetAlpha(Lighting.GetColor((int)(npc.position.X + npc.width * 0.5) / 16, (int)(npc.position.Y + npc.height * 0.5) / 16, Color.White));
				color.R = (byte)(color.R * 0.55);
				color.G = (byte)(color.G * 0.55);
				color.B = (byte)(color.B * 0.55);
				color.A = (byte)(color.A * 0.55);

				spriteBatch.Draw(Main.frozenTexture, position, new Rectangle(0, 0, Main.frozenTexture.Width, Main.frozenTexture.Height), color, npc.rotation, new Vector2(Main.frozenTexture.Width / 2f, Main.frozenTexture.Height / 2f), 1f, SpriteEffects.None, 0f);
			}
		}

		public override bool PreAI(NPC npc)
		{
			bool b = base.PreAI(npc);

			foreach (VitriBuff buff in buffs)
			{
				b &= buff.PreAI(this);
			}

			if (npc.HasBuff(BuffID.Frozen))
			{
				b = false;
				npc.velocity *= 0f;
			}

			return b;
		}

		public override void AI(NPC npc)
		{
			foreach (VitriBuff buff in buffs)
			{
				buff.AI(this);
			}

			if (npc.HasBuff(BuffID.Chilled))
			{
				npc.velocity *= 0.9f;
			}
		}

		public override void PostAI(NPC npc)
		{
			foreach (VitriBuff buff in buffs)
			{
				buff.PostAI(this);
			}
		}

		public override bool PreNPCLoot(NPC npc)
		{
			bool b = base.PreNPCLoot(npc);

			foreach (VitriBuff buff in buffs)
			{
				b &= buff.PreNPCLoot(this);
			}

			return b;
		}

		public override void NPCLoot(NPC npc)
		{
			foreach (VitriBuff buff in buffs)
			{
				buff.NPCLoot(this);
			}

			bufftuples = null;
			buffbuffer = null;
		}

		public override void UpdateLifeRegen(NPC npc, ref int damage)
		{
			foreach (VitriBuff buff in buffs)
			{
				buff.UpdateLifeRegen(this, ref damage);
			}
		}

		public override void ModifyHitPlayer(NPC npc, Player target, ref int damage, ref bool crit)
		{
			foreach (VitriBuff buff in buffs)
			{
				buff.ModifyHitPlayer(this, target, ref damage, ref crit);
			}

			if (npc.HasBuff(BuffID.Frozen))
			{
				damage = 0;
				crit = false;
			}
		}

		public override void ModifyHitNPC(NPC npc, NPC target, ref int damage, ref float knockback, ref bool crit)
		{
			foreach (VitriBuff buff in buffs)
			{
				buff.ModifyHitNPC(this, target, ref damage, ref knockback, ref crit);
			}

			if (npc.HasBuff(BuffID.Frozen))
			{
				damage = 0;
				crit = false;
			}
		}
	}
}
