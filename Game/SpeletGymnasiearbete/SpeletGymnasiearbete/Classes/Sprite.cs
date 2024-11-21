using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

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
            Globals.SpriteBatch.Draw(Texture, Position - Vector2.UnitY * Z_offset - camera_pos, Tint);
        }
    }
}    public class AnimatedSprite : Sprite
{
    public Timer newTimer;         // Timer for animation frames
    public int AnimationLength;    // Total number of animation frames
    public int currentFrame = 0;   // Current frame index

    public float frameTick;        // Time per frame in milliseconds

    public AnimatedSprite(Texture2D? texture, Vector2 position, int animationLength, float frameTick, bool repeat)
        : base(texture, position)
    {
        AnimationLength = animationLength;
        this.frameTick = frameTick;
        
        // Timer is set to the time for a single frame
        newTimer = new Timer(frameTick, repeat);
    }

    public new void Update(GameTime gameTime)
    {
        // Update the frame timer
        newTimer.Update(gameTime);

        // Check if it's time to advance to the next frame
        if (newTimer.Finished)
        {
            currentFrame++;

            // Loop back to the first frame if we've reached the end
            if (currentFrame >= AnimationLength)
            {
                currentFrame = 0;
            }

            // Restart the timer for the next frame
            newTimer.StartTimer();
        }
    }

    public void Draw(GameTime gameTime)
    {
        if (Texture is not null)
        {
            // Calculate the width of a single frame
            int frameWidth = Texture.Width / AnimationLength;

            // Define the source rectangle for the current frame
            Rectangle sourceRectangle = new Rectangle(currentFrame * frameWidth, 0, frameWidth, Texture.Height);

            // Draw the current frame
            Globals.SpriteBatch.Draw(
                Texture,
                Position - Globals.Active_Camera.Position,
                sourceRectangle,       // Source rectangle
                Color.White,           // Color tint
                0f,                    // Rotation
                Vector2.Zero,          // Origin
                1f,                    // Scale
                SpriteEffects.None,    // Sprite effect
                0f                     // Layer depth
            );
        }
    }
}



