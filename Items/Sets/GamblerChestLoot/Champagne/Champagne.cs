using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using SpiritMod.Prim;

namespace SpiritMod.Items.Sets.GamblerChestLoot.Champagne
{
	public class Champagne : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Champagne");
			Tooltip.SetDefault("Hold down and release to pop the cork");
		}

		public override void SetDefaults()
		{
			Item.useStyle = 100;
			Item.width = 40;
			Item.height = 32;
			Item.noUseGraphic = false;
			Item.UseSound = SoundID.Item1;
			Item.channel = true;
			Item.noMelee = true;
			Item.useAnimation = 320;
			Item.useTime = 320;
			Item.shootSpeed = 8f;
			Item.value = Item.sellPrice(0, 0, 0, 0);
			Item.rare = ItemRarityID.Blue;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.shoot = ModContent.ProjectileType<ChampagneProj>();
			Item.maxStack = 999;
		}
	}

	public class ChampagneProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Champagne");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 9;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 1;

		}
		public override void SetDefaults()
		{
			Projectile.width = 90;
			Projectile.height = 90;
			Projectile.friendly = false;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.ownerHitCheck = true;
		}

		bool released = false;
		bool stopped = false;
		Vector2 direction;

		private float Charge
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			if (player == Main.LocalPlayer)
			{
				if (Main.MouseWorld.X > player.Center.X)
					player.direction = 1;
				else
					player.direction = -1;
			}

			Projectile.Center = player.Center;

			if (!stopped)
			{
				direction = Vector2.Normalize(Main.MouseWorld - player.Center);
				player.itemAnimation -= (int)((Charge + 50) / 6);

				while (player.itemAnimation < 3)
				{
					SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);

					if (released && Charge < 100)
					{
						Projectile.active = false;
						player.itemAnimation = 2;
						player.itemTime = 2;
						break;
					}
					else
						player.itemAnimation += 320;
				}

				if (Charge < 100)
					Charge++;

				if (Charge == 99)
					SoundEngine.PlaySound(SoundID.NPCDeath7, Projectile.Center);

				if (!player.channel || released)
					released = true;

				if (released)
				{
					int degrees = (int)(((player.itemAnimation) * -0.7) + 55) * player.direction * (int)player.gravDir;

					if (player.direction == 1)
						degrees += 180;
					else
						degrees += 90;

					double radians = degrees * (Math.PI / 180);
					double throwingAngle = direction.ToRotation() + 3.9;

					if (Math.Abs(radians - throwingAngle) < 0.15f && Charge >= 100)
					{
						stopped = true;
						Projectile.timeLeft = 60;
						Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center, direction * (Charge / 6f), ModContent.ProjectileType<ChampagneCork>(), 1, Projectile.knockBack, player.whoAmI);
						Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center + direction * 20, direction * (float)Math.Sqrt(Projectile.timeLeft), ModContent.ProjectileType<ChampagneLiquid>(), Projectile.damage, Projectile.knockBack, player.whoAmI);
					}
				}
			}
			else
			{
				if (Projectile.timeLeft % 2 == 0)
					Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center + direction * 20, direction * (float)Math.Sqrt(Projectile.timeLeft), ModContent.ProjectileType<ChampagneLiquid>(), Projectile.damage, Projectile.knockBack, player.whoAmI);
				
				player.itemAnimation++;

				if (Projectile.timeLeft == 2)
				{
					player.HeldItem.stack -= 1;
					player.itemAnimation = 2;
					Projectile.active = false;
				}
			}

			player.itemTime = player.itemAnimation;
		}

		public override bool PreDraw(ref Color lightColor) => false;
	}

	public class ChampagneCork : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Champagne");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 9;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 8;
			Projectile.height = 8;
			Projectile.friendly = true;
			Projectile.tileCollide = true;
			Projectile.ignoreWater = false;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.aiStyle = 1;
			Projectile.penetrate = 1;
		}

		public override void Kill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
			for (int i = 0; i < 8; i++)
			{
				int dusttype = (Main.rand.NextBool(3)) ? 1 : 7;
				Vector2 velocity = Projectile.velocity.RotatedByRandom(Math.PI / 16) * Main.rand.NextFloat(0.3f);
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dusttype, velocity.X, velocity.Y);
			}
		}
	}

	public class ChampagneLiquid : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Champagne");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 9;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 8;
			Projectile.height = 8;
			Projectile.friendly = false;
			Projectile.tileCollide = true;
			Projectile.ignoreWater = false;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.aiStyle = 1;
			Projectile.penetrate = 1;
			Projectile.alpha = 255;
		}

		bool primsCreated = false;

		public override void AI()
		{
			if (!primsCreated)
			{
				primsCreated = true;
				SpiritMod.primitives.CreateTrail(new ChampagnePrimTrail(Projectile, new Color(250, 214, 165), (int)(2.5f * Projectile.velocity.Length()), Main.rand.Next(15, 25)));
			}
		}
	}
}