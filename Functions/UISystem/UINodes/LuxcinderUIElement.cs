using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.UI;

namespace Luxcinder.Functions.UISystem.UINodes;
public class LuxcinderUIElement : UIElement
{
	public bool IsActive { get; set; } = true;
    public override void Draw(SpriteBatch spriteBatch) 
	{
		if (IsActive)
		{
			base.Draw(spriteBatch);
        }
	}

	public override void Update(GameTime gameTime)
	{
		if (IsActive)
		{
			base.Update(gameTime);
		}
    }
}
