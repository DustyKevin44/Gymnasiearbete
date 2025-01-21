using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using Game.Custom.Input;
using Game.Custom.States;


namespace Game.Custom.ObjectComponents;

public class SpriteComponent
{
    public Texture2D Texture { get; set; }         // The texture to render
    public Rectangle? SourceRectangle { get; set; } // Source rectangle for texture atlas (nullable)
    public Color Color { get; set; }              // Tint color for the sprite
    public Vector2 Origin { get; set; }           // Origin point for rotation/scaling
    public float Rotation { get; set; }           // Rotation angle in radians
    public float Scale { get; set; }              // Uniform scale factor
    public SpriteEffects Effects { get; set; }    // Sprite effects (e.g., flip horizontally/vertically)
    public float LayerDepth { get; set; }         // Depth for rendering order (0 = front, 1 = back)

    public SpriteComponent(Texture2D texture, Rectangle? sourceRectangle = null, Color? color = null)
    {
        Texture = texture;
        SourceRectangle = sourceRectangle;
        Color = color ?? Color.White;
        Origin = Vector2.Zero;
        Rotation = 0f;
        Scale = 1f;
        Effects = SpriteEffects.None;
        LayerDepth = 0f;
    }
}
