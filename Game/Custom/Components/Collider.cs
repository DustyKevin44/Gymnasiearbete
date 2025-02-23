using MonoGame.Extended;
using MonoGame.Extended.ECS;
using Microsoft.Xna.Framework;

public class ColliderComponent
{
    public RectangleF Bounds;
    
    public ColliderComponent(float width, float height)
    {
        Bounds = new RectangleF(0, 0, width, height);
    }
}