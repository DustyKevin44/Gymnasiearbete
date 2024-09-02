using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static SpeletGymnasiearbete.Utils;


namespace SpeletGymnasiearbete.Classes;

public class Bullet : Sprite  // TODO: create Projetile class to inherit from
{
    public Vector2 Velocity { get; set; }

    public Bullet(Texture2D texture, Vector2 position) : base(texture, position) { Velocity = Vector2.Zero; }
    public Bullet(Texture2D texture, Vector2 position, Vector2 velocity) : base(texture, position) { Velocity = velocity; }

    public new void Update(GameTime gameTime)
    {
        // Update position
        Position += Velocity * GameTimeToDelta(gameTime);

        // Check if fully outside window, TODO?: create global window_size that updates on window_resize
        Vector2 camera_pos = (Globals.Active_Camera is Camera camera) ? camera.Position : new Vector2();
        Vector2 window_size = Globals.GraphicsDeviceManager.GraphicsDevice.PresentationParameters.Bounds.Size.ToVector2() + camera_pos;
        if (Position.X <= camera_pos.X - Texture.Bounds.Width  || Position.X >= window_size.X ||
            Position.Y <= camera_pos.Y - Texture.Bounds.Height || Position.Y >= window_size.Y)
        {
            Queue_kill();  // suicide
        }
    }
}
