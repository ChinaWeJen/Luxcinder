using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.UI;

namespace Luxcinder.Functions.UISystem;
internal class UISystem : ModSystem
{
    private static List<LuxcinderUILayer> LoadedUIs = new List<LuxcinderUILayer>();

    public static void RegisterUI(LuxcinderUILayer ui)
    {
        if (!LoadedUIs.Contains(ui))
        {
            LoadedUIs.Add(ui);
        }
    }

    public override void OnModLoad()
    {
        foreach (var ui in LoadedUIs)
        {
            ui.Initialize();
        }
    }

    public override void UpdateUI(GameTime gameTime)
    {
        foreach (var ui in LoadedUIs)
        {
            ui.Update(gameTime);
        }
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        // 在Inventory界面层上添加任务按钮
        int inventoryLayerIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));

        if (inventoryLayerIndex != -1)
        {
            foreach (var ui in LoadedUIs)
            {
                string name = ui.InterfaceLayerName;
                layers.Insert(inventoryLayerIndex - 1, new LegacyGameInterfaceLayer(name, delegate ()
                {
                    ui.Draw(Main.spriteBatch);
                    return true;
                }, InterfaceScaleType.UI));
            }
        }
    }
}
