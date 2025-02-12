using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Game.Custom.Components;

public class SpriteComponent(Texture2D texture, Rectangle? sourceRectangle = null, Color? color = null, Vector2? origin = null)
{
    public Texture2D Texture = texture;                     // The texture to render
    public Rectangle? SourceRectangle = sourceRectangle;    // Source rectangle for texture atlas (nullable)
    public Color Color = color ?? Color.White;              // Tint color for the sprite
    public Vector2 Origin = origin ?? Vector2.Zero;         // Origin point for rotation/scaling
    public float Rotation = 0f;                             // Rotation angle in radians
    public float Scale = 1f;                                // Uniform scale factor
    public SpriteEffects Effects = SpriteEffects.None;      // Sprite effects (e.g., flip horizontally/vertically)
    public float LayerDepth = 0f;                           // Depth for rendering order (0 = front, 1 = back)
}
