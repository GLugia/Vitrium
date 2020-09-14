using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Vitrium.Buffs;
using Vitrium.Core.Cache;

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

		private IEnumerable<VitriBuff> buffs => BuffCache.GetNPCBuffs(GlobalID);

		public NPC npc { get; private set; }
		public int GlobalID { get; internal set; }
		public Vector2 PreFrozenVelocity { get; private set; }

		public override bool InstancePerEntity => true;
		public override bool CloneNewInstances => true;

		public override void SetDefaults(NPC npc)
		{
			this.npc = npc;
		}

		public override GlobalNPC Clone()
		{
			VNPC clone = (VNPC)base.Clone();
			clone.npc = npc;
			clone.GlobalID = BuffCache.ReserveNPC();
			return clone;
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

				spriteBatch.Draw(Main.frozenTexture, npc.getRect(), new Rectangle(0, 0, Main.frozenTexture.Width, Main.frozenTexture.Height), color, npc.rotation, new Vector2(Main.frozenTexture.Width / 2f, Main.frozenTexture.Height / 2f), SpriteEffects.None, 0f);
			}
		}

		public override bool PreAI(NPC npc)
		{
			bool b = base.PreAI(npc);

			foreach (VitriBuff buff in buffs)
			{
				b &= buff.PreAI(this);
			}

			if (npc.HasBuff(BuffID.Frozen)) // @TODO save old velocity and re-apply it when the npc is unfrozen
			{
				b = false;

				if (PreFrozenVelocity == default)
				{
					PreFrozenVelocity = npc.velocity;
				}

				npc.velocity = Vector2.Zero;

				if (!npc.noGravity)
				{
					npc.velocity.Y++;
				}

				if (Main.netMode != NetmodeID.SinglePlayer)
				{
					NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI, npc.velocity.X, npc.velocity.Y, 0);
				}
			}
			else if (PreFrozenVelocity != default)
			{
				npc.velocity = PreFrozenVelocity;
				PreFrozenVelocity = default;

				if (Main.netMode != NetmodeID.SinglePlayer)
				{
					NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI, npc.velocity.X, npc.velocity.Y, 0);
				}
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
				npc.velocity *= npc.boss ? 0.95f : 0.9f;

				if (Main.netMode != NetmodeID.SinglePlayer)
				{
					NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI, npc.velocity.X, npc.velocity.Y, 0);
				}
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

			BuffCache.DeleteNPC(GlobalID);
			GlobalID = 0;
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

		public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
		{
			bool b = base.CanHitPlayer(npc, target, ref cooldownSlot);

			b &= !npc.HasBuff(BuffID.Darkness) || Main.rand.NextFloat(0f, 1f) < 0.75f / 60f;

			return b;
		}
	}
}
