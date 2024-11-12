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
            Globals.SpriteBatch.Begin();
            Globals.SpriteBatch.Draw(Texture, Position - Vector2.UnitY * Z_offset - camera_pos, Tint);
            Globals.SpriteBatch.End();
        }
    }
}    
public class AnimatedSprite : Sprite{
    public Timer newTimer;
    public int AnimationLength; // Hur många bilder animationen är
    public int currentFrame = 0; // Hehe -- 8=======D  O:
    
        public float frameTick; // Ifall lika med 1000 så byter den bild efter en sekund / Tid per bild i millisekunder
    public AnimatedSprite(Texture2D? texture, Vector2 position, int TheLengthOfAnimationAmount, float FrameTick, bool repeat) : base(texture, position) 
    {
        AnimationLength = TheLengthOfAnimationAmount;
        frameTick = FrameTick;
        newTimer = new Timer(frameTick*AnimationLength, repeat);
    }   
    public new void Update(GameTime gametime){
        newTimer.Update(gametime);
        if (newTimer.Finished){
            currentFrame++;
            if( currentFrame == AnimationLength){
                currentFrame = 0;
            }
        }
    }
    public void Draw(GameTime gametime)
    {
        Globals.SpriteBatch.Draw(Texture, Position- Globals.Active_Camera.Position, new Rectangle(currentFrame * Texture.Bounds.X / AnimationLength, 0, Texture.Bounds.X / AnimationLength, Texture.Bounds.Y), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

    }
}

