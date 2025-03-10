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

public class HitBox(IShapeF shape) : ColliderBox(shape)
{
    public override void OnCollision(CollisionEventArgs collisionInfo)
    {
        Console.WriteLine("Bonk!");
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

    public CollisionBox(IShapeF shape, bool isStatic = false) : base(shape)
    {
        IsStatic = isStatic;
        Global.CollisionSystem.Insert(this);
    }

    public void Initialize(Entity parent) => Parent = parent;

    public override void OnCollision(CollisionEventArgs collisionInfo)
    {
        if (!Parent.Has<Transform2>() || !Parent.Has<VelocityComponent>()) return;
        if (collisionInfo.Other is not CollisionBox) return; // <-- TODO: Why doesn't this work!!?!

        var transfrom = Parent.Get<Transform2>();
        transfrom.Position -= collisionInfo.PenetrationVector / 2f;
    }
}
