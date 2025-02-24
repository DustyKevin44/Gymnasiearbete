using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.ECS;

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
    public bool onCollision;

    public Entity entity {get; private set;}
    
    public CollisionBox(IShapeF shape, CollisionComponent collisionComponent,Entity entity, bool isStatic=false) : base(shape)
    {
        entity = entity;
        IsStatic = isStatic;
        collisionComponent.Insert(this);
        Console.WriteLine("Collision added");

    }

      public override void OnCollision(CollisionEventArgs collisionInfo)
    {
        Console.WriteLine($"Collision detected for entity {entity}");
        onCollision = true;

        // 🔹 Example: Change the entity's sprite color when colliding
        if (entity.Has<SpriteComponent>())
        {
            entity.Get<SpriteComponent>().Color = Color.Black;
        }
    }
}
