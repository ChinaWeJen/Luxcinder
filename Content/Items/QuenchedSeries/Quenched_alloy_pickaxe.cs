using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Luxcinder.Content.Items.QuenchedSeries;

namespace Luxcinder.Content.Items.QuenchedSeries
{
	public class Quenched_alloy_pickaxe : ModItem
	{
		private int blocksMined = 0;
		private int cooldown = 0;
		private const int BuffThreshold = 230; // 提高触发门槛
		private const int DefenseBuffAmount = 15; // 增强buff效果
		private const int BuffDuration = 3600;
		private const float CoinDropChance = 0.005f; // 降低掉落概率
		private const int MinCoinAmount = 15;
		private const int MaxCoinAmount = 50;
		private const int CooldownTime = 300; // 5秒冷却(60*5)
		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 16;  // 原14降低14.3%
			Item.DamageType = DamageClass.Melee;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 20;  // 与原版铂金镐一致
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 3;  // 与原版铂金镐一致
			Item.value = Item.buyPrice(gold: 1, silver: 5); // 原价1金50银降价30%
			Item.rare = ItemRarityID.LightRed;  // 匹配铂金套装
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.useTurn = true; // 允许使用转向

			Item.pick = 78;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient<Quenched_Ingot>(12); // 与原版铂金镐材料数量一致
			recipe.AddIngredient(ItemID.Wood, 15); // 添加木材作为辅助材料
			recipe.AddTile(TileID.Anvils)
				.Register();
		}

		public override bool? UseItem(Player player)
		{
			// 冷却时间更新
			if (cooldown > 0)
			{
				cooldown--;
				return base.UseItem(player);
			}

			// 检查玩家是否在挖掘矿物/石头类方块
			if (player.itemAnimation > 0 && player.controlUseItem)
			{
				int tileType = Main.tile[Player.tileTargetX, Player.tileTargetY].TileType;
				if (tileType == TileID.Stone || (tileType >= TileID.Copper && tileType <= TileID.Chlorophyte) ||
					(tileType >= TileID.Amethyst && tileType <= TileID.Diamond))
				{
					// 增加挖掘计数器
					blocksMined++;

					// 检查是否达到buff触发条件
					if (blocksMined >= BuffThreshold)
					{
						player.AddBuff(BuffID.Ironskin, BuffDuration);
						player.statDefense += DefenseBuffAmount;
						blocksMined = 0;
						cooldown = CooldownTime; // 触发冷却

						// 显示buff获得提示
						if (player.whoAmI == Main.myPlayer)
						{
							Main.NewText($"淬火合金镐: 挖掘奖励 - 获得{DefenseBuffAmount}点防御力(持续1分钟)", Color.LightGreen);
						}
					}

					// 检查是否掉落铜币
					if (Main.rand.NextFloat() < CoinDropChance)
					{
						int coinAmount = Main.rand.Next(MinCoinAmount, MaxCoinAmount + 1);
						Item.NewItem(null, player.position, ItemID.CopperCoin, coinAmount);

						if (player.whoAmI == Main.myPlayer)
						{
							Main.NewText($"淬火合金镐: 发现{coinAmount}个铜币!", Color.Orange);
						}
					}
				}

				return base.UseItem(player);
			}
			return null;
		}
	}
}