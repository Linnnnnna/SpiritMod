using Microsoft.Xna.Framework;
using SpiritMod.Items.Placeable.Tiles;
using SpiritMod.Projectiles;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Tiles.Block
{
	public class Spiritsand : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileBlendAll[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileLighted[Type] = true;

			TileID.Sets.CanBeDugByShovel[Type] = true;
			TileID.Sets.Conversion.Sand[Type] = true;
			TileID.Sets.Falling[Type] = true;
			TileID.Sets.TouchDamageSands[Type] = 15;
			TileID.Sets.ForAdvancedCollision.ForSandshark[Type] = true;

			AddMapEntry(new Color(135, 206, 235));
			ItemDrop = ModContent.ItemType<SpiritSandItem>();
			DustType = DustID.Water_Space;
		}

		public override bool CanExplode(int i, int j) => true;
        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            if (WorldGen.noTileActions)
                return true;

            Tile above = Main.tile[i, j - 1];
            Tile below = Main.tile[i, j + 1];
            bool canFall = true;

            if (below == null || below.HasTile)
                canFall = false;

            if (above.HasTile && (TileID.Sets.BasicChest[above.TileType] || TileID.Sets.BasicChestFake[above.TileType] || above.TileType == TileID.PalmTree || TileID.Sets.BasicDresser[above.TileType]))
                canFall = false;

            if (canFall)
            {
                int projectileType = ModContent.ProjectileType<SpiritSand>();
                float positionX = i * 16 + 8;
                float positionY = j * 16 + 8;

                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Main.tile[i, j].ClearTile();
                    int proj = Projectile.NewProjectile(new Terraria.DataStructures.EntitySource_TileBreak(i, j), positionX, positionY, 0f, 0.41f, projectileType, 10, 0f, Main.myPlayer);
                    Main.projectile[proj].ai[0] = 1f;
                    WorldGen.SquareTileFrame(i, j);
                }
                else if (Main.netMode == NetmodeID.Server)
                {
					Tile tile = Main.tile[i, j];
					tile.HasTile = false;
                    bool spawnProj = true;

                    for (int k = 0; k < 1000; k++)
                    {
                        Projectile otherProj = Main.projectile[k];

                        if (otherProj.active && otherProj.owner == Main.myPlayer && otherProj.type == projectileType && Math.Abs(otherProj.timeLeft - 3600) < 60 && otherProj.Distance(new Vector2(positionX, positionY)) < 4f)
                        {
                            spawnProj = false;
                            break;
                        }
                    }

                    if (spawnProj)
                    {
                        int proj = Projectile.NewProjectile(new Terraria.DataStructures.EntitySource_TileBreak(i, j), positionX, positionY, 0f, 2.5f, projectileType, 10, 0f, Main.myPlayer);
                        Main.projectile[proj].velocity.Y = 0.5f;
                        Main.projectile[proj].position.Y += 2f;
                        Main.projectile[proj].netUpdate = true;
                    }

                    NetMessage.SendTileSquare(-1, i, j, 1);
                    WorldGen.SquareTileFrame(i, j);
                }
                return false;
            }
            return true;
        }
    }
}

