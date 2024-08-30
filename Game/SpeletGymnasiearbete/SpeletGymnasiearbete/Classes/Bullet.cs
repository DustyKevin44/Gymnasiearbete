using Microsoft.Xna.Framework.Graphics;
using static SpeletGymnasiearbete.Utils;


namespace SpeletGymnasiearbete.Classes;

public class Bullet : Sprite
{
    public IVector2 Velocity { get; set; }

    public Bullet(Texture2D texture, IVector2 position) : base(texture, position) {}
    public Bullet(Texture2D texture, IVector2 position, IVector2 velocity) : base(texture, position) { Velocity = velocity; }

    public new void Update(Microsoft.Xna.Framework.GameTime gameTime)
    {
        Position.Value += Velocity.Value * GameTimeToDelta(gameTime);

        Microsoft.Xna.Framework.Vector2 window_size = Globals.GraphicsDeviceManager.GraphicsDevice.PresentationParameters.Bounds.Size.ToVector2();
        if (Position.Value.X <= 0 - Texture.Bounds.Width  || Position.Value.X >= window_size.X ||
            Position.Value.Y <= 0 - Texture.Bounds.Height || Position.Value.Y >= window_size.Y)
        {
            Queue_kill();  // suicide
        }
    }
}
