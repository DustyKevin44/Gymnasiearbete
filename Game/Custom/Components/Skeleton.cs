using System;
using System.Collections.Generic;
using System.Linq;
using Game.Custom.Experimental;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ECS;

namespace Game.Custom.Components;


public class ChainComponent(Vector2 anchor, Entity target, List<Joint> joints) : Chain(anchor, Vector2.Zero, joints)
{
    public Entity ETarget = target;
}


public class Skeleton(List<ChainComponent> limbs)
{
    public List<ChainComponent> Limbs = limbs;

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        foreach (var limb in Limbs)
        {
            limb.Draw(gameTime, spriteBatch);
        }
    }
}