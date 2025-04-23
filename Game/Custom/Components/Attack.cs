using System;
using Game.Custom.Static;
using Microsoft.Xna.Framework;

namespace Game.Custom.Components;


public abstract class Attack(float damage, float cooldown)
{
    public float Damage = damage;
    public float Cooldown = cooldown;
    public float previousTimeUsed;
    public int ComboCounter = 0;

    public bool IsOffCooldown(GameTime gameTime) => (gameTime.TotalGameTime.Seconds - previousTimeUsed) >= Cooldown;
}


public class MeleeAttack(float damage, float cooldown, MeleeType type) : Attack(damage, cooldown)
{
    public bool OnLeftSide = true; // For attacks that swap the side the weapon is on
    public readonly MeleeType MeleeType = type;
}


public class RangedAttack(float damage, float range, float speed, float cooldown, RangedType type) : Attack(damage, cooldown)
{
    public readonly RangedType RangedType = type;
    public float Range = range;
    public float Speed = speed;
}


public class UniqueAttack(float damage, float cooldown, Action<UniqueAttack> attack) : Attack(damage, cooldown)
{
    public Action<UniqueAttack> Attack = attack;
}

