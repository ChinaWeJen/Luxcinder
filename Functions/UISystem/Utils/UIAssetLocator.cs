using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReLogic.Content;

namespace Luxcinder.Functions.UISystem.Utils;
public class UIAssetLocator
{
	public static Asset<Texture2D> CheckBox_Box_Texture;
	public static Asset<Texture2D> CheckBox_Check_Texture;

	public static void Load()
	{
		CheckBox_Box_Texture = AssetExtensions.RequestModRelativeTexturePathFull<UIAssetLocator>("../Assets/CheckBox");
		CheckBox_Check_Texture = AssetExtensions.RequestModRelativeTexturePathFull<UIAssetLocator>("../Assets/CheckMark");
	}
}
