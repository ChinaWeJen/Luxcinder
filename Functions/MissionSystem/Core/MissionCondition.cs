using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luxcinder.Functions.UISystem.UICore;
using Luxcinder.Functions.UISystem.UINodes;
using Luxcinder.Functions.UISystem.UINodes.Layout;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.IO;

namespace Luxcinder.Functions.MissionSystem.Core;
public abstract class MissionCondition
{
	public abstract bool IsCompleted(Player player);

	public abstract void Save(TagCompound tag);

	public abstract void Load(TagCompound tag);

	public bool NeedUIRepresentation => true; // 是否需要UI显示进度

	public virtual LuxUIContainer GetProgressUI(Player player)
	{
		return new LuxUIContainer();
	}
}

// 获得物品条件
public class ItemCollectCondition : MissionCondition
{
	public int ItemType;
	public int Amount;

	// 一定要有一个默认无参构造函数，用于动态创建类型
	public ItemCollectCondition()
	{
	}
	public ItemCollectCondition(int itemType, int amount)
	{
		ItemType = itemType;
		Amount = amount;
	}

	public override bool IsCompleted(Player player)
	{
		int count = 0;
		foreach (var item in player.inventory)
			if (item.type == ItemType)
				count += item.stack;
		return count >= Amount;
	}

	public override void Load(TagCompound tag)
	{
		Item item = ItemIO.Load(tag.Get<TagCompound>("item"));
		ItemType = item.type;
		Amount = tag.GetInt("amount");
	}
	public override void Save(TagCompound tag)
	{
		Item item = new Item();
		item.SetDefaults(ItemType);
		tag["item"] = ItemIO.Save(item);
		tag["amount"] = Amount;
	}

	public override LuxUIContainer GetProgressUI(Player player)
	{
		var checkBox = new LuxUICheckBox();
		checkBox.Width.Set(16, 0);
		checkBox.Height.Set(16, 0);
		var uiAlign = new LuxUIHorizontalAlign(true);
		uiAlign.Width.Set(0, 1f);
		uiAlign.Height.SetAuto(true);
		uiAlign.MaxWidth.Set(0, 1f);


		Item target = new Item();
		target.SetDefaults(ItemType);
		int count = 0;
		foreach (var item in player.inventory)
			if (item.type == ItemType)
				count += item.stack;
		checkBox.SetChecked(count >= Amount);

		var uiText = new LuxUIText($"获得{Amount}个{target.Name} 【{count}/{Amount}】");
		uiText.TextLayout = TextLayout.AutoWrap;
		uiText.MarginLeft = 12f;
		uiText.Width.Set(360, 0);

		var uiItemSlot = new LuxUIItemSlot(target);
		uiAlign.AddChild(checkBox);
		uiAlign.AddChild(uiText);
		uiAlign.AddChild(uiItemSlot);

		return uiAlign;

	}
}

public class DeathCountCondition : MissionCondition
{
	private int _deathCount = 0;
	private int _maxDeathCount = 5;

	public DeathCountCondition()
	{
	}

	public DeathCountCondition(int maxDeathCount)
	{
		_maxDeathCount = maxDeathCount;
	}

	public override bool IsCompleted(Player player)
	{
		return _deathCount >= _maxDeathCount;
	}

	public override void Load(TagCompound tag)
	{
		_deathCount = tag.GetInt("deathCount");
		_maxDeathCount = tag.GetInt("maxDeathCount");
	}
	public override void Save(TagCompound tag)
	{
		tag["deathCount"] = _deathCount;
		tag["maxDeathCount"] = _maxDeathCount;
	}
}