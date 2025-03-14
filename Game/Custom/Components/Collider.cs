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
    public Entity Parent;
    public Vector2 previousPosition;

    public abstract void OnCollision(CollisionEventArgs collisionInfo);
}

public class HitBox : ColliderBox
{
    public HitBox(IShapeF shape) : base(shape)
    {
        Global.CollisionSystem.Insert(this);
    }

    public override void OnCollision(CollisionEventArgs collisionInfo)
    {
        Console.WriteLine("Bonk!");
    }
}

public class HurtBox : ColliderBox
{
    public HurtBox(IShapeF shape) : base(shape)
    {
        Global.CollisionSystem.Insert(this);
    }

    public override void OnCollision(CollisionEventArgs collisionInfo)
    {
        Console.WriteLine("Ouch!");
    }
}

public class CollisionBox : ColliderBox
{
    public bool IsStatic;

    public CollisionBox(IShapeF shape, bool isStatic = false) : base(shape)
    {
        IsStatic = isStatic;
        Global.CollisionSystem.Insert(this);
    }

    public override void OnCollision(CollisionEventArgs collisionInfo)
    {
        if (!Parent.Has<Transform2>() || !Parent.Has<VelocityComponent>()) return;
        if (collisionInfo.Other is not CollisionBox) return;

        var transfrom = Parent.Get<Transform2>();
        transfrom.Position -= collisionInfo.PenetrationVector / 2f;
    }
}
