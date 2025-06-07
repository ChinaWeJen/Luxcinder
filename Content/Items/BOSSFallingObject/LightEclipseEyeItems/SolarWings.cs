// using Microsoft.Xna.Framework;
// using Terraria;
// using Terraria.ID;
// using Terraria.ModLoader;

// namespace Luxcinder.Content.Items.LightEclipseEyeItems
// {
//     public class SolarWings : ModItem
//     {


//         public override void SetDefaults()
//         {
//             Item.width = 34;
//             Item.height = 34;
//             Item.useTime = 20;
//             Item.useAnimation = 20;
//             Item.useStyle = ItemUseStyleID.HoldUp;
//             Item.value = Item.sellPrice(0, 5);
//             Item.rare = ItemRarityID.Pink;
//             Item.UseSound = SoundID.Item25;
//             Item.noMelee = true;
//             Item.mountType = ModContent.MountType<SolarWingsMount>();
//         }
//     }

//     public class SolarWingsMount : ModMount
//     {
//         public override void SetStaticDefaults()
//         {
//             MountData.spawnDust = DustID.GoldFlame;
//             MountData.buff = ModContent.BuffType<SolarWingsBuff>();
//             MountData.heightBoost = 20;
//             MountData.flightTimeMax = 300;
//             MountData.fatigueMax = 0;
//             MountData.fallDamage = 0f;
//             MountData.usesHover = true;
//             MountData.runSpeed = 8f;
//             MountData.dashSpeed = 8f;
//             MountData.acceleration = 0.16f;
//             MountData.jumpHeight = 10;
//             MountData.jumpSpeed = 6f;
//             MountData.blockExtraJumps = false;
//             MountData.totalFrames = 4;
//             MountData.constantJump = true;

//             int[] array = new int[MountData.totalFrames];
//             for (int l = 0; l < array.Length; l++)
//             {
//                 array[l] = 28;
//             }
//             MountData.playerYOffsets = array;
//             MountData.xOffset = 0;
//             MountData.bodyFrame = 3;
//             MountData.yOffset = 0;
//             MountData.playerHeadOffset = 0;

//             MountData.textureWidth = 128;
//             MountData.textureHeight = 80;
//         }

//         public override void UpdateEffects(Player player)
//         {
//             // 飞行时产生神圣粒子
//             if (Main.rand.NextBool(5))
//             {
//                 Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, 
//                     DustID.GoldFlame, 0f, 0f, 100, default, 1.5f);
//                 dust.noGravity = true;
//                 dust.velocity *= 0.5f;
//             }
//         }
//     }

//     public class SolarWingsBuff : ModBuff
//     {
//         public override void SetStaticDefaults()
//         {

//             Main.buffNoTimeDisplay[Type] = true;
//             Main.buffNoSave[Type] = true;
//         }

//         public override void Update(Player player, ref int buffIndex)
//         {
//             player.mount.SetMount(ModContent.MountType<SolarWingsMount>(), player);
//             player.buffTime[buffIndex] = 10;
//         }
//     }
// }