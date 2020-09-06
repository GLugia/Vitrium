using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Vitrium.Buffs;

namespace Vitrium.Core
{
	public class VPlayer : ModPlayer
	{
		public static VPlayer GetData(Player player) => player.GetModPlayer<VPlayer>();

		internal List<VitriBuff> buffs;
		internal List<VitriBuff> buffbuffer;
		public T GetBuff<T>() where T : VitriBuff => (T)buffs?.FirstOrDefault(a => a.GetType() == typeof(T));

		public void AddBuff(VitriBuff buff, int duration = 2)
		{
			if (buff != null && !buffs.Contains(buff))
			{
				var vanilla = Vitrium.GetVanillaBuff(buff.Name);

				if (vanilla != -1)
				{
					player.AddBuff(vanilla, duration);
				}
				else
				{
					player.AddBuff(buff.Type, duration);
				}

				buffbuffer.Add(buff);
			}
		}

		private void UpdateBuffCache()
		{
			buffs.AddRange(buffbuffer.Where(a => !buffs.Contains(a)));
			buffbuffer.Clear();
		}

		public override void Initialize()
		{
			buffs = new List<VitriBuff>();
			buffbuffer = new List<VitriBuff>();
		}

		public override void SetupStartInventory(IList<Item> items, bool mediumcoreDeath)
		{
			foreach (var item in items)
			{
				if (item != null && item.IsValid() && item.Enchantable())
				{
					var data = VItem.GetData(item);
					var buffs = data.NewBuffs(item);
					data.buff = buffs[Main.rand.Next(0, buffs.Count())];
					data.Hash = Main.rand.NextString();
				}
			}
		}

		public override void ResetEffects()
		{
			UpdateBuffCache();

			foreach (var buff in buffs)
			{
				buff.ResetEffects(this);
			}
		}

		public override void PreUpdate()
		{
			UpdateBuffCache();

			foreach (var buff in buffs)
			{
				buff.PreUpdate(this);
			}
		}

		public override void PostUpdate()
		{
			UpdateBuffCache();

			foreach (var buff in buffs)
			{
				buff.PostUpdate(this);
			}
		}

		public override void UpdateLifeRegen()
		{
			UpdateBuffCache();

			foreach (var buff in buffs)
			{
				buff.UpdateLifeRegen(this);
			}
		}

		public override void UpdateBadLifeRegen()
		{
			UpdateBuffCache();

			foreach (var buff in buffs)
			{
				buff.UpdateBadLifeRegen(this);
			}
		}

		public override void ModifyWeaponDamage(Item item, ref float add, ref float mult, ref float flat)
		{
			UpdateBuffCache();
			item = item.DeepClone();

			foreach (var buff in buffs)
			{
				buff.ModifyWeaponDamage(this, item, ref add, ref mult, ref flat);
			}
		}

		public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
		{
			UpdateBuffCache();

			foreach (var buff in buffs)
			{
				buff.ModifyHitByNPC(this, npc, ref damage, ref crit);
			}
		}

		public override void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit)
		{
			UpdateBuffCache();

			foreach (var buff in buffs)
			{
				buff.ModifyHitByProjectile(this, proj, ref damage, ref crit);
			}
		}

		public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
			UpdateBuffCache();

			bool b = base.PreHurt(pvp, quiet, ref damage, ref hitDirection, ref crit, ref customDamage, ref playSound, ref genGore, ref damageSource);

			foreach (var buff in buffs)
			{
				if (!buff.PreHurt(this, pvp, quiet, ref damage, ref hitDirection, ref crit, ref customDamage, ref playSound, ref genGore, ref damageSource))
				{
					b = false;
				}
			}

			return b;
		}

		public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
		{
			UpdateBuffCache();

			foreach (var buff in buffs)
			{
				buff.ModifyHitNPC(this, target, item, ref damage, ref knockback, ref crit);
			}
		}

		public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			UpdateBuffCache();

			foreach (var buff in buffs)
			{
				buff.ModifyHitNPCWithProj(this, target, proj, ref damage, ref knockback, ref crit, ref hitDirection);
			}
		}

		public override void ModifyHitPvp(Item item, Player target, ref int damage, ref bool crit)
		{
			UpdateBuffCache();

			foreach (var buff in buffs)
			{
				buff.ModifyHitPvp(this, target, item, ref damage, ref crit);
			}
		}

		public override void ModifyHitPvpWithProj(Projectile proj, Player target, ref int damage, ref bool crit)
		{
			UpdateBuffCache();

			foreach (var buff in buffs)
			{
				buff.ModifyHitPvpWithProj(this, target, proj, ref damage, ref crit);
			}
		}

		public override void PostUpdateEquips()
		{
			UpdateBuffCache();

			foreach (var buff in buffs)
			{
				buff.PostUpdateEquips(this);
			}
		}

		public override void PostUpdateRunSpeeds()
		{
			UpdateBuffCache();

			foreach (var buff in buffs)
			{
				buff.PostUpdateRunSpeeds(this);
			}
		}

		public override void OnHitAnything(float x, float y, Entity victim)
		{
			UpdateBuffCache();

			foreach (var buff in buffs)
			{
				buff.OnHitAnything(this, x, y, victim);
			}
		}
	}
}
