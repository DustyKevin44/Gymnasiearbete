using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Custom.States;

public abstract class State(Game game, GraphicsDevice graphicsDevice, ContentManager content)
{
    #region Fields

    protected ContentManager _content = content;
    protected GraphicsDevice _graphicsDevice = graphicsDevice;
    protected Game _game = game;

    #endregion

    #region Methods

    public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    public abstract void PostUpdate(GameTime gameTime);
    public abstract void Update(GameTime gameTime);

    #endregion
}