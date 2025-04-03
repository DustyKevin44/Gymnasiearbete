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
    public Vector2 previousPosition;
    public bool IsEnabled = enabled;

    public abstract void OnCollision(CollisionEventArgs collisionInfo);
}

public class HitBox : ColliderBox
{
    public readonly HashSet<HurtBox> _hurtBoxesHit = []; // Store collisions to ignore hitting the same enemy in 1 attack
    private List<HurtBox> _newCollisions = [];
    public bool IsPerFrameHitBox; // If true: hits enemies every frame, if false then hits once and never again until 'EmptyCollisions()' is called

    public HitBox(IShapeF shape, bool enabled = true, bool isPerFrameHitBox = false) : base(shape, enabled)
    {
        IsPerFrameHitBox = isPerFrameHitBox;
        Global.CollisionSystem.Insert(this);
    }

    public override void OnCollision(CollisionEventArgs collisionInfo)
    {
        // TODO: fix saving colliders so that attacks only hit once per swing
        
        // if (IsPerFrameHitBox) return;
        // if (IsEnabled &&
        //     collisionInfo.Other is HurtBox hurtBox &&
        //     !HasCollidedWith(hurtBox) &&
        //     hurtBox.IsEnabled)
        // {
        //     _newCollisions.Add(hurtBox);
        // }
        if (IsEnabled && collisionInfo.Other is HurtBox)
            Console.WriteLine("Pow!!");
    }

    public bool HasCollidedWith(HurtBox hurtBox) => _hurtBoxesHit.Contains(hurtBox);
    public void EmptyCollisions() => _hurtBoxesHit.Clear();

    public void UpdateCollisions()
    {
        foreach (var c in _newCollisions)
            _hurtBoxesHit.Add(c);
        _newCollisions.Clear();
    }
}

public class HurtBox : ColliderBox
{
    public HurtBox(IShapeF shape, bool enabled = true) : base(shape, enabled)
    {
        Global.CollisionSystem.Insert(this);
    }

    public override void OnCollision(CollisionEventArgs collisionInfo)
    {
        if (IsEnabled &&
            Parent is not null &&
            collisionInfo.Other is HitBox hitBox &&
            hitBox.IsEnabled &&
            !hitBox.HasCollidedWith(this))
        {
            Console.WriteLine("Ow");
            if (!Utils.TryGet(hitBox.Parent, out MeleeAttack ma)) return;
            if (!Utils.TryGet(Parent, out HealthComponent hc)) return;
            hc.Health -= ma.Damage;
        }
    }
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
