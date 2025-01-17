using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Items.Consumable;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using SpiritMod.Biomes.Events;

namespace SpiritMod.NPCs.MoonjellyEvent
{
	public class DreamlightJelly : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Dreamlight Jelly");
			Main.npcFrameCount[NPC.type] = 6;
			Main.npcCatchable[NPC.type] = true;
			NPCID.Sets.CountsAsCritter[Type] = true;
		}

		public override void SetDefaults()
		{
			NPC.width = 12;
			NPC.height = 20;
            NPC.rarity = 3;
            NPC.damage = 0;
			NPC.defense = 0;
			NPC.lifeMax = 5;
			NPC.HitSound = SoundID.NPCHit25;
			NPC.DeathSound = SoundID.NPCDeath28;
            NPC.value = 0f;
			NPC.catchItem = (short)ModContent.ItemType<DreamlightJellyItem>();
			NPC.knockBackResist = .45f;
			NPC.aiStyle = 64;
            NPC.scale = 1f;
			NPC.noGravity = true;
            NPC.noTileCollide = true;
			AIType = NPCID.Firefly;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<JellyDelugeBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				new FlavorTextBestiaryInfoElement("A tiny Lunazoa that emits a gorgeous, dreamlike green glow. A genetic mutation is the source of their chartreuse gleam, but also prevents them from ever maturing."),
			});
		}

		public override bool? CanBeHitByProjectile(Projectile projectile) => projectile.minion ? false : null;

		public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 15; k++)
            {
                Dust d = Dust.NewDustPerfect(NPC.Center, 226, Vector2.One.RotatedByRandom(6.28f) * Main.rand.NextFloat(2), 0, default, 0.65f);
                d.noGravity = true;
            }
        }
        float alphaCounter;
        public override void AI()
        {
            NPC.rotation = NPC.velocity.X * .15f;
            NPC.spriteDirection = NPC.direction;
            alphaCounter += .04f;
               Lighting.AddLight(NPC.Center, 0.075f * 2, 0.231f * 2, 0.255f * 2);
        }
        public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter += 0.15f;
			NPC.frameCounter %= Main.npcFrameCount[NPC.type];
			int frame = (int)NPC.frameCounter;
			NPC.frame.Y = frame * frameHeight;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot) => npcLoot.AddCommon(ItemID.Gel, 1);

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			drawColor = NPC.GetNPCColorTintedByBuffs(drawColor);
			float sineAdd = (float)Math.Sin(alphaCounter) + 3;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;
            int xpos = (int)((NPC.Center.X) - screenPos.X + 16) - (int)(TextureAssets.Npc[NPC.type].Value.Width / 2);
            int ypos = (int)((NPC.Center.Y) - screenPos.Y + 10) - (int)(TextureAssets.Npc[NPC.type].Value.Width / 2);
            Texture2D ripple = Mod.Assets.Request<Texture2D>("Effects/Masks/Extra_49").Value;
            Main.spriteBatch.Draw(ripple, new Vector2(xpos, ypos), null, new Color((int)(18f * sineAdd), (int)(25f * sineAdd), (int)(20f * sineAdd), 0), NPC.rotation, ripple.Size() / 2f, .5f, spriteEffects, 0);

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Main.spriteBatch.Draw(
                Mod.Assets.Request<Texture2D>("NPCs/MoonjellyEvent/DreamlightJelly_Glow").Value,
				NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY),
				NPC.frame,
				Color.White,
				NPC.rotation,
				NPC.frame.Size() / 2,
				NPC.scale,
				SpriteEffects.None,
				0
			);
            
        }
    }
}
