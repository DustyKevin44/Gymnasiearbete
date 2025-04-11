using Game.Custom.Components;
using MonoGame.Extended;
using Game.Custom.Utilities;
using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;

namespace Game.Custom.Static;


public enum MeleeType
{
    Slash,
}

public static class Melee
{
    public static bool Attack(Entity entity)
    {
        if (!entity.Has<MeleeAttack>()) return false; // Weapon is not melee weapon
        if (!entity.Has<Transform2>()) return false; // Weapon cannot be used (swing, stab, etc.)
        if (!entity.Has<HitBox>()) return false; // Weapon cannot hit

        var melee = entity.Get<MeleeAttack>();
        var transform = entity.Get<Transform2>(); // Make sure that the world matrix is the parent entites transform
        var hitbox = entity.Get<HitBox>();

        hitbox.EmptyCollisions(); // Remove all the collisions that happened last attack
        hitbox.IsEnabled = true; // Enable collisions

        /* Rules:
         *   1. melee.Cooldown must be larger than the MeleeTypes tween duration
         *   2. transform should be relative the owner of the weapon
         */
        var tweener = Utils.GetTweener(entity);
        switch (melee.MeleeType)
        {
            case MeleeType.Slash: // Basic swipe from 45 to -45 degrees, where 0 is facing forward in relative to the parent entity
                if (melee.OnLeftSide)
                {
                    // Change local rotation and position to right side
                    tweener.TweenTo(transform, t => t.Rotation, MathHelper.ToRadians(-45), 0.1f)
                        .OnEnd(_ => { hitbox.IsEnabled = false; melee.OnLeftSide = false; });
                }
                else
                {
                    // Change local rotation and position to left side
                    tweener.TweenTo(transform, t => t.Rotation, MathHelper.ToRadians(45), 0.1f)
                        .OnEnd(_ => { hitbox.IsEnabled = false; melee.OnLeftSide = true; });
                }
                break;
            default:
                throw new System.Exception("Melee Attack '" + melee.MeleeType + "' not implemented.");
        }

        return true;
    }
}
