using System;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;

namespace Game.Custom.Components;


public abstract class ColliderBox(IShapeF shape) : ICollisionActor
{
    public IShapeF Shape = shape;
    public IShapeF Bounds => Shape;

    public abstract void OnCollision(CollisionEventArgs collisionInfo);
}

public class HitBox(IShapeF shape) : ColliderBox(shape)
{
    public override void OnCollision(CollisionEventArgs collisionInfo)
    {
        
    }
}

public class HurtBox(IShapeF shape) : ColliderBox(shape)
{
    public override void OnCollision(CollisionEventArgs collisionInfo)
    {
        
    }
}

public class CollisionBox : ColliderBox
{
    public bool IsStatic;
    
    public CollisionBox(IShapeF shape, CollisionComponent collisionComponent, bool isStatic=false) : base(shape)
    {
        IsStatic = isStatic;
        collisionComponent.Insert(this);
    }

    public override void OnCollision(CollisionEventArgs collisionInfo)
    {
        Console.WriteLine("Collision");
        
        
    }
}
