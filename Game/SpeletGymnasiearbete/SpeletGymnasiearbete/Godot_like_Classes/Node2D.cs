using System;
using Microsoft.Xna.Framework;
using static SpeletGymnasiearbete.Node;

namespace SpeletGymnasiearbete;

public class Node2D : Node
{
    private Vector2 _position = Vector2.Zero;
    private Vector2 _scale = Vector2.One;
    private double _rotation = 0d;


    // Make properties scalable
    private T PropegateThroughAncestors<T, BASE>(Func<BASE, T> getValue, T localValue, Func<T, T, T> combine)
    {
        if (get_parent() is BASE baseType) { return combine(localValue, getValue(baseType)); }
        return localValue;
    }

    public Vector2 Position {
        get => PropegateThroughAncestors<Vector2, Node2D>(node => node.Position, _position, (local, parent) => local + parent);
        set => _position = value;
    }

    public Vector2 Scale {
        get => PropegateThroughAncestors<Vector2, Node2D>(node => node.Scale, _scale, (local, parent) => local * parent);
        set => _scale = value;
    }

    public double Rotation {
        get => PropegateThroughAncestors<double, Node2D>(node => node.Rotation, _rotation, (local, parent) => local + parent);
        set => _rotation = value;
    }

    private void draw() {}

    private void process(double delta) {}
    
    private void physics_process(double delta) {}
}