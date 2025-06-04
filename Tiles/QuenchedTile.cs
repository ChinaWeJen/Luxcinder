using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Luxcinder.Tiles
{
	public class Quenched_Tile : ModTile
	{
		public override void SetStaticDefaults()
		{
			// 基础物块属性
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileStone[Type] = true;

			// 挖掘属性
			MineResist = 5f; // 物块被挖掘时受到"伤害"的系数，越大则越难以破坏
			MinPick = 80;   // 能被挖掘需要的最小镐力，默认0
			HitSound = SoundID.Tink; // 物块被挖掘时的声音
			DustType = DustID.Dirt;  // 产生的粒子


			// 发光和地图设置
			Main.tileLighted[Type] = true; // 启用发光效果
			AddMapEntry(new Color(255,99,71), CreateMapEntryName()); 
		}

		// 火焰主题发光设置
		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			// 脉动式火焰色发光效果
			float pulse = (float)System.Math.Sin(Main.GlobalTimeWrappedHourly * 3f) * 0.15f + 0.85f;
			r = 0.8f * pulse;    // 红色分量 (主导)
			g = 0.4f * pulse;    // 橙色分量
			b = 0.1f * pulse;    // 少量蓝色
		}

		// 火焰主题粒子效果
		public override void NumDust(int i, int j, bool fail, ref int num) {
			num = fail ? 4 : 8; // 更多粒子数量
		}

		// 火焰主题破坏效果
		public override bool CreateDust(int i, int j, ref int type)
		{
			// 70%火焰, 20%烟雾, 10%火花
			float rand = Main.rand.NextFloat();
			if (rand < 0.7f)
				type = DustID.Torch;
			else if (rand < 0.9f)
				type = DustID.Smoke;
			else
				type = DustID.Firework_Red;
			
			return true;
		}

		// 世界中的火焰特效
		public override void AnimateTile(ref int frame, ref int frameCounter)
		{
			// 每4帧检查一次是否生成特效
			if (++frameCounter >= 4)
			{
				frameCounter = 0;
				// 只对屏幕内的方块生成特效
				int minX = (int)(Main.screenPosition.X / 16) - 10;
				int maxX = (int)((Main.screenPosition.X + Main.screenWidth) / 16) + 10;
				int minY = (int)(Main.screenPosition.Y / 16) - 10;
				int maxY = (int)((Main.screenPosition.Y + Main.screenHeight) / 16) + 10;
				
				for (int i = minX; i < maxX; i++)
				{
					for (int j = minY; j < maxY; j++)
					{
						if (i >= 0 && j >= 0 && i < Main.maxTilesX && j < Main.maxTilesY && 
							Main.tile[i, j].TileType == Type && Main.rand.NextBool(40))
						{
							// 火焰粒子变体
							int dustType = Main.rand.Next(3) switch {
								0 => DustID.Torch,
								1 => DustID.Firework_Red,
								_ => DustID.FlameBurst
							};

							int dust = Dust.NewDust(
								new Vector2(i * 16 + Main.rand.Next(16), j * 16 + Main.rand.Next(16)),
								6, 6,
								dustType,
								0f, 0f,
								180, default(Color), 1.0f);
							Main.dust[dust].noGravity = true;
							Main.dust[dust].velocity = new Vector2(
								Main.rand.NextFloat(-0.5f, 0.5f), 
								Main.rand.NextFloat(-1.5f, -0.5f));
						}
					}
				}
			}
		}

		// 火焰光晕效果(使用原版特效替代贴图)
		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			// 绘制火焰光晕效果
			Tile tile = Main.tile[i, j];
			if (tile.IsTileInvisible || !Main.SettingsEnabled_TilesSwayInWind)
				return;

			Vector2 position = new Vector2(
				i * 16 - (int)Main.screenPosition.X,
				j * 16 - (int)Main.screenPosition.Y) + new Vector2(8, 8);

			// 使用原版火焰粒子效果
			if (Main.rand.NextBool(3))
			{
				Dust dust = Dust.NewDustPerfect(
					position,
					DustID.Firework_Red,
					Vector2.Zero,
					100, 
					new Color(255, 150, 50), 
					0.7f);
				dust.noLight = true;
				dust.noGravity = true;
			}
		}
	}
}