
using Luxcinder.Content.Tiles.Building;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Luxcinder.Content.Items.BlackSeries
{
	public class BlackIronAxe
	 : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 14;  // 原16降低12.5%
			Item.DamageType = DamageClass.Melee;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 20;  // 与原版铂金斧一致
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 4;  // 原5降低20%
			Item.value = Item.buyPrice(gold: 0, silver: 70); // 原价1金降价30%
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.useTurn = true; // 允许使用转向

			Item.axe = 55; // 斧力数值(铂金斧35的2倍)
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient<BlackIronIngot>(8); // 与原版铂金斧材料数量一致
			recipe.AddIngredient(ItemID.Wood, 3); // 添加木材作为辅助材料
			recipe.AddTile<GiantStoneBlastFurnace>()
				.Register();
		}
	}
}
