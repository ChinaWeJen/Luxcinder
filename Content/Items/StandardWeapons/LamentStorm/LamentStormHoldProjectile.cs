using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Luxcinder.Content.Items.StandardWeapons.LamentStorm.Projectiles;
using Mono.Cecil;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Luxcinder.Content.Items.StandardWeapons.LamentStorm;
public class LamentStormHoldProjectile : ModProjectile
{
	// 蓄力相关参数
	private const int MaxChargeTime = 150; // 最大蓄力帧数（1秒）
	private const int MinChargeTime = 10; // 最小蓄力帧数
	private const float MinVelocity = 8f;
	private const float MaxVelocity = 20f;
	private const float MinDamageScale = 0.5f;
	private const float MaxDamageScale = 2.0f;
	private List<Vector2> _relativeArrows = new List<Vector2>();
	private float _currentBowStretchLength;
	private Asset<Texture2D> _textureGlow;

	public override void SetStaticDefaults()
	{
		// 设置弹幕的显示名称和描述
		Main.projFrames[Projectile.type] = 4;
	}

	public override void SetDefaults()
	{
		Projectile.width = 14;
		Projectile.height = 14;
		Projectile.friendly = true;
		Projectile.DamageType = DamageClass.Ranged;
		Projectile.penetrate = 1;
		Projectile.timeLeft = 600;
		Projectile.aiStyle = 0;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
	}

