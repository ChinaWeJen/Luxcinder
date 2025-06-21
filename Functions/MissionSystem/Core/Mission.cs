using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Localization;

namespace Luxcinder.Functions.MissionSystem.Core;
public enum MissionStatus
{
	NotAccepted,
	InProgress,
	CanComplete,
	Completed,
	Rejected
}


public class Mission
{
	public int Id { get; set; }
	public LocalizedText Name { get; set; }
	public LocalizedText Description { get; set; }

	public List<MissionCondition> Conditions = new();
	public MissionStatus Status = MissionStatus.NotAccepted;
	public Action<Player> RewardAction;

	public bool CheckCanComplete(Player player)
	{
		if (Status != MissionStatus.InProgress)
			return false;
		foreach (var cond in Conditions)
			if (!cond.IsCompleted(player))
				return false;
		Status = MissionStatus.CanComplete;
		return true;
	}

	public void GiveReward(Player player)
	{
		if (Status == MissionStatus.CanComplete)
		{
			RewardAction?.Invoke(player);
			Status = MissionStatus.Completed;
		}
	}
}