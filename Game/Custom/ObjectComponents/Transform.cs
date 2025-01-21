using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
namespace Game.Custom.ObjectComponents;

public class TransformComponent
{
    public Vector2 Position { get; set; } // Positionen
    public float Rotation { get; set; }
    public Vector2 Scale { get; set; }

    public TransformComponent(Vector2 position, float rotation = 0f, Vector2 scale = default)
    {
        Position = position;
        Rotation = rotation;
        Scale = scale == default ? Vector2.One : scale;
    }
}