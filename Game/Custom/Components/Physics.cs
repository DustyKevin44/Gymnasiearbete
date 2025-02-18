using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Game.Custom.Components;

public class PhysicsOutdated(Vector2 position)
{
    public Vector2 Velocity = Vector2.Zero;
    public Vector2 Position = position;

    public void Update(GameTime gameTime)
    {
        Position += Velocity * gameTime.GetElapsedSeconds();
    }
}
