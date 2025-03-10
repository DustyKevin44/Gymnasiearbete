using System;
using System.Collections.Generic;
using MonoGame.Extended;
using MonoGame.Extended.ECS;

namespace Game.Custom.Components;


public class Equipable(Transform2 localTransform = null, Action<float> effect = null)
{
    public Entity Entity;
    public Transform2 LocalTransform = localTransform ?? new();
    public Action<float> Effect = effect;
}


public class EquipmentSlot
{
    public Entity Entity;
    public bool IsLocked = false;
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

    public bool Equip(string slot, Entity entity)
    {
        if (_equipment.ContainsKey(slot)) return false;
        if (_equipment[slot] is not null) return false;
        if (!entity.Has<Equipable>()) return false;
        _equipment[slot].Entity = entity;

        return true;
    }

    public bool TryGet(string slot, out Entity entity)
    {
        if (_equipment.TryGetValue(slot, out EquipmentSlot eq))
        {
            entity = eq.Entity;
            return true;
        }
        entity = null;
        return false;
    }

    public bool TryUnequip(string slot, out Entity entity)
    {
        if (TryGet(slot, out entity))
        {
            _equipment[slot].Entity = null;
            return true;
        }
        return false;
    }

    public Entity Replace(string slot, Entity replacement)
    {
        if (!_equipment.TryGetValue(slot, out EquipmentSlot value)) throw new Exception("Equipment slot '" + slot + "' does not exist.");
        var old = value.Entity;
        value.Entity = replacement;
        return old;
    }
}
