using SpiritMod.Tiles.Furniture.Paintings;
using Terraria.ID;
using Terraria.ModLoader;
namespace SpiritMod.Items.Placeable.Furniture.Paintings
{
	[Sacrifice(1)]
	public class AdvPainting12 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Drowning Deluge");
			Tooltip.SetDefault("'S. Yaki'");
		}

		public override void SetDefaults()
		{
			Item.width = 22;
			Item.height = 32;
			Item.value = Item.value = Terraria.Item.buyPrice(0, 0, 60, 0);
			Item.rare = ItemRarityID.White;

			Item.maxStack = 99;

			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 10;
			Item.useAnimation = 15;

			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;

			Item.createTile = ModContent.TileType<AdvPainting12Tile>();
		}

	}
}