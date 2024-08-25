using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using static SpeletGymnasiearbete.Node;
#nullable enable

namespace SpeletGymnasiearbete;

public class Node2D : Node
{
    private Vector2 _position;
    private Vector2 _scale;
    private float _rotation;

    public Node2D(Vector2 position, float rotation = 0f, Vector2? scale = null, List<Node>? children = null) : base(children)
    {
        _position = position;
        _scale = (scale is null) ? Vector2.One : (Vector2)scale;
        _rotation = rotation;
    }

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

    public float Rotation {
        get => PropegateThroughAncestors<float, Node2D>(node => node.Rotation, _rotation, (local, parent) => local + parent);
        set => _rotation = value;
    }
}