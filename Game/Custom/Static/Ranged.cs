using System;
using Game.Custom.Components;
using Game.Custom.Factories;
using Game.Custom.Utilities;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ECS;

namespace Game.Custom.Static;

public enum RangedType
{
    Mango,
}

public static class Ranged
{
    public static bool Attack(Entity entity, Vector2 direction)
    {
        if (!entity.Has<RangedAttack>()) return false; // Weapon is not ranged weapon
        if (!entity.Has<Transform2>()) return false; // For projectile spawn position

        var ranged = entity.Get<RangedAttack>();
        var transform = entity.Get<Transform2>(); // Make sure that the world matrix is the parent entites transform

        switch (ranged.RangedType)
        {
            case RangedType.Mango:
                var projectile = EntityFactory.CreateProjectileAt(transform.Position, ranged, direction);
                var tweener = Utils.GetTweener(projectile);
                tweener.TweenTo(projectile.Get<Transform2>(), transform => transform.Rotation, 360f, 2f).RepeatForever();
                break;
            default:
                throw new Exception("Ranged Attack '" + ranged.RangedType + "' not implemented.");
        }
        return true;
    }
}
