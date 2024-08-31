using System;


namespace SpeletGymnasiearbete.Classes;

// Base Vector2
public interface IVector2
{
    Microsoft.Xna.Framework.Vector2 Value { get; set; }
}

// Wrapper for Xna Vector2 to make StickyVector2 compatible
public class Vector2 : IVector2
{
    public Microsoft.Xna.Framework.Vector2 Value { get; set; }

    public Vector2() { Value = new Microsoft.Xna.Framework.Vector2(); }
    public Vector2(Microsoft.Xna.Framework.Vector2 other) { Value = other; }
    public Vector2(float v) { Value = new Microsoft.Xna.Framework.Vector2(value: v); }
    public Vector2(float x, float y) { Value = new Microsoft.Xna.Framework.Vector2(x: x, y: y); }
}

// Vector2 that sticks to another Vector2
public class StickyVector2(
    Func<Microsoft.Xna.Framework.Vector2> getBaseValue,
    Microsoft.Xna.Framework.Vector2 offset,
    Func<Microsoft.Xna.Framework.Vector2, Microsoft.Xna.Framework.Vector2, Microsoft.Xna.Framework.Vector2> combine
) : Sticky<Microsoft.Xna.Framework.Vector2>(getBaseValue, offset, combine), IVector2 {}

