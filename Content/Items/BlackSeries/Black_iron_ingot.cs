﻿using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Terraria;
using Terraria.Audio;
using Luxcinder.Content.Tiles.Building; // 引入音效命名空间

namespace Luxcinder.Content.Items.BlackSeries
{
    public class Black_iron_ingot : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
            ItemID.Sets.SortingPriorityMaterials[Item.type] = 59; // Influences the inventory sort order. 59 is PlatinumBar, higher is more valuable.
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 999;
            Item.value = 900; // The cost of the item in copper coins. (1 = 1 copper, 100 = 1 silver, 1000 = 1 gold, 10000 = 1 platinum)
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ItemRarityID.Orange;
        }

        // 当玩家捡起物品时触发
        public override bool OnPickup(Player player)
        {
            // 使用富文本格式设置颜色
            string message = $"获得来自 [c/DC143C:蚀世之烬] 的科技树材料";
            Main.NewTextMultiline(message, false, new Microsoft.Xna.Framework.Color(255, 215, 0));

            // 播放原版音效
            SoundEngine.PlaySound(SoundID.Item37, player.position);

            return base.OnPickup(player);
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.IronBar, 1)
            .AddIngredient(ItemID.CopperBar, 2)
            .AddTile<GiantStoneBlastFurnace>()
            // 铁砧Anvil 工作台WorkBenches 熔炉Furnaces 工匠作坊TinkerersWorkbench
            .Register();
        }
    }
}