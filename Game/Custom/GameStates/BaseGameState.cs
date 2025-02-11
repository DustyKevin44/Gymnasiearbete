using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Custom.GameStates;

public abstract class GameState(Game game, GraphicsDevice graphicsDevice, ContentManager content)
{
    protected ContentManager _content = content;
    protected GraphicsDevice _graphicsDevice = graphicsDevice;
    protected Game _game = game;

    public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    public abstract void PostUpdate(GameTime gameTime);
    public abstract void Update(GameTime gameTime);
    public abstract void LoadContent();
}