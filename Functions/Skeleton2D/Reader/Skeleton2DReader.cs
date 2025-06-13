using System;
using System.ComponentModel;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Spine;
using Terraria;
using static ReLogic.Peripherals.RGB.Corsair.CorsairDeviceGroup;

namespace Luxcinder.Functions.Skeleton2D.Reader;

public class Skeleton2DReader
{
	public static Skeleton2D ReadSkeleton(byte[] atlasData, byte[] jsonData, Texture2D atlasTexture, float scale = 1.0f)
	{
		using var ms_atlas = new MemoryStream(atlasData);
		using var sr_atlas = new StreamReader(ms_atlas);

		// Using empty dir string here because we do not support multiple layers
		Atlas atlas = new Atlas(sr_atlas, "", new XnaTextureLoader(atlasTexture));

		using var ms_json = new MemoryStream(jsonData);
		using var sr_json = new StreamReader(ms_json);
		SkeletonJson json = new SkeletonJson(atlas);
		json.Scale = scale;
		SkeletonData skeletonData = json.ReadSkeletonData(sr_json);

		Skeleton skeleton = new Skeleton(skeletonData);

		skeleton.X = Main.screenWidth / 2;
		skeleton.Y = Main.screenHeight;

		return ConvertTopublicSkeleton(skeleton, atlas);
	}


	private static Skeleton2D ConvertTopublicSkeleton(Skeleton skeleton, Atlas atlas)
	{
		AnimationStateData stateData = new AnimationStateData(skeleton.Data);
		AnimationState state = new AnimationState(stateData);
		return new Skeleton2D(skeleton, atlas, state);
	}

	///// <summary>
	///// 解析动画帧部分
	///// </summary>
	///// <param name="jAnimations"></param>
	///// <param name="skeleton"></param>
	///// <param name="attachmentsDict"></param>
	///// <param name="slotsDict"></param>
	///// <param name="bonesDict"></param>
	//private static void ParseAnimations(Dictionary<string, JAnimation> jAnimations,
	//	Skeleton2D skeleton,
	//	Dictionary<string, Attachment> attachmentsDict,
	//	Dictionary<string, Slot> slotsDict,
	//	Dictionary<string, Bone2D> bonesDict)
	//{
	//	skeleton.Animations = new Dictionary<string, Animation>();

	//	foreach (var animKV in jAnimations)
	//	{
	//		var animation = new Animation();
	//		animation.Name = animKV.Key;
	//		animation.BonesTimeline = ParseBoneTimelines(animKV.Value.Bones, bonesDict);
	//		animation.SlotsTimeline = ParseSlotTimelines(animKV.Value.Slots, slotsDict, attachmentsDict);
	//		skeleton.Animations.Add(animation.Name, animation);
	//	}
	//}


	//private static List<Timeline> ParseBoneTimelines(Dictionary<string, JObject> bones,
	//	Dictionary<string, Bone2D> bonesDict)
	//{
	//	var timelines = new List<Timeline>();
	//	if (bones == null)
	//		return timelines;

	//	foreach (var kvPair in bones)
	//	{
	//		var bone = bonesDict[kvPair.Key];
	//		var jobject = kvPair.Value;

	//		var timeline = new Timeline();

	//		foreach (var componentKFPair in jobject)
	//		{
	//			var type = componentKFPair.Key;
	//			var values = componentKFPair.Value as JArray;
	//			var track = new Track();

	//			foreach (JObject keyFrame in values)
	//			{
	//				float time = 0;
	//				InterpolationMethod interpolation = InterpolationMethod.Lerp;

	//				// 非必要 time 属性
	//				if (keyFrame.ContainsKey("time"))
	//					time = keyFrame.Value<float>("time");

	//				// 非必要 curve 属性
	//				if (keyFrame.ContainsKey("curve"))
	//				{
	//					if (keyFrame["curve"].Type == JTokenType.String)
	//					{
	//						var curvetype = keyFrame.Value<string>("curve");
	//						switch (curvetype)
	//						{
	//							case "linear":
	//								{
	//									interpolation = InterpolationMethod.Lerp;
	//									break;
	//								}
	//							case "stepped":
	//								{
	//									interpolation = InterpolationMethod.Step;
	//									break;
	//								}
	//						}
	//					}
	//					else // 不是string那么就是曲线参数
	//					{
	//						float cx1 = keyFrame.Value<float>("curve");
	//						float cy1 = 0;
	//						float cx2 = 1;
	//						float cy2 = 1;
	//						if (keyFrame.ContainsKey("c2"))
	//							cy1 = keyFrame.Value<float>("c2");
	//						if (keyFrame.ContainsKey("c3"))
	//							cx2 = keyFrame.Value<float>("c3");
	//						if (keyFrame.ContainsKey("c4"))
	//							cy2 = keyFrame.Value<float>("c4");
	//						interpolation = new Curve(new Vector2(cx1, cy1), new Vector2(cx2, cy2));
	//					}
	//				}

