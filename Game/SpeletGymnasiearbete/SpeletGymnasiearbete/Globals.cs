using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace SpeletGymnasiearbete;

public sealed class Utils
{
    private static Utils _global = null;
    private static readonly object _lock = new();

    public ContentManager ContentManager { get; private set; }
    public SpriteBatch SpriteBatch { get; private set; }
    public GraphicsDeviceManager GraphicsDeviceManager { get; private set; }

    public static Utils Globals { get { lock (_lock) { _global ??= new Utils(); return _global; } } }

    public void SetContentManager(ContentManager contentManager) { ContentManager = contentManager; }
    public void SetSpriteBatch(SpriteBatch spriteBatch) { SpriteBatch = spriteBatch; }
    public void SetGraphicsDeviceManager(GraphicsDeviceManager graphicsDeviceManager) { GraphicsDeviceManager = graphicsDeviceManager; }

    public static float GameTimeToDelta(GameTime gameTime) { return (float)gameTime.ElapsedGameTime.TotalSeconds; }

    public Texture2D CreateTexture(int width, int height, Color color, Func<int, int, int, int, Color, Color> Shader = null)
    {
        Texture2D texture = new(GraphicsDeviceManager.GraphicsDevice, width, height);
        Color[] colorData = new Color[width * height];
        for(int x=0; x<width; x++) {
            for(int y=0; y<height; y++) {
                colorData[x + y*width] = Shader is null ? color : Shader(x, y, width, height, color);
            }
        }
        texture.SetData(colorData);
        return texture;
    }

    public Func<int, int, int, int, Color, Color> CircleShader = (int x, int y, int width, int height, Color color) =>
    {
        float radius = Math.Min(width, height) / 2;
        float dx = x - width / 2f;
        float dy = y - height / 2f;
        float distanceSqrd = dx * dx + dy * dy;

        return distanceSqrd <= radius * radius ? color : Color.Transparent;
    };
}