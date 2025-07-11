using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Luxcinder.Content.Items.TreasureBags
{
	public class SearingLight : ModItem
	{
		public override void SetStaticDefaults() {
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 2;

			Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(8, 4));
			ItemID.Sets.AnimatesAsSoul[Item.type] = true;
			ItemID.Sets.ItemNoGravity[Item.type] = true; // Makes the item have an animation while in world (not held.). Use in combination with RegisterItemAnimation
		}

		public override void SetDefaults() {
			Item.width = 38;
			Item.height = 38;
			Item.value = Item.sellPrice(silver: 1);
			Item.rare = ItemRarityID.Pink;
			Item.maxStack = 999;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTurn = true;
			Item.useAnimation = 15;
		}		public override void AddRecipes() {
			CreateRecipe()
				.Register();
		}
	}
}