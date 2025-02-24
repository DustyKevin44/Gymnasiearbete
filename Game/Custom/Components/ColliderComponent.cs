using System;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;

namespace Game.Custom.Components;

public class ColliderComponent : ICollisionActor
{
    public IShapeF Bounds { get; }
    public object Tag { get; set; }

    public ColliderComponent(RectangleF rectangle, object tag = null)
    {
        Bounds = rectangle;
        Tag = tag;
    }

    public void OnCollision(CollisionEventArgs collisionInfo)
    {
        Console.WriteLine("Collision");
        // Handle collision response (e.g., stop movement, bounce, etc.)
    }
}
