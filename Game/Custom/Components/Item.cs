using System;
using System.Collections.Generic;
using System.Linq;
using Game.Custom.Static;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.ECS;


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


public class Inventory(int width, int height)
{
    public Stack[,] inventory = new Stack[width, height];
    public float PickupRange;
    public bool IsAutoPickupActive;

    public bool TryAddItem(int x, int y, Item item)
    {
        if (inventory.GetLength(0) >= x) return false;
        if (inventory.GetLength(1) >= y) return false;
        Stack stack = inventory[x,y];
        if (stack.items.Length >= stack.StackSize) return false;
        if (stack.items.Length == 0 || stack.items.First().GetType() == item.GetType())
        {
            stack.items[stack.items.Length] = item;
            return true;
        }
        return false;
    }
}


public class Equipable(Action<float> effect)
{
    public Action<float> Effect = effect;
}


public class EquipmentSlot
{
    public Equipable Equipable;
    public bool IsLocked = false;
}

public class Weapon : Equipable
{
    public int WeaponId;

    public Weapon(int weaponId, Action<float> passiveEffect) : base(passiveEffect)
    {
        var weapon = Global.World.GetEntity(weaponId) ?? throw new Exception("Entity does not exist.");
        if (weapon.Get<MeleeAttack>() is null && weapon.Get<RangedAttack>() is null) throw new Exception("Entity does not have a melee nor a ranged Attack.");
        WeaponId = weaponId;
    }
}

public class Equipment
{
    private readonly Dictionary<string, EquipmentSlot> _equipment = [];
    public int Slots;

    public Equipment(List<string> slots)
    {
        Slots = slots.Count;
        foreach (string slot in slots)
            _equipment.Add(slot, new EquipmentSlot());
    }

    public void AddEquipmentSlot(string slot)
    {
        if (_equipment.ContainsKey(slot)) throw new Exception("Equipment slot '" + slot + "' already exists.");
        Slots++;
        _equipment.Add(slot, new EquipmentSlot());
    }

    public bool Equip(string slot, Equipable equipable)
    {
        if (_equipment.ContainsKey(slot)) return false;
        if (_equipment[slot] is not null) return false;
        _equipment[slot].Equipable = equipable;
        return true;
    }

    public bool TryGet(string slot, out Equipable equipable)
    {
        if (_equipment.TryGetValue(slot, out EquipmentSlot eq))
        {
            equipable = eq.Equipable;
            return true;
        }
        equipable = null;
        return false;
    }

    public bool TryUnequip(string slot, out Equipable equipable)
    {
        if (TryGet(slot, out equipable))
        {
            _equipment[slot].Equipable = null;
            return true;
        }
        return false;
    }

    public Equipable Replace(string slot, Equipable replacement)
    {
        if (!_equipment.TryGetValue(slot, out EquipmentSlot value)) throw new Exception("Equipment slot '" + slot + "' does not exist.");
        var old = value.Equipable;
        value.Equipable = replacement;
        return old;
    }
}

public abstract class Attack
{
    public float Damage;
    public float Cooldown;
    public float previousTimeUsed;

    public bool IsOffCooldown(GameTime gameTime) => (gameTime.TotalGameTime.Seconds - previousTimeUsed) >= Cooldown;
}

public class MeleeAttack : Attack
{
    public readonly MeleeId MeleeId;
}

public class RangedAttack : Attack
{
    public readonly RangedId RangedId;
    public float Range;
}

public class UniqueAttack : Attack
{
    public Action<UniqueAttack> attack;
}
