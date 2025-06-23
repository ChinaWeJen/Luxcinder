using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Terraria;

namespace Luxcinder.Content.Items.Accessory
{
    public class CornerGrass : ModItem
	{
		public override void SetStaticDefaults() {
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
			ItemID.Sets.SortingPriorityMaterials[Item.type] = 59; // Influences the inventory sort order. 59 is PlatinumBar, higher is more valuable.
		}

		public override void SetDefaults() {
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = 1;
			Item.value = 35000; // The cost of the item in copper coins. (1 = 1 copper, 100 = 1 silver, 1000 = 1 gold, 10000 = 1 platinum)
			Item.useStyle = ItemUseStyleID.None;//饰品无法挥动
			Item.useTurn = true;
			Item.useAnimation = 15;
			Item.rare = ItemRarityID.Blue;
			Item.useTime = 10;
			Item.autoReuse = false;
			Item.placeStyle = 0;
			Item.accessory = true;
			}
			public override void UpdateAccessory(Player player, bool hideVisual) {
	player.GetAttackSpeed(MeleeDamageClass.Melee) += 0.05f;
	player.moveSpeed += 0.05f;
			}
		public override void AddRecipes() {
			CreateRecipe()
			
			.AddIngredient(ItemID.Vine,5)
			.AddIngredient(ItemID.AnkletoftheWind,1)
			.AddIngredient(ItemID.Stinger,2)
			.AddTile(TileID.TinkerersWorkbench)//铁砧Anvil 工作台WorkBenches 熔炉Furnaces 工匠作坊TinkerersWorkbench
			.Register();
		}
	}
}


