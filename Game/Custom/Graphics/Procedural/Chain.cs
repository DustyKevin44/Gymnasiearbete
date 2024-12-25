using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Game.Custom.Graphics.Procedural;


public class Chain(List<Joint> joints)
{
    private List<Joint> _joints = joints;

    public Joint Head { get => _joints.First(); }
    public Vector2 Position { get => Head.Position; set => Head.Position = value; }

    public void Draw(GameTime _, SpriteBatch spriteBatch)
    {
        for (int i = 0; i < _joints.Count - 1; i++)
        {
            var A = _joints.ElementAt(i);
            var B = _joints.ElementAt(i + 1);
            spriteBatch.DrawCircle(new CircleF(A.Position, A.Length), 32, A.Color, A.Thickness);
            spriteBatch.DrawLine(A.Position, B.Position, A.Color, A.Thickness);
        }
    }

    public void Update(GameTime _)
    {
        for (int i = 1; i < _joints.Count; i++)
        {
            var parent = _joints[i - 1];
            var current = _joints[i];

            // Pass the parent's position to the current joint for updating
            current.Update(parent.Position);
        }
    }
}

public class Joint(Vector2 position, float length, Color? color = null)
{
    public Vector2 Position { get; set; } = position;
    public float Length { get; set; } = length;
    public float Thickness { get; set; } = 2f;
    public Color Color { get; set; } = color ?? Color.Red;

    public virtual void Update(Vector2 parentPosition)
    {
        var dir = Position - parentPosition;
        dir.Normalize();

        Position = parentPosition + dir * Length;
    }
}
