using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Vitrium.Core;

namespace Vitrium.Buffs
{
	public abstract class VitriBuff : ModBuff, ICloneable
	{
		public string UnderlyingName => base.Name;
		public new virtual string Name => GetType().Name;
		public virtual string Tooltip => "DEFAULT";
		public virtual bool Debuff => false;
		public virtual float Weight => 1f;
		public virtual string Texture => null;

		public override void SetDefaults()
		{
			canBeCleared = false;
#if !DEBUG
			Main.debuff[Type] = true;
#endif
			DisplayName.SetDefault(Name);
			Description.SetDefault(Tooltip);
		}

		public virtual void RealDefaults() { }

		internal bool IApplicableTo(Item item)
		{
			return item.Enchantable() && ApplicableTo(item);
		}

		public virtual bool ApplicableTo(Item item)
		{
			return true;
		}

		public sealed override void ModifyBuffTip(ref string tip, ref int rare)
		{
			tip = Tooltip;
			rare = 100;
		}

		// Useless methods
		public sealed override bool Autoload(ref string name, ref string texture)
		{
			texture = Texture ?? texture;
			return true;
		}

		public sealed override void Update(NPC npc, ref int buffIndex) { base.Update(npc, ref buffIndex); }
		public sealed override void Update(Player player, ref int buffIndex) { base.Update(player, ref buffIndex); }

		public object Clone()
		{
			VitriBuff buff = (VitriBuff)MemberwiseClone();
			buff.canBeCleared = canBeCleared;
			buff.longerExpertDebuff = longerExpertDebuff;
			Clone(ref buff);
			return buff;
		}

		public virtual void Load(Item item, TagCompound tag) { }
		public virtual void Save(Item item, TagCompound tag) { }
		public virtual void NetReceive(Item item, BinaryReader reader) { }
		public virtual void NetSend(Item item, BinaryWriter writer) { }

		public virtual void Clone(ref VitriBuff buff) { }

		// Update methods
		public virtual void ResetEffects(VPlayer player) { }
		public virtual void ResetEffects(VNPC npc) { }
		public virtual void PreUpdate(VPlayer player) { }
		public virtual void PostUpdate(VPlayer player) { }
		public virtual void PostUpdateEquips(VPlayer player) { }
		public virtual void PostUpdateRunSpeeds(VPlayer player) { }
		public virtual void UpdateLifeRegen(VPlayer player) { }
		public virtual void UpdateLifeRegen(VNPC npc, ref int damage) { }
		public virtual void UpdateBadLifeRegen(VPlayer player) { }
		public virtual bool PreNPCLoot(VNPC npc)
		{
			return true;
		}

		public virtual bool PreAI(VNPC npc)
		{
			return true;
		}

		public virtual void AI(VNPC npc) { }
		public virtual void EditSpawnRate(VPlayer player, ref int spawnRate, ref int maxSpawns) { }
		public virtual void OnConsumeAmmo(VPlayer player, Item weapon, Item ammo) { }
		public virtual bool ConsumeAmmo(VPlayer player, Item weapon, Item ammo)
		{
			return true;
		}

		// Collision methods
		public virtual bool PreHurt(VPlayer player, bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
			return true;
		}

		public virtual void OnHitAnything(VPlayer player, float x, float y, Entity victim) { }
		public virtual void ModifyHitByNPC(VPlayer player, NPC npc, ref int damage, ref bool crit) { }
		public virtual void ModifyHitByProjectile(VPlayer player, Projectile proj, ref int damage, ref bool crit) { }
		public virtual void ModifyHitByPvp(VPlayer vp, Player player, ref int damage, ref bool crit) { }
		public virtual void ModifyHitNPC(VPlayer player, NPC target, Item item, ref int damage, ref float knockback, ref bool crit) { }
		public virtual void ModifyHitNPCWithProj(VPlayer player, NPC target, Projectile proj, ref int damage, ref float knockback, ref bool crit, ref int direction) { }
		public virtual void ModifyHitPvp(VPlayer player, Player target, Item item, ref int damage, ref bool crit) { }
		public virtual void ModifyHitPvpWithProj(VPlayer player, Player target, Projectile proj, ref int damage, ref bool crit) { }

		public virtual void ModifyWeaponDamage(VPlayer player, Item item, ref float add, ref float mult, ref float flat) { }
	}
}