	public override void AI()
	{
		Player player = Main.player[Projectile.owner];
		LamentStormPlayer lamentStormPlayer = player.GetModPlayer<LamentStormPlayer>();

		// 让弹幕跟随玩家手部
		Projectile.Center = player.MountedCenter;

		// 方向朝向鼠标
		Vector2 toMouse = Main.MouseWorld - Projectile.Center;

		if (lamentStormPlayer.LamentStormAttackMode == LamentStormAttackType.Fall)
		{
			float h = MathHelper.Clamp(1600 / Math.Abs(toMouse.X), 8f, 1000f);
			toMouse = new Vector2(Math.Sign(toMouse.X), -h);
		}
		float rot = toMouse.ToRotation();
		Projectile.rotation = rot;

		if ((player.channel || (lamentStormPlayer.LamentStormAttackMode == LamentStormAttackType.Charge && Main.mouseRight)) 
			&& !player.noItems && !player.CCed)
		{
			if (lamentStormPlayer.LamentStormAttackMode == LamentStormAttackType.Charge)
			{
				if (Projectile.localAI[0] == 0)
				{
					_relativeArrows.Clear();
				}
				// 记录蓄力时间
				Projectile.localAI[0]++;
			}

			// 播放拉弓动画
			player.itemTime = 2;
			player.itemAnimation = 2;
			player.heldProj = Projectile.whoAmI;
			player.itemRotation = (float)System.Math.Atan2(toMouse.Y * player.direction, toMouse.X * player.direction);
			player.ChangeDir((Main.MouseWorld.X > player.Center.X) ? 1 : -1);

			Player.CompositeArmStretchAmount armStretchAmount = Player.CompositeArmStretchAmount.Full;
			Projectile.frame = 0;

			if (lamentStormPlayer.LamentStormAttackMode != LamentStormAttackType.Charge)
			{
				if (Projectile.localAI[1] > player.itemAnimationMax + 5)
				{
					armStretchAmount = Player.CompositeArmStretchAmount.Full;
					Projectile.frame = 0;
				}
				else if (Projectile.localAI[1] > player.itemAnimationMax)
				{
					armStretchAmount = Player.CompositeArmStretchAmount.Quarter;
					Projectile.frame = 2;
				}
				else if (Projectile.localAI[1] > player.itemAnimationMax * 0.75f)
				{
					armStretchAmount = Player.CompositeArmStretchAmount.None;
					Projectile.frame = 3;
				}
				else if (Projectile.localAI[1] > player.itemAnimationMax * 0.5f)
				{
					armStretchAmount = Player.CompositeArmStretchAmount.Quarter;
					Projectile.frame = 2;
				}
				else if (Projectile.localAI[1] > player.itemAnimationMax * 0.25f)
				{
					armStretchAmount = Player.CompositeArmStretchAmount.ThreeQuarters;
					Projectile.frame = 1;
				}

				if (Projectile.localAI[1] == 0)
				{
					_relativeArrows.Clear();
					// 平射模式 - 3-5只箭散射
					// 从天而降模式 4-8只
					int arrowCount = lamentStormPlayer.LamentStormAttackMode == LamentStormAttackType.Fall ? Main.rand.Next(4, 9) : Main.rand.Next(3, 6);
					for (int i = 0; i < arrowCount; i++)
					{
						float scatterRange = lamentStormPlayer.LamentStormAttackMode == LamentStormAttackType.Fall ? 20 : 45;
						Vector2 points = Vector2.UnitX.RotatedByRandom(MathHelper.ToRadians(scatterRange)) * 8f;
						_relativeArrows.Add(points);
					}
				}
				Projectile.localAI[1]++;
				if (Projectile.localAI[1] == player.itemAnimationMax)
				{
					if (lamentStormPlayer.LamentStormAttackMode == LamentStormAttackType.Fall)
					{
						// 发射虚假的箭矢弹幕
						Vector2 shootVel = toMouse.SafeNormalize(Vector2.UnitX) * 10;

						for (int i = 0; i < _relativeArrows.Count; i++)
						{
							Vector2 perturbedSpeed = ((_relativeArrows[i] - new Vector2(-_currentBowStretchLength, 0)).ToRotation() + rot).ToRotationVector2();
							perturbedSpeed *= Main.rand.NextFloat(0.8f, 1.2f) * 24;

							var proj = Projectile.NewProjectile(
								Projectile.GetSource_FromThis(),
								Projectile.Center,
								perturbedSpeed,
								ModContent.ProjectileType<LamentStormPDummy>(),
								Projectile.damage * 2,
								Projectile.knockBack,
								Projectile.owner
							);
							Main.projectile[proj].ai[0] = Main.MouseWorld.X;
							Main.projectile[proj].ai[1] = Main.MouseWorld.Y;
						}
					}
					else
					{
						// 发射真正的箭矢弹幕
						Vector2 shootVel = toMouse.SafeNormalize(Vector2.UnitX) * 10;

						for (int i = 0; i < _relativeArrows.Count; i++)
						{
							Vector2 perturbedSpeed = ((_relativeArrows[i] - new Vector2(-_currentBowStretchLength, 0)).ToRotation() + rot).ToRotationVector2();
							perturbedSpeed *= Main.rand.NextFloat(0.8f, 1.2f) * 18;

							Projectile.NewProjectile(
								Projectile.GetSource_FromThis(),
								Projectile.Center,
								perturbedSpeed,
								ModContent.ProjectileType<LamentStormP>(),
								Projectile.damage * 2,
								Projectile.knockBack,
								Projectile.owner
							);
						}
					}
				}
				else if (Projectile.localAI[1] > player.itemAnimationMax && Projectile.localAI[1] < player.itemAnimationMax + 20)
				{

				}
				else if (Projectile.localAI[1] >= player.itemAnimationMax + 20)
				{
					Projectile.localAI[1] = 0;
				}
			}
			else
			{
				if (Projectile.localAI[0] > MaxChargeTime * 0.75)
				{
					armStretchAmount = Player.CompositeArmStretchAmount.None;
					Projectile.frame = 3;
				}
				else if (Projectile.localAI[0] > MaxChargeTime * 0.5f)
				{
					armStretchAmount = Player.CompositeArmStretchAmount.Quarter;
					Projectile.frame = 2;
				}
				else if (Projectile.localAI[0] > MaxChargeTime * 0.25f)
				{
					armStretchAmount = Player.CompositeArmStretchAmount.ThreeQuarters;
					Projectile.frame = 1;
				}
				else
				{
					armStretchAmount = Player.CompositeArmStretchAmount.Full;
					Projectile.frame = 0;
				}

				if (Projectile.localAI[0] == 1)
				{
					for (int i = 0; i < 5; i++)
					{
						Vector2 points = Vector2.UnitX.RotatedByRandom(MathHelper.ToRadians(45)) * 8f;
						_relativeArrows.Add(points);
					}
				}

				if (Projectile.localAI[0] % 15 == 0 && _relativeArrows.Count < 15)
				{
					Vector2 points = Vector2.UnitX.RotatedByRandom(MathHelper.ToRadians(45)) * 8f;
					_relativeArrows.Add(points);
				}
			}
			player.SetCompositeArmFront(true, armStretchAmount, rot - 1.5707964f);
			switch (armStretchAmount)
			{
				case Player.CompositeArmStretchAmount.Full:
					_currentBowStretchLength = 8f;
					break;
				case Player.CompositeArmStretchAmount.None:
					_currentBowStretchLength = 18f;

					break;
				case Player.CompositeArmStretchAmount.Quarter:
					_currentBowStretchLength = 14f;
					break;
				case Player.CompositeArmStretchAmount.ThreeQuarters:
					_currentBowStretchLength = 10f;
					break;
				default:
					break;
			}
		}
		else
		{
			if (lamentStormPlayer.LamentStormAttackMode == LamentStormAttackType.Charge)
			{
				// 松开鼠标时发射
				int charge = (int)Projectile.localAI[0];
				float chargePercent = Utils.Clamp(charge / (float)MaxChargeTime, 0f, 1f);

				float velocity = MathHelper.Lerp(MinVelocity, MaxVelocity, chargePercent);
				float damageScale = MathHelper.Lerp(MinDamageScale, MaxDamageScale, chargePercent);

				// 发射真正的箭矢弹幕
				for (int i = 0; i < _relativeArrows.Count; i++)
				{
					Vector2 perturbedSpeed = ((_relativeArrows[i] - new Vector2(-_currentBowStretchLength, 0)).ToRotation() + rot).ToRotationVector2();
					perturbedSpeed *= Main.rand.NextFloat(0.8f, 1.2f) * velocity;

					Projectile.NewProjectile(
						Projectile.GetSource_FromThis(),
						Projectile.Center,
						perturbedSpeed,
						ModContent.ProjectileType<LamentStormHoming>(),
						(int)(Projectile.damage * damageScale),
						Projectile.knockBack,
						Projectile.owner
					);
				}
			}

			Projectile.Kill(); // 蓄力弹幕消失
			player.reuseDelay = 20;
		}
	}

