using MonoGame.Extended;
using Microsoft.Xna.Framework;

namespace Game.Custom.ObjectComponents;
public class VelocityComponent
{
    public Vector2 Velocity { get; set; } // En hastighet

    public VelocityComponent(Vector2 velocity)
    {
        Velocity = velocity;  // En hastighet
    }
}