using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.World.Sepulchre
{
	public class SepulchreBrickTwo : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileBlendAll[Type] = true;
			Main.tileBlockLight[Type] = true;

			AddMapEntry(new Color(87, 85, 81));

			HitSound = SoundID.Tink;
			ItemDrop = ModContent.ItemType<Items.Placeable.Tiles.SepulchreBrickTwoItem>();
			DustType = DustID.Wraith;
		}
	}
}
