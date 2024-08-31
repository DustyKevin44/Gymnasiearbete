using Microsoft.Xna.Framework.Graphics;
using static SpeletGymnasiearbete.Utils;


namespace SpeletGymnasiearbete.Classes;

public class Bullet : Sprite  // TODO: create Projetile class to inherit from
{
    public IVector2 Velocity { get; set; }

    public Bullet(Texture2D texture, IVector2 position) : base(texture, position) { Velocity = new Vector2(Microsoft.Xna.Framework.Vector2.Zero); }
    public Bullet(Texture2D texture, IVector2 position, IVector2 velocity) : base(texture, position) { Velocity = velocity; }

    public new void Update(Microsoft.Xna.Framework.GameTime gameTime)
    {
        // Update position
        Position.Value += Velocity.Value * GameTimeToDelta(gameTime);

        // Check if fully outside window, TODO?: create global window_size that updates on window_resize
        Microsoft.Xna.Framework.Vector2 window_size = Globals.GraphicsDeviceManager.GraphicsDevice.PresentationParameters.Bounds.Size.ToVector2();
        if (Position.Value.X <= 0 - Texture.Bounds.Width  || Position.Value.X >= window_size.X ||
            Position.Value.Y <= 0 - Texture.Bounds.Height || Position.Value.Y >= window_size.Y)
        {
            Queue_kill();  // suicide
        }
    }
}
