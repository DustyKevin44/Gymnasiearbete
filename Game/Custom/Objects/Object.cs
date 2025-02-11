using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Custom.Objects;

public abstract class Object(Texture2D texture, Vector2 position)
{
	private readonly Texture2D _texture = texture;
    public Vector2 Position = position;

    public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
	public abstract void Update(GameTime gameTime);
}
