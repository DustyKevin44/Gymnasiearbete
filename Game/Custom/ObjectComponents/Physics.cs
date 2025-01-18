using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Game.Custom.ObjectComponents;

public class Physics(Vector2 position) : Component
{
    public Vector2 _velocity { get; private set; }
    public Vector2 _position = position; // Objektets position (Sluta med dessa kommentarer, det är skit)

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch) {}
	public override void Update(GameTime gameTime) {}

    public void PosUpdate(GameTime gameTime, Vector2 position, Vector2 velocity)
	{ 
        _position += velocity * gameTime.GetElapsedSeconds(); 
    }
}
