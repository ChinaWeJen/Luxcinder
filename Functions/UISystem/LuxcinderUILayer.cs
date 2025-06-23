using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Luxcinder.Functions.UISystem;
public abstract class LuxcinderUILayer : ILoadable
{
    public Mod Mod { get; private set; }

    /// <summary>
    /// UI层名称，不要和其他UI层名称冲突
    /// </summary>
    public abstract string InterfaceLayerName
    {
        get;
    }

	public bool IsActive { get; set; } = false;

	public void Load(Mod mod)
    {
        Mod = mod;
        LuxUISystem.RegisterUI(this);
    }

    public void Unload()
    {
        // Unregister UI elements or clean up resources here
    }

    /// <summary>
    /// 当UI被加载时调用，初始化逻辑在Mod加载后执行且只执行一次
    /// </summary>
    public virtual void Initialize()
    {

    }

    /// <summary>
    /// 当UI被激活时调用
    /// </summary>
    public virtual void OnActivate()
    {

    }

    /// <summary>
    /// 
    /// </summary>
    public virtual void OnDeactivate()
    {
        
    }

    /// <summary>
    /// 
    /// </summary>
    public virtual void Update(GameTime gameTime)
    {
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="sb"></param>
    public virtual void Draw(SpriteBatch sb)
    {
        
    }
}
