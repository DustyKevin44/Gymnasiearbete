using static SpeletGymnasiearbete.Node2D;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SpeletGymnasiearbete;

public class Sprite2D : Node2D
{
    private Texture2D _texture;

    public Texture2D Texture {
        get => _texture;
        set { if (value is Texture2D) { _texture = value; }}
    }
}