	public override bool ShouldUpdatePosition() => false; // 蓄力时不自动移动

	public override bool? CanDamage() => false; // 蓄力本体不造成伤害

	public override void Kill(int timeLeft)
	{
		// 可以在这里加蓄力满时的特效
		// if (Projectile.localAI[0] >= MaxChargeTime) { ... }
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
		Texture2D textureWoodArrow = TextureAssets.Projectile[ModContent.ProjectileType<LamentStormP>()].Value;
		Texture2D textureGlow = this.RequestModRelativeTexture("WeaponGlow").Value;
		var player = Main.player[Projectile.owner];
		LamentStormPlayer lamentStormPlayer = player.GetModPlayer<LamentStormPlayer>();

		Vector2 drawPos = Projectile.Center - Main.screenPosition;
		float rot = Projectile.rotation;
		int textureHeight = texture.Height / Main.projFrames[Type];
		Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
		SpriteEffects flip = (player.direction == 1) ? SpriteEffects.None : SpriteEffects.FlipVertically;
		Vector2 offset = new Vector2(20, 0).RotatedBy(rot);
		Main.EntitySpriteDraw(texture, drawPos + offset, frame, lightColor, rot, new Vector2((float)(frame.Width / 2), (float)(frame.Height / 2)), base.Projectile.scale, flip, 0f);

		float chargePercent = 0;
		if (lamentStormPlayer.LamentStormAttackMode == LamentStormAttackType.Charge)
		{
			chargePercent = Utils.Clamp(Projectile.localAI[0] / MaxChargeTime, 0f, 1f);
			int frameGlow = (int)((Projectile.localAI[0] % 15) / 8);
			Rectangle frameGlowRect = textureGlow.Frame(1, 2, 0, frameGlow);
			Main.EntitySpriteDraw(textureGlow, drawPos + offset, frameGlowRect, Color.White * chargePercent, rot, new Vector2((float)(frameGlowRect.Width / 2), (float)(frameGlowRect.Height / 2)), Projectile.scale, flip, 0f);
		}
		if (Projectile.localAI[1] < player.itemAnimationMax)
		{
			foreach (var point in _relativeArrows)
			{
				float rotArrow = (point - new Vector2(-_currentBowStretchLength, 0)).ToRotation() + rot;
				Main.EntitySpriteDraw(textureWoodArrow, drawPos + offset - new Vector2(_currentBowStretchLength, 0).RotatedBy(rot), null, lightColor, rotArrow, new Vector2(0, textureWoodArrow.Height / 2f), Projectile.scale * MathHelper.Lerp(1, 1.23f, chargePercent), SpriteEffects.None, 0f);
			}
		}

		return false; // 不直接绘制蓄力弹幕
	}
}
