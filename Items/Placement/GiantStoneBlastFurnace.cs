using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Luxcinder.Items.Placement
{
    public class GiantStoneBlastFurnace : ModItem
    {
        public override void SetDefaults()
        {
            // 物品属性设置
            Item.width = 30;
            Item.height = 30;
            Item.maxStack = 99;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.Building.GiantStoneBlastFurnace>();
        }

        public override void AddRecipes()
        {
            // 创建配方
            Recipe recipe = Recipe.Create(Type, 1);
            
            // 添加配方材料
            recipe.AddIngredient(ItemID.StoneBlock, 50);
            recipe.AddIngredient(ItemID.Torch, 10);
            recipe.AddTile(TileID.WorkBenches);
            
            // 注册配方
            recipe.Register();
        }
    }
}
