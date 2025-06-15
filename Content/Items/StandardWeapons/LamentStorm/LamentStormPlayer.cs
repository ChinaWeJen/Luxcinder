using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Luxcinder.Content.Items.StandardWeapons.LamentStorm;

public enum LamentStormAttackType
{
	Normal, //平射
	Fall, // 从天而降

}
public class LamentStormPlayer : ModPlayer
{
	public int windSigils = 0; // 风之符印数量
	public LamentStormAttackType LamentStormAttackMode = LamentStormAttackType.Normal;

	public override void PostUpdate()
	{
		//LamentStormAttackMode = LamentStormAttackType.Fall;
	}
}
