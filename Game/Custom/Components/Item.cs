using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Graphics;

namespace Game.Custom.Components;


public class Item
{
    public Texture2D Icon;
    public bool InInventory;
}

public class Stack(int stackSize)
{
    public Item[] items = new Item[stackSize];
    public int StackSize => items.Length;
}

public class Inventory
{
    public Stack[,] inventory;
    public float PickupRange;
    public bool IsAutoPickupActive;
}

public class Effect
{
    public int Type;
}

public class Equipable
{
    public bool IsEquiped;
    public Effect buff;
}

public class Ability
{
    public string Name;
}

public abstract class Attack
{
    public string Name;
    public float Damage;
    public float Cooldown;
    public float LastTimeUsed;
}

public class MeleeAttack : Attack;

public class RangedAttack : Attack
{
    public Projectile[] Projectiles;
    public float Range;
}

public class Projectile
{
    public int Id;
}
