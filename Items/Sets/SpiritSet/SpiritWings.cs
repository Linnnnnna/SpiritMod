using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.SpiritSet
{
	[AutoloadEquip(EquipType.Wings)]
	public class SpiritWings : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Spirit Wings");
			Tooltip.SetDefault("Allows for flight and slow fall.");

			ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new Terraria.DataStructures.WingStats(90, 7f, 2);
		}

		public override void SetDefaults()
		{
			Item.width = 47;
			Item.height = 37;
			Item.value = 60000;
			Item.rare = ItemRarityID.Pink;
			Item.accessory = true;
			Item.rare = ItemRarityID.Pink;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.wingTimeMax = 150;

			if (!hideVisual || player.velocity.Y != 0)
			{
				if (Main.rand.NextBool(4))
				{
					Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.UnusedWhiteBluePurple, 0, 0, 100, default, 1f);
					dust.noGravity = true;
					dust.velocity = player.velocity * .5f;
				}
			}
		}

		public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
		{
			ascentWhenFalling = 0.75f;
			ascentWhenRising = 0.11f;
			maxCanAscendMultiplier = 1f;
			maxAscentMultiplier = 2.6f;
			constantAscend = 0.135f;
		}

		public override void AddRecipes()
		{
			Recipe modRecipe = CreateRecipe(1);
			modRecipe.AddIngredient(ModContent.ItemType<SpiritBar>(), 14);
			modRecipe.AddIngredient(ItemID.SoulofFlight, 12);
			modRecipe.AddTile(TileID.MythrilAnvil);
			modRecipe.Register();
		}
	}
}