using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luxcinder.Functions.UISystem.Utils;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Terraria.UI;

namespace Luxcinder.Functions.UISystem;
internal class LuxUISystem : ModSystem
{
    private static Dictionary<string, LuxcinderUILayer> LoadedUIs = new Dictionary<string, LuxcinderUILayer>();

    public static void RegisterUI(LuxcinderUILayer ui)
    {
        if (!LoadedUIs.ContainsKey(ui.GetType().Name))
        {
            LoadedUIs.Add(ui.GetType().Name, ui);
        }
		else
		{
			throw new Exception($"UI {ui.GetType().Name} is already registered in UISystem.");
		}
    }

	public static void Toggle<T>() where T : LuxcinderUILayer
	{
		if (LoadedUIs.TryGetValue(typeof(T).Name, out var ui))
		{
			if (ui.IsActive)
			{
				ui.IsActive = false;
				ui.OnDeactivate();
			}
			else
			{
				ui.IsActive = true;
				ui.OnActivate();
			}
		}
		else
		{
			throw new Exception($"UI {typeof(T).Name} is not registered in UISystem.");
		}
	}

	public static void SetActive<T>(bool active) where T : LuxcinderUILayer
	{
		if (LoadedUIs.TryGetValue(typeof(T).Name, out var ui))
		{
			if (active != ui.IsActive)
			{
				if(active)
				{
					ui.IsActive = true;
					ui.OnActivate();
				}
				else
				{
					ui.IsActive = false;
					ui.OnDeactivate();
				}
			}
		}
		else
		{
			throw new Exception($"UI {typeof(T).Name} is not registered in UISystem.");
		}
	}

	public static T GetUI<T>() where T : LuxcinderUILayer
	{
		if (LoadedUIs.TryGetValue(typeof(T).Name, out var ui))
		{
			return ui as T;
		}
		else
		{
			throw new Exception($"UI {typeof(T).Name} is not registered in UISystem.");
		}
	}

	public override void Load()
	{
		UIAssetLocator.Load();
	}

    public override void OnModLoad()
    {
		foreach (var ui in LoadedUIs)
		{
			ui.Value.Initialize();
		}
    }

    public override void UpdateUI(GameTime gameTime)
    {
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        // 在Inventory界面层上添加任务按钮
        int inventoryLayerIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));

        if (inventoryLayerIndex != -1)
        {
            foreach (var ui in LoadedUIs.Values)
            {
                string name = ui.InterfaceLayerName;
                layers.Insert(inventoryLayerIndex - 1, new LegacyGameInterfaceLayer(name, delegate ()
                {
					ui.Update(Main.gameTimeCache);
					ui.Draw(Main.spriteBatch);
                    return true;
                }, InterfaceScaleType.UI));
            }
        }
    }
}
