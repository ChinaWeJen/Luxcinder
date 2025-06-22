using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luxcinder.Functions.MissionSystem.Core;
using Luxcinder.Functions.UISystem.UICore;
using Luxcinder.Functions.UISystem.UINodes;
using Luxcinder.Functions.UISystem.UINodes.Layout;
using Terraria.UI;

namespace Luxcinder.Functions.MissionSystem.UI;
public class MissionContentUI : LuxUIContainer
{
	private LuxUIText _missionName;
	private LuxUIText _missionDescription;
	private LuxUIText _missionStatusText;
	private LuxUIText _missionReturnText;

	private LuxUIVertialAlign _missionConditionList;
	private Mission _mission;
	public MissionContentUI()
	{
		LuxUIVertialAlign luxUIVertialAlign = new LuxUIVertialAlign();
		luxUIVertialAlign.Width.Set(0, 1f);
		luxUIVertialAlign.Height.SetAuto(true);
		AddChild(luxUIVertialAlign);

		_missionName = new LuxUIText("Mission.Name");
		_missionName.Width.Set(0, 1f);
		luxUIVertialAlign.AddChild(_missionName);

		_missionDescription = new LuxUIText("Mission.Description");
		_missionDescription.Width.Set(0, 1f);
		_missionDescription.TextLayout = TextLayout.AutoWrap;
		luxUIVertialAlign.AddChild(_missionDescription);

		_missionConditionList = new LuxUIVertialAlign();
		_missionConditionList.Width.Set(0, 1f);
		luxUIVertialAlign.AddChild(_missionConditionList);

		_missionStatusText = new LuxUIText("Mission.Status");
		var anchor = new LuxUIAnchor(_missionStatusText, Vector2.One * 0.5f, Vector2.One * 0.5f);
		anchor.Height.Set(38, 0);
		luxUIVertialAlign.AddChild(anchor);

		_missionReturnText = new LuxUIText("Mission.ReturnText");
		var anchor2 = new LuxUIAnchor(_missionReturnText, Vector2.One * 0.5f, Vector2.One * 0.5f);
		anchor2.Height.Set(32, 0);
		luxUIVertialAlign.AddChild(anchor2);

	}
	public override void Update(GameTime gameTime)
	{
		if (_mission != null)
		{
			_missionReturnText.Visible = _mission.Status == MissionStatus.CanComplete;
		}
		base.Update(gameTime);
	}

	public void SetMission(Mission mission)
	{
		_mission = mission;
		_missionConditionList.ClearChildren();
		if (mission == null)
		{
			_missionName.SetText("");
			_missionDescription.SetText("");
			_missionStatusText.SetText("");
			_missionReturnText.SetText("");
		}
		else
		{
			_missionName.SetText(mission.Name.Value);
			_missionDescription.SetText(mission.Description.Value);
			_missionStatusText.SetText(mission.Status.ToString());
			_missionReturnText.SetText("把任务交回给NPC以完成任务");

			for (int i = 0; i < mission.Conditions.Count; i++)
			{
				var condition = mission.Conditions[i];
				if (condition.NeedUIRepresentation)
				{
					_missionConditionList.AddChild(condition.GetProgressUI(Main.LocalPlayer));
				}
			}
		}
	}

	public void RefreshMissionUI()
	{
		if (_mission != null)
		{
			SetMission(_mission);
		}
	}

	public override void InitializeDependencies()
	{
		base.InitializeDependencies();
	}
	public override void ResolveDependencies()
	{
		base.ResolveDependencies();
	}
}
