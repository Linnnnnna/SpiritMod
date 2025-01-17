﻿using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Terraria.DataStructures;

namespace SpiritMod.Projectiles.Summon.CimmerianStaff
{
	public class CimmerianRedGlyph : ModProjectile, IDrawAdditive
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Red Glyph");
			ProjectileID.Sets.MinionShot[Projectile.type] = true;
			Main.projFrames[Projectile.type] = 4;
        }

		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.WoodenArrowFriendly);
			Projectile.DamageType = DamageClass.Default;
			Projectile.width = 24;
			Projectile.height = 38;
			Projectile.friendly = true;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 40;
            Projectile.alpha = 100;
		}

        private float SineAdd => (float)Math.Sin(AlphaCounter) + 2;
		private float AlphaCounter
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		public override void OnSpawn(IEntitySource source) => Projectile.frame = Main.rand.Next(Main.projFrames[Type]);

		public override void AI()
		{
            Projectile.rotation = 0f;
			AlphaCounter += .095f;

			if (Main.rand.NextBool(15))
            {
                int glyphnum = Main.rand.Next(4);
                DustHelper.DrawDustImage(new Vector2(Projectile.Center.X + Main.rand.Next(-30, 30), Projectile.Center.Y + Main.rand.Next(-30, 30)), 130, 0.05f, "SpiritMod/Effects/DustImages/CimmerianGlyph" + glyphnum, 1f);
            }
            DoDustEffect(Projectile.Center, 34f);

            Projectile.velocity = Vector2.Zero;
        }

        public override void Kill(int timeLeft)
        {
            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center.X, Projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<Fire>(), (int)(Projectile.damage *.66f), Projectile.knockBack, Projectile.owner, 0f, 0f);
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);

            for (int k = 0; k < 40; k++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, 130, Vector2.One.RotatedByRandom(6.28f) * Main.rand.NextFloat(5), 0, default, Main.rand.NextFloat(.4f, .8f));
                d.noGravity = true;
            }
            for (int k = 0; k < 6; k++)
            {
                int glyphnum = Main.rand.Next(4);
                DustHelper.DrawDustImage(new Vector2(Projectile.Center.X + Main.rand.Next(-30, 30), Projectile.Center.Y + Main.rand.Next(-30, 30)), 130, 0.05f, "SpiritMod/Effects/DustImages/CimmerianGlyph" + glyphnum, 1f);
            }
        }

        private static void DoDustEffect(Vector2 position, float distance, float minSpeed = 2f, float maxSpeed = 3f, object follow = null)
        {
            float angle = Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi);
            Vector2 vec = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            Vector2 vel = vec * Main.rand.NextFloat(minSpeed, maxSpeed);

            int dust = Dust.NewDust(position - vec * distance, 0, 0, DustID.Firework_Red);
            Main.dust[dust].noGravity = true;
            Main.dust[dust].scale *= .5f;
            Main.dust[dust].velocity = vel;
            Main.dust[dust].customData = follow;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 94, 94) * SineAdd;

        public void AdditiveCall(SpriteBatch spriteBatch, Vector2 screenPos)
		{
			for (int k = 0; k < Projectile.oldPos.Length; k++) {
				Color color = new Color(255, 255, 200) * 0.75f * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);

				float scale = Projectile.scale * (float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length * 0.2f;
				Texture2D tex = ModContent.Request<Texture2D>("SpiritMod/Textures/Glow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

				spriteBatch.Draw(tex, Projectile.oldPos[k] + Projectile.Size / 2 - screenPos, null, color * .1f, 0, tex.Size() / 2, scale * 5, default, default);
				spriteBatch.Draw(tex, Projectile.oldPos[k] + Projectile.Size / 2 - screenPos, null, color * 0.3f, 0, tex.Size() / 2, scale * 4, default, default);
			}
		}
	}
}
