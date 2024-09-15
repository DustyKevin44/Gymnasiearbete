using System;


namespace SpeletGymnasiearbete.Classes;

// Vector2 that sticks to another Vector2
public class StickyVector2(
    Func<Microsoft.Xna.Framework.Vector2> getBaseValue,
    Microsoft.Xna.Framework.Vector2 offset,
    Func<Microsoft.Xna.Framework.Vector2, Microsoft.Xna.Framework.Vector2, Microsoft.Xna.Framework.Vector2> combine
) : Sticky<Microsoft.Xna.Framework.Vector2>(getBaseValue, offset, combine) {}

