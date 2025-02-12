using Microsoft.Xna.Framework;

namespace Game.Custom.Components;

public class TransformComponent(Vector2 position, float rotation = 0f, Vector2 scale = default)
{
    public Vector2 Position = position;
    public float Rotation = rotation;
    public Vector2 Scale = scale == default ? Vector2.One : scale;
}
