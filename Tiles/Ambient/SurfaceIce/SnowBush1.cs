using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ObjectData;
using System.Collections.Generic;
using SpiritMod.Items.Consumable.Food;
using Terraria.GameContent;

namespace SpiritMod.Tiles.Ambient.SurfaceIce;

public class SnowBush1 : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileSolidTop[Type] = false;
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
		TileObjectData.newTile.Height = 2;
		TileObjectData.newTile.Width = 3;
		TileObjectData.newTile.Origin = new Point16(0, 2);
		TileObjectData.newTile.CoordinateHeights = new int[]
		{
			16,
			16
		};
		HitSound = SoundID.Grass;
		TileObjectData.addTile(Type);
		DustType = DustID.GrassBlades;
		TileID.Sets.DisableSmartCursor[Type] = true;
		TileID.Sets.BreakableWhenPlacing[Type] = true;
	}

	public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) => offsetY = 2;

	public override IEnumerable<Item> GetItemDrops(int i, int j)
	{
		if (Main.rand.NextBool(3))
			yield return new Item(ModContent.ItemType<IceBerries>());
	}
}

public class SnowBush1Rubble : SnowBush1
{
	public override string Texture => base.Texture.Replace("Rubble", "");

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();

		FlexibleTileWand.RubblePlacementLarge.AddVariation(ItemID.SnowBlock, Type, 0);
		RegisterItemDrop(ItemID.SnowBlock);
	}

	public override IEnumerable<Item> GetItemDrops(int i, int j) { yield break; }
}