	//				if (type == "translate")
	//				{
	//					// translate有x，y两个可选属性
	//					Vector2 translate = Vector2.Zero;
	//					if (keyFrame.ContainsKey("x"))
	//						translate.X = keyFrame.Value<float>("x");
	//					if (keyFrame.ContainsKey("y"))
	//						translate.Y = keyFrame.Value<float>("y");
	//					track.AddKeyFrame(new BoneTranslationKeyFrame(time, bone, interpolation, bone.Position + translate));
	//				}
	//				else if (type == "rotate")
	//				{
	//					// rotate 只有旋转角一个可选属性
	//					float angle = 0f;
	//					if (keyFrame.ContainsKey("angle"))
	//						angle = keyFrame.Value<float>("angle");
	//					track.AddKeyFrame(new BoneRotationKeyFrame(time, bone, interpolation, bone.Rotation - angle / 180.0f * MathHelper.Pi));
	//				}
	//			}
	//			timeline.AddTrack(track);
	//		}
	//		timelines.Add(timeline);
	//	}
	//	return timelines;
	//}


	//private static List<Timeline> ParseSlotTimelines(Dictionary<string, JObject> slots,
	//	Dictionary<string, Slot> slotsDict,
	//	Dictionary<string, Attachment> attachmentsDict
	//	)
	//{
	//	var timelines = new List<Timeline>();
	//	if (slots == null)
	//		return timelines;
	//	foreach (var kvPair in slots)
	//	{
	//		var slot = slotsDict[kvPair.Key];
	//		var jobject = kvPair.Value;

	//		var timeline = new Timeline();

	//		foreach (var componentKFPair in jobject)
	//		{
	//			var type = componentKFPair.Key;
	//			var values = componentKFPair.Value as JArray;
	//			var track = new Track();

	//			foreach (JObject keyFrame in values.Cast<JObject>())
	//			{
	//				float time = 0;
	//				InterpolationMethod interpolation = InterpolationMethod.Lerp;

	//				// 非必要 time 属性
	//				if (keyFrame.ContainsKey("time"))
	//					time = keyFrame.Value<float>("time");

	//				// 非必要 curve 属性
	//				if (keyFrame.ContainsKey("curve"))
	//				{
	//					// TODO: 曲线控制参数暂时没做
	//					var curvetype = keyFrame.Value<string>("curve");
	//					switch (curvetype)
	//					{
	//						case "linear":
	//							{
	//								interpolation = InterpolationMethod.Lerp;
	//								break;
	//							}
	//						case "steppped":
	//							{
	//								interpolation = InterpolationMethod.Step;
	//								break;
	//							}
	//					}
	//				}

	//				if (type == "attachment")
	//				{
	//					Attachment attachment = null;
	//					if (keyFrame.GetValue("name").Type != JTokenType.Null)
	//					{
	//						var name = keyFrame.Value<string>("name");
	//						if (attachmentsDict.ContainsKey(name))
	//							attachment = attachmentsDict[name];
	//					}

	//					track.AddKeyFrame(new SlotAttachmentKeyFrame(time, slot, attachment));
	//				}

	//			}

	//			timeline.AddTrack(track);
	//		}
	//		timelines.Add(timeline);
	//	}
	//	return timelines;
	//}

	private static int charToHex(char c)
	{
		if (char.IsNumber(c))
		{
			return c - '0';
		}
		else if (char.IsLower(c))
		{
			return c - 'a';
		}
		else if (char.IsUpper(c))
		{
			return c - 'A';
		}
		return 0;
	}

	private static Color HexToColor(string hex)
	{
		int r = charToHex(hex[0]) * 16 + charToHex(hex[1]);
		int g = charToHex(hex[2]) * 16 + charToHex(hex[3]);
		int b = charToHex(hex[4]) * 16 + charToHex(hex[5]);
		int a = charToHex(hex[6]) * 16 + charToHex(hex[7]);
		return new Color(r, g, b, a);
	}
}
