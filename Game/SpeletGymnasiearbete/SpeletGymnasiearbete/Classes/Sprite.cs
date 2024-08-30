using Microsoft.Xna.Framework.Graphics;
#nullable enable


using static SpeletGymnasiearbete.Utils;
namespace SpeletGymnasiearbete.Classes;

public class Sprite : IGameObject
{
    public bool Object_is_dying { get; private set; } = false;
    public Texture2D? Texture;
    public IVector2 Position;
    public Microsoft.Xna.Framework.Color Tint = Microsoft.Xna.Framework.Color.White;

    public Sprite(Texture2D? texture, IVector2 position) { Texture = texture; Position = position; }
    public Sprite(Texture2D? texture, IVector2 position, Microsoft.Xna.Framework.Color tint) { Texture = texture; Position = position; Tint = tint; }

    public void Queue_kill() { Object_is_dying = true; }

    public void Update(Microsoft.Xna.Framework.GameTime gameTime) {}

    public void Draw()
    {
        if (Texture is not null)
        {
            Globals.SpriteBatch.Begin();
            Globals.SpriteBatch.Draw(Texture, Position.Value, Tint);
            Globals.SpriteBatch.End();
        }
    }
}
