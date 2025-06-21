using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luxcinder.Functions.MissionSystem.Core;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Luxcinder.Functions.MissionSystem;
public class MissionPlayer : ModPlayer
{
	public List<Mission> Missions = new();

	public override void OnEnterWorld()
	{
		// 可在此处初始化或加载任务
		if(Missions.Count == 0)
		{
			Missions.Add(new Mission
			{
				Id = 0,
				Name = Mod.GetLocalization("Missions.FirstMission.Name"),
				Description = Mod.GetLocalization("Missions.FirstMission.Description"),
				Status = MissionStatus.InProgress,
				Conditions = new List<MissionCondition>
				{
					new ItemCollectCondition(ModContent.ItemType<Items.ExampleItem>(), 10)
				}
			});
		}
	}

	// --- 存档与读取 ---
	public override void SaveData(TagCompound tag)
	{
		var missionsTag = new List<TagCompound>();
		foreach (var mission in Missions)
		{
			var missionTag = new TagCompound
			{
				["id"] = mission.Id,
				["status"] = mission.Status,
				// 使用Key来保存本地化文本的Key
				["name"] = mission.Name.Key,
				["description"] = mission.Description.Key
			};
			// 保存每个条件的进度（如击杀数等）
			var condProgress = new List<TagCompound>();
			foreach (var cond in mission.Conditions)
			{
				TagCompound tagCond = new TagCompound();
				cond.Save(tagCond);
				condProgress.Add(tagCond);
			}
			missionTag["condProgress"] = condProgress;
			missionsTag.Add(missionTag);
		}
		tag["missions"] = missionsTag;
	}

	public override void LoadData(TagCompound tag)
	{
		Missions.Clear();
		if (tag.ContainsKey("missions"))
		{
			var missionsTag = tag.GetList<TagCompound>("missions");
			foreach (var missionTag in missionsTag)
			{
				string id = missionTag.GetString("id");
				Mission mission = new Mission();
				mission.Id = id;
				mission.Name = Mod.GetLocalization(missionTag.GetString("name"));
				mission.Description = Mod.GetLocalization(missionTag.GetString("description"));
				if (mission == null)
					continue;
				mission.Status = missionTag.Get<MissionStatus>("status");
				var condProgress = missionTag.GetList<TagCompound>("condProgress");

				for (int i = 0; i < condProgress.Count && i < mission.Conditions.Count; i++)
				{
					mission.Conditions[i].Load(condProgress[i]);
				}
				Missions.Add(mission);
			}
		}
	}
}