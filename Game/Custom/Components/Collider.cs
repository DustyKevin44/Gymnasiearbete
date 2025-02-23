using MonoGame.Extended;
using System;

namespace Game.Custom.Components;


public abstract class ColliderBox<Layer>(IShapeF shape, Layer layers, bool isStatic) where Layer : Enum
{
    public IShapeF Shape = shape;
    public Layer Layers = layers;
    public bool IsStatic = isStatic;
}

public class HitBox<Layer>(IShapeF shape, Layer layers, bool isStatic) : ColliderBox<Layer>(shape, layers, isStatic) where Layer : Enum;
public class HurtBox<Layer>(IShapeF shape, Layer layers, bool isStatic) : ColliderBox<Layer>(shape, layers, isStatic) where Layer : Enum;
public class CollisionBox<Layer>(IShapeF shape, Layer layers, bool isStatic) : ColliderBox<Layer>(shape, layers, isStatic) where Layer : Enum;
