using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Custom.Objects;

public class Object(Texture2D texture, Vector2 position) : Component
{
	private readonly Texture2D _texture = texture;
    public Vector2 _Position = position;

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch){}
	public override void Update(GameTime gameTime) {}
}
