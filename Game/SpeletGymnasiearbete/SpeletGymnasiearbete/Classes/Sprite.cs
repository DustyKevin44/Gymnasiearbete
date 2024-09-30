using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#nullable enable


using static SpeletGymnasiearbete.Utils;
namespace SpeletGymnasiearbete.Classes;

public class Sprite : IGameObject
{
    public bool Object_is_dying { get; private set; } = false;
    public Texture2D? Texture;
    public Vector2 Position;
    public Color Tint = Color.White;
    public float Z_offset = 0f;

    public Sprite(Texture2D? texture, Vector2 position) { Texture = texture; Position = position; }
    public Sprite(Texture2D? texture, Vector2 position, Color tint) { Texture = texture; Position = position; Tint = tint; }

    public void Queue_kill() { Object_is_dying = true; }

    public void Update(GameTime gameTime) {}

    public void Draw()
    {
        if (Texture is not null)
        {
            Vector2 camera_pos = (Globals.Active_Camera is Camera camera) ? camera.Position : new Vector2();
            Globals.SpriteBatch.Begin();
            Globals.SpriteBatch.Draw(Texture, Position - Vector2.UnitY * Z_offset - camera_pos, Tint);
            Globals.SpriteBatch.End();
        }
    }
}
