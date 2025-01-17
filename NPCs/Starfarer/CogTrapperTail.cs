﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.NPCs.Starfarer
{
	public class CogTrapperTail : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Stardancer");
			NPCHelper.BuffImmune(Type, true);
			Main.npcFrameCount[NPC.type] = 1;
			
			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
			{
				Hide = true,
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
		}

		public override void SetDefaults()
		{
			NPC.damage = 15;
			NPC.npcSlots = 0f;
			NPC.width = 14;
			NPC.height = 20;
			NPC.defense = 12;
			NPC.lifeMax = 300;
			NPC.aiStyle = -1;
			NPC.knockBackResist = 0f;
			NPC.alpha = 255;
			NPC.behindTiles = true;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.HitSound = SoundID.NPCHit4;
			NPC.DeathSound = SoundID.NPCDeath14;
			NPC.netAlways = true;
			NPC.killCount[Type] = NPC.killCount[ModContent.NPCType<CogTrapperHead>()];
			NPC.dontCountMe = true;
			NPC.npcSlots = 0;

			Banner = ModContent.NPCType<CogTrapperHead>();
			BannerItem = ModContent.ItemType<Items.Banners.StardancerBanner>();
			AIType = -1;
			AnimationType = 10;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) => bestiaryEntry.UIInfoProvider = new CustomEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[ModContent.NPCType<CogTrapperHead>()], false);
		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => false;

		public override void AI()
		{
			Lighting.AddLight((int)(NPC.Center.X / 16f), (int)(NPC.Center.Y / 16f), 0f, 0.0375f * 2, 0.125f * 2);

			var parent = Main.npc[(int)NPC.ai[1]];

			if (!parent.active || parent.type != ModContent.NPCType<CogTrapperBody>())
			{
				NPC.life = 0;
				NPC.HitEffect(0, 10.0);
				NPC.active = false;
			}

			const int BodyLength = 12;

			if (parent.DistanceSQ(NPC.Center) > BodyLength * BodyLength)
				NPC.velocity = NPC.DirectionTo(parent.Center) * (parent.Distance(NPC.Center) - BodyLength);
			else
				NPC.velocity = Vector2.Zero;

			NPC.rotation = NPC.velocity.ToRotation() + 1.57f;

			if (parent.alpha < 128)
			{
				if (NPC.alpha != 0)
				{
					for (int num934 = 0; num934 < 2; num934++)
					{
						int num935 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Electric, 0f, 0f, 100, default, 2f);
						Main.dust[num935].noGravity = true;
						Main.dust[num935].noLight = true;
					}
				}

				NPC.alpha -= 42;
				if (NPC.alpha < 0)
					NPC.alpha = 0;
			}
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Electric, hitDirection, -1f, 0, default, 1f);
			}
			if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
			{
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Stardancer5").Type, 1f);
				NPC.position.X = NPC.position.X + (float)(NPC.width / 2);
				NPC.position.Y = NPC.position.Y + (float)(NPC.height / 2);
				NPC.width = 20;
				NPC.height = 20;
				NPC.position.X = NPC.position.X - (float)(NPC.width / 2);
				NPC.position.Y = NPC.position.Y - (float)(NPC.height / 2);
				for (int num621 = 0; num621 < 5; num621++)
				{
					int num622 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Electric, 0f, 0f, 100, default, .5f);
					Main.dust[num622].velocity *= 2f;
				}
				for (int num623 = 0; num623 < 10; num623++)
				{
					int num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Electric, 0f, 0f, 100, default, 1f);
					Main.dust[num624].noGravity = true;
					Main.dust[num624].velocity *= 4f;
					num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.DungeonSpirit, 0f, 0f, 100, default, .5f);
					Main.dust[num624].velocity *= 1f;
				}
			}
		}
		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{

			var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY), NPC.frame,
							 drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
			return false;
		}

		public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) => GlowmaskUtils.DrawNPCGlowMask(spriteBatch, NPC, Mod.Assets.Request<Texture2D>("NPCs/Starfarer/CogTrapperTail_Glow").Value, screenPos);

		public override bool CheckActive() => false;

		public override bool PreKill() => false;

		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * bossLifeScale);
			NPC.damage = (int)(NPC.damage * 0.65f);
		}
	}
}