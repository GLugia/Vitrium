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
		public static VPlayer GetData(Player player)
		{
			return player.GetModPlayer<VPlayer>();
		}

		private List<(VitriBuff buff, int duration)> bufftuples;
		private List<(VitriBuff buff, int duration)> buffbuffer;
		internal IEnumerable<VitriBuff> buffs => bufftuples?.Select(a => a.buff) ?? Enumerable.Empty<VitriBuff>();
		public T GetBuff<T>() where T : VitriBuff
		{
			return (T)buffs?.FirstOrDefault(a => a.GetType() == typeof(T));
		}

		public void AddBuff(VitriBuff buff, int duration = 2)
		{
			if (buff != null && !buffbuffer.Select(a => a.buff).Contains(buff) && !player.buffImmune[buff.Type])
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
					player.AddBuff(vanilla, duration);
				}
				else
				{
					player.AddBuff(buff.Type, duration);
				}
			}
		}

		public override void Initialize()
		{
			bufftuples = new List<(VitriBuff, int)>();
			buffbuffer = new List<(VitriBuff, int)>();
		}

		public override void SetupStartInventory(IList<Item> items, bool mediumcoreDeath)
		{
			foreach (Item item in items)
			{
				if (item != null && item.IsValid() && item.Enchantable())
				{
					VItem data = VItem.GetData(item);
					VitriBuff[] buffs = data.NewBuffs(item);
					data.buff = buffs[Main.rand.Next(0, buffs.Count())];
					data.Hash = Main.rand.NextString();
				}
			}
		}

		public override void ResetEffects()
		{
			foreach (VitriBuff buff in buffs)
			{
				buff.ResetEffects(this);
			}
		}

		public override void PreUpdate()
		{
			foreach (VitriBuff buff in buffs)
			{
				buff.PreUpdate(this);
			}
		}

		public override void PostUpdate()
		{
			foreach (VitriBuff buff in buffs)
			{
				buff.PostUpdate(this);
			}
		}

		public override void UpdateLifeRegen()
		{
			foreach (VitriBuff buff in buffs)
			{
				buff.UpdateLifeRegen(this);
			}
		}

		public override void UpdateBadLifeRegen()
		{
			foreach (VitriBuff buff in buffs)
			{
				buff.UpdateBadLifeRegen(this);
			}
		}

		public override void ModifyWeaponDamage(Item item, ref float add, ref float mult, ref float flat)
		{
			item = item.DeepClone();

			foreach (VitriBuff buff in buffs)
			{
				buff.ModifyWeaponDamage(this, item, ref add, ref mult, ref flat);
			}
		}

		public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
		{
			foreach (VitriBuff buff in buffs)
			{
				buff.ModifyHitByNPC(this, npc, ref damage, ref crit);
			}
		}

		public override void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit)
		{
			foreach (VitriBuff buff in buffs)
			{
				buff.ModifyHitByProjectile(this, proj, ref damage, ref crit);
			}
		}

		public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
			bool b = base.PreHurt(pvp, quiet, ref damage, ref hitDirection, ref crit, ref customDamage, ref playSound, ref genGore, ref damageSource);

			foreach (VitriBuff buff in buffs)
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
			foreach (VitriBuff buff in buffs)
			{
				buff.ModifyHitNPC(this, target, item, ref damage, ref knockback, ref crit);
			}
		}

		public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			foreach (VitriBuff buff in buffs)
			{
				buff.ModifyHitNPCWithProj(this, target, proj, ref damage, ref knockback, ref crit, ref hitDirection);
			}
		}

		public override void ModifyHitPvp(Item item, Player target, ref int damage, ref bool crit)
		{
			foreach (VitriBuff buff in buffs)
			{
				buff.ModifyHitPvp(this, target, item, ref damage, ref crit);
			}
		}

		public override void ModifyHitPvpWithProj(Projectile proj, Player target, ref int damage, ref bool crit)
		{
			foreach (VitriBuff buff in buffs)
			{
				buff.ModifyHitPvpWithProj(this, target, proj, ref damage, ref crit);
			}
		}

		public override void PostUpdateEquips()
		{
			foreach (VitriBuff buff in buffs)
			{
				buff.PostUpdateEquips(this);
			}
		}

		public override void PostUpdateRunSpeeds()
		{
			foreach (VitriBuff buff in buffs)
			{
				buff.PostUpdateRunSpeeds(this);
			}
		}

		public override void OnHitAnything(float x, float y, Entity victim)
		{
			foreach (VitriBuff buff in buffs)
			{
				buff.OnHitAnything(this, x, y, victim);
			}
		}

		public override void OnConsumeAmmo(Item weapon, Item ammo)
		{
			foreach (VitriBuff buff in buffs)
			{
				buff.OnConsumeAmmo(this, weapon, ammo);
			}
		}

		public override bool ConsumeAmmo(Item weapon, Item ammo)
		{
			bool b = base.ConsumeAmmo(weapon, ammo);

			foreach (VitriBuff buff in buffs)
			{
				b &= buff.ConsumeAmmo(this, weapon, ammo);
			}

			return b;
		}
	}
}
