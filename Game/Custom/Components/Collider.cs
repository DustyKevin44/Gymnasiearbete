using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Graphics;

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
    public bool onCollisionBool;
    public Vector2 PenetrationVector = Vector2.Zero; // Store collision resolution vector

    public int entityId { get; set; }

    public CollisionBox(IShapeF shape, CollisionComponent collisionComponent, bool isStatic = false) 
        : base(shape)
    {
        IsStatic = isStatic;
        collisionComponent.Insert(this);
        Console.WriteLine("Collision added");
    }

    public void Initialize(int EntityId)
    {
        entityId = EntityId; 
    }

    public override void OnCollision(CollisionEventArgs collisionInfo)
    {
        Console.WriteLine($"Collision detected for: {entityId}");
        onCollisionBool = true;
        PenetrationVector = collisionInfo.PenetrationVector; // Store penetration vector
    }
}
