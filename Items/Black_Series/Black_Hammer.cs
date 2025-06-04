using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Luxcinder.Items.Black_Series;

namespace Luxcinder.Items.Black_Series
{
	public class Black_Hammer : ModItem
	{
		public override void SetStaticDefaults() {

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() {
			Item.damage = 13;  // 原15降低13.3%
			Item.DamageType = DamageClass.Melee;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 20;  // 与原版铂金锤一致
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 6;  // 原7降低14.3%
			Item.value = Item.buyPrice(gold: 0, silver: 70); // 原价1金降价30%
			Item.rare = ItemRarityID.LightRed;  // 匹配铂金套装
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.useTurn = true; // 允许使用转向

			Item.hammer = 75; // 锤力数值(铂金锤35的2倍)
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient<Black_iron_ingot>(10); // 与原版铂金锤材料数量一致
			recipe.AddIngredient(ItemID.Wood, 3); // 添加木材作为辅助材料
			recipe.AddTile(TileID.Anvils)
				.Register();
		}
	}
}