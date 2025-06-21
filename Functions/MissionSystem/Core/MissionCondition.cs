using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.IO;

namespace Luxcinder.Functions.MissionSystem.Core;
public abstract class MissionCondition
{
	public abstract bool IsCompleted(Player player);

	public abstract void Save(TagCompound tag);

	public abstract void Load(TagCompound tag);
}

// 获得物品条件
public class ItemCollectCondition : MissionCondition
{
	public int ItemType;
	public int Amount;

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

	}
	public override void Save(TagCompound tag)
	{

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