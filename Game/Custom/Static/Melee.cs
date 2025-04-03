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
        System.Console.WriteLine(hitbox._hurtBoxesHit.Count);

        /* Rules:
         *   1. melee.Cooldown must be larger than the MeleeTypes tween duration
         *   2. transform should be relative the owner of the weapon
         */

        switch (melee.MeleeType)
        {
            case MeleeType.Slash: // Basic swipe from 45 to -45 degrees, where 0 is facing forward in relative to the parent entity or world
                var tweener = Utils.GetTweener(entity);
                if (melee.OnLeftSide)
                {
                    // Change local rotation and position to right side
                    tweener.TweenTo(transform, (t) => t.Rotation, MathHelper.ToRadians(225), 1).OnEnd((_) => { melee.OnLeftSide = false; });
                    tweener.TweenTo(transform, (t) => t.Position, new Vector2(10, 0), 1);
                }
                else
                {
                    // Change local rotation and position to left side
                    tweener.TweenTo(transform, (t) => t.Rotation, MathHelper.ToRadians(-45), 1).OnEnd((_) => { melee.OnLeftSide = true; });
                    tweener.TweenTo(transform, (t) => t.Position, new Vector2(-10, 0), 1);
                }
                break;
            default:
                throw new System.Exception("Melee Attack '" + melee.MeleeType + "' not implemented.");
        }

        return true;
    }
}
