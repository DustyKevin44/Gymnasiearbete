using System;
using System.Collections.Generic;
using System.Linq;
using Game.Custom.Utilities;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.ECS;

namespace Game.Custom.Components;


public abstract class ColliderBox(IShapeF shape, bool enabled) : ICollisionActor
{
    public IShapeF Shape = shape;
    public IShapeF Bounds => Shape;
    public Entity Parent;
    public Vector2 currentParentPosition;
    public bool IsEnabled = enabled;

    public abstract void OnCollision(CollisionEventArgs collisionInfo);
}

public class HitBox : ColliderBox
{
    public readonly HashSet<HurtBox> HurtBoxesHit = []; // Store collisions to ignore hitting the same enemy in 1 attack
    public bool IsPerFrameHitBox; // If true: hits enemies every frame, if false then hits once and never again until 'EmptyCollisions()' is called

    public HitBox(IShapeF shape, bool enabled = true, bool isPerFrameHitBox = false) : base(shape, enabled)
    {
        IsPerFrameHitBox = isPerFrameHitBox;
        Global.CollisionSystem.Insert(this);
    }

    public override void OnCollision(CollisionEventArgs collisionInfo)
    {
        if (!IsPerFrameHitBox &&
            IsEnabled &&
            collisionInfo.Other is HurtBox hurtBox &&
            hurtBox.IsEnabled &&
            !HasCollidedWith(hurtBox))
        {
            // HitBox logic
            HurtBoxesHit.Add(hurtBox);

            // HurtBox logic
            if (!Utils.TryGet(Parent, out MeleeAttack mAttack)) return;
            if (!Utils.TryGet(hurtBox.Parent, out HealthComponent healthC)) return;
            healthC.Health -= mAttack.Damage;
        }
    }

    public bool HasCollidedWith(HurtBox hurtBox) => HurtBoxesHit.Contains(hurtBox);
    public void EmptyCollisions() => HurtBoxesHit.Clear();
}

public class HurtBox : ColliderBox
{
    public HurtBox(IShapeF shape, bool enabled = true) : base(shape, enabled)
    {
        Global.CollisionSystem.Insert(this);
    }

    public override void OnCollision(CollisionEventArgs collisionInfo) { /* Hurt logic is handled by hitbox */ }
}

public class CollisionBox : ColliderBox
{
    public CollisionBox(IShapeF shape, bool enabled = true) : base(shape, enabled)
    {
        Global.CollisionSystem.Insert(this);
    }

    public override void OnCollision(CollisionEventArgs collisionInfo)
    {
        if (collisionInfo.Other is not CollisionBox cBox || !cBox.IsEnabled) return;
        if (!Parent.Has<Transform2>() || !Parent.Has<VelocityComponent>()) return;
        if (!IsEnabled) return;

        var transfrom = Parent.Get<Transform2>();
        transfrom.Position -= collisionInfo.PenetrationVector / 2f;
    }
}
