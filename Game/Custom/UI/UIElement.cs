using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Game.Custom.UI;

public abstract class UIElement
{
    public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    public abstract void Update(GameTime gameTime);
}
