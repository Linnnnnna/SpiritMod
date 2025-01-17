﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Items.Accessory;
using SpiritMod.Items.Pets;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.NPCs.Phantom
{
	public class Phantom : ModNPC
	{
		int moveSpeed = 0;
		int moveSpeedY = 0;
		float HomeY = 120f;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Phantom");
			Main.npcFrameCount[NPC.type] = 5;
			NPCID.Sets.TrailCacheLength[NPC.type] = 3;
			NPCID.Sets.TrailingMode[NPC.type] = 0;
		}

		public override void SetDefaults()
		{
			NPC.width = 60;
			NPC.height = 48;
			NPC.value = 140;
			NPC.damage = 45;
			NPC.noTileCollide = true;
			NPC.defense = 10;
			NPC.lifeMax = 200;
			NPC.knockBackResist = 0.45f;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;

			Banner = NPC.type;
			BannerItem = ModContent.ItemType<Items.Banners.PhantomBanner>();
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
				new FlavorTextBestiaryInfoElement("A typical cave bat, transformed into pure darkness by ancient magicks. When night falls, they take to the skies to feed on those unfortunate enough to still be awake."),
			});
		}

		bool trailbehind;
		bool noise;

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
			=> spawnInfo.Player.ZoneOverworldHeight && Main.hardMode && !Main.dayTime && !spawnInfo.Player.ZoneSnow && !spawnInfo.Player.ZoneCorrupt && 
			!spawnInfo.Player.ZoneCrimson && !spawnInfo.Player.ZoneHallow && !spawnInfo.PlayerInTown && !Main.pumpkinMoon && !Main.snowMoon ? 0.015f : 0f;

		public override bool PreAI()
		{
			NPC.TargetClosest(true);
			NPC.spriteDirection = NPC.direction;
			Player player = Main.player[NPC.target];

			if (NPC.Center.X >= player.Center.X && moveSpeed >= -30) // flies to players x position
				moveSpeed--;
			if (NPC.Center.X <= player.Center.X && moveSpeed <= 30)
				moveSpeed++;

			NPC.velocity.X = moveSpeed * 0.18f;

			if (NPC.Center.Y >= player.Center.Y - HomeY && moveSpeedY >= -27) //Flies to players Y position
			{
				moveSpeedY--;
				HomeY = 220f;
			}

			if (NPC.Center.Y <= player.Center.Y - HomeY && moveSpeedY <= 27)
				moveSpeedY++;

			NPC.velocity.Y = moveSpeedY * 0.12f;
			if (player.velocity.Y != 0)
			{
				HomeY = -60f;
				trailbehind = true;
				NPC.velocity.Y = moveSpeedY * 0.16f;
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.ShadowbeamStaff, 0f, -2.5f, 0, default, 0.6f);
				if (!noise)
				{
					SoundEngine.PlaySound(SoundID.Zombie7, NPC.Center);
					noise = true;
				}
				NPC.rotation = NPC.velocity.X * .1f;
			}
			else
			{
				if (HomeY < 220f)
					HomeY += 8f;
				NPC.rotation = 0f;
				noise = false;
				trailbehind = false;
			}

			if (Main.dayTime)
			{
				SoundEngine.PlaySound(SoundID.NPCDeath6, NPC.Center);
				Gore.NewGore(NPC.GetSource_FromAI(), NPC.position, NPC.velocity, 99);
				Gore.NewGore(NPC.GetSource_FromAI(), NPC.position, NPC.velocity, 99);
				Gore.NewGore(NPC.GetSource_FromAI(), NPC.position, NPC.velocity, 99);
				NPC.active = false;
			}
			return true;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PhantomEgg>(), 12));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ShadowSingeFang>(), 22));
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, NPC.GetNPCColorTintedByBuffs(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

			if (!NPC.IsABestiaryIconDummy)
			{
				Vector2 drawOrigin = new Vector2(TextureAssets.Npc[NPC.type].Value.Width * 0.5f, (NPC.height / Main.npcFrameCount[NPC.type]) * 0.5f);
				for (int k = 0; k < NPC.oldPos.Length; k++)
				{
					Vector2 drawPos = NPC.oldPos[k] - screenPos + drawOrigin + new Vector2(0f, NPC.gfxOffY);
					Color color = NPC.GetAlpha(drawColor) * (float)(((NPC.oldPos.Length - k) / (float)NPC.oldPos.Length) / 2);
					spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, drawPos, NPC.frame, color, NPC.rotation, drawOrigin, NPC.scale, effects, 0f);
				}
			}
			return false;
		}

		public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			if (trailbehind)
				GlowmaskUtils.DrawNPCGlowMask(spriteBatch, NPC, Mod.Assets.Request<Texture2D>("NPCs/Phantom/Phantom_Glow").Value, screenPos);
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 20; k++)
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hitDirection * 2, -1f, 0, default, 1f);

			if (trailbehind)
				for (int k = 0; k < 20; k++)
					Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.ShadowbeamStaff, hitDirection * 2, -1f, 0, default, 1f);

			if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
			{
				for (int k = 0; k < 20; k++)
					Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.ShadowbeamStaff, hitDirection * 2, -1f, 0, default, 1f);
				for (int k = 0; k < 20; k++)
					Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hitDirection * 2, -1f, 0, default, 1f);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Phantom1").Type, .5f);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Phantom2").Type, .5f);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Phantom2").Type, .5f);
			}
		}

		public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter += .25f;
			NPC.frameCounter %= Main.npcFrameCount[NPC.type];
			int frame = (int)NPC.frameCounter;
			NPC.frame.Y = frame * frameHeight;
		}
	}
}