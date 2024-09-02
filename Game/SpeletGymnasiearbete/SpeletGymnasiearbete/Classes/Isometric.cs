using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using static SpeletGymnasiearbete.Utils;
namespace SpeletGymnasiearbete.Classes;

public class IsometricGrid(Vector2 position, Texture2D missing_texture, int width, int height, int tileW, int tileH) : IGameObject
{
    public bool Object_is_dying { get; private set; } = false;
    private readonly int _width = width;
    private readonly int _height = height;
    private readonly int _tileW = tileW;
    private readonly int _tileH = tileH;
    public Texture2D _missing_texture = missing_texture;
    public Vector2 Position = position;

    public void Queue_kill() { Object_is_dying = true; }
    public void Update(GameTime gameTime) {}

    public void Draw()
    {
        Vector2 camera_pos = (Globals.Active_Camera is Camera camera) ? camera.Position : new Vector2();
        Globals.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        for(int x = 0; x < _width; x++)
        {
            for(int y = 0; y < _height; y++)
            {
                Vector2 IsoPos = CartesianToIsometric(new(x, y));
                IsoPos -= camera_pos;
                Globals.SpriteBatch.Draw(_missing_texture, new Rectangle((int)(IsoPos.X + Position.X), (int)(IsoPos.Y + Position.Y), _tileW, _tileH), Color.White);
            }
        }

        Globals.SpriteBatch.End();
    }

    public Vector2 CartesianToIsometric(Vector2 cartesian)
    {
        return new Vector2(
            (cartesian.X - cartesian.Y) * (_tileW / 2),
            (cartesian.X + cartesian.Y) * (_tileH / 4)
        );
    }

    public Microsoft.Xna.Framework.Point ScreenToCartesian(Vector2 screen)
    {
        return new Microsoft.Xna.Framework.Point(
            (int)((screen.X / (_tileW / 2f) + screen.Y / (_tileH / 4f)) / 2f),
            (int)((screen.Y / (_tileW / 2f) + screen.X / (_tileW / 4f)) / 2f)
        );
    }
}




