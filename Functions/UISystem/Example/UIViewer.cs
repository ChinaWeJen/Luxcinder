using Terraria.ID;
using Terraria.ModLoader;

namespace Luxcinder.Functions.UISystem.Example;

public class UIViewer : ModItem
{
	public override void SetDefaults()
	{
		Item.width = 10;
		Item.height = 10;
		Item.useTime = 17;
		Item.useAnimation = 17;
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.noMelee = true;
		Item.noUseGraphic = true;
	}

	public override bool? UseItem(Player player)
	{
		if (player.whoAmI != Main.myPlayer)
			return false;
		ExampleUI.Instance.Activate();
        return true;
	}
}