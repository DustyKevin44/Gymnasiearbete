using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Custom.Components;


public class Item(Texture2D icon)
{
    public Texture2D Icon = icon;
    public bool InInventory = false;
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
            item.InInventory = true;
            return true;
        }

        return false;
    }
}
