using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Game.Custom.ObjectComponents;

public class Physics(Vector2 position)
{
    public Vector2 Velocity = Vector2.Zero;
    public Vector2 Position = position;

    public void Update(GameTime gameTime)
    {
        Position += Velocity * gameTime.GetElapsedSeconds();
    }
}
