using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luxcinder.Content.Items.Mission.One;
using Luxcinder.Functions.MissionSystem.Core;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Luxcinder.Functions.MissionSystem;
public class MissionPlayer : ModPlayer
{
	public Dictionary<string, Mission> Missions = new Dictionary<string, Mission>();

	//public override ModPlayer NewInstance(Player entity)
	//{
	//	base.NewInstance(entity);
	//}

	//protected override bool CloneNewInstances => true;
	//public override ModPlayer Clone(Player newEntity)
	//{
	//	var missionplayer = (MissionPlayer)base.Clone(newEntity);
	//	missionplayer.Missions = new Dictionary<string, Mission>();
	//	return missionplayer;
	//}
	public override void OnEnterWorld()
	{
	}

	public override void UpdateEquips()
	{
		//Missions.Clear();
	}

	public void AcceptMission(Mission mission)
	{
		if (Missions.ContainsKey(mission.Id))
		{
			Main.NewText($"Mission with ID {mission.Id} already exists.", Color.Red);
			return;
		}

		Missions[mission.Id] = mission;
		mission.Status = MissionStatus.InProgress; // 默认状态为进行中
	}
	public bool HasIncompletedMission()
	{
		foreach (var mission in Missions.Values)
		{
			if (mission.Status == MissionStatus.InProgress || mission.Status == MissionStatus.CanComplete)
			{
				return true; // 存在未完成的任务
			}
		}
		return false;
	}

	public bool CheckIfMissionCompleted(string missionName)
	{
		if (!Missions.ContainsKey(missionName))
		{
			return false;
		}

		if (Missions[missionName].Status == MissionStatus.Completed || Missions[missionName].CheckCanComplete(Player))
		{
			return true;
		}
		return true;
	}

	public void CompleteMission(string missionName)
	{
		// 完成任务
		if (!Missions.ContainsKey(missionName))
		{
			return;
		}
		Missions[missionName].Status = MissionStatus.Completed;
		Missions[missionName].GiveReward(Player);
		Main.NewText($"任务 [{Missions[missionName].Name.Value}] 已完成！", Color.Lime);
	}

	// --- 存档与读取 ---
	public override void SaveData(TagCompound tag)
	{
		var missionsTag = new List<TagCompound>();
		foreach (var mission in Missions.Values)
		{
			var missionTag = new TagCompound
			{
				["id"] = mission.Id,
				["status"] = (int)mission.Status,
				// 使用Key来保存本地化文本的Key
				["name"] = mission.Name.Key,
				["description"] = mission.Description.Key
			};
			// 保存每个条件的进度（如击杀数等）
			var condProgress = new List<TagCompound>();
			foreach (var cond in mission.Conditions)
			{
				TagCompound tagCond = new TagCompound();
				// 使用条件的类型名来标识条件类型
				tagCond["type"] = cond.GetType().FullName;
				TagCompound conditionContent = new TagCompound();
				cond.Save(conditionContent);
				tagCond["content"] = conditionContent;
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
				mission.Name = Language.GetText(missionTag.GetString("name"));
				mission.Description = Language.GetText(missionTag.GetString("description"));
				mission.Status = (MissionStatus)missionTag.Get<int>("status");
				var condProgress = missionTag.GetList<TagCompound>("condProgress");

				for (int i = 0; i < condProgress.Count; i++)
				{
					string type = condProgress[i].GetString("type");
					Type condType = Type.GetType(type);
					MissionCondition condition = (MissionCondition)Activator.CreateInstance(condType);
					condition.Load(condProgress[i].Get<TagCompound>("content"));
					mission.Conditions.Add(condition);
				}
				Missions.Add(mission.Id, mission);
			}
		}
	}
}