using static SpeletGymnasiearbete.Node2D;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
#nullable enable

namespace SpeletGymnasiearbete;

public class Sprite2D : Node2D
{
    public Sprite2D(Vector2? position = null,
                    float rotation = 0f,
                    Vector2? scale = null,
                    List<Node>? children = null
    ) : base(position, rotation, scale, children) {}

    public Texture2D? Texture;
}