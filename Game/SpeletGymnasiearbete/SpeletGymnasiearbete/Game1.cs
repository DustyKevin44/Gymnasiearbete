using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpeletGymnasiearbete.Classes;

using static SpeletGymnasiearbete.Utils;  // Globals
namespace SpeletGymnasiearbete;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    // Player
    private Sprite Player;
    private float _player_speed = 400f;
    // Bullet
    private List<Bullet> _bullets = [];
    private Texture2D bullet_sprite;
    private float _bullet_speed = 400f;
    private Timer _bullet_cooldown = new(1, false);

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.AllowUserResizing = true;
    }

    protected override void Initialize()
    {
        Player = new Sprite(null, new Classes.Vector2(_graphics.GraphicsDevice.PresentationParameters.Bounds.Center.ToVector2()));
        _bullet_cooldown.StartTimer();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        Globals.SetSpriteBatch(new SpriteBatch(GraphicsDevice));
        Globals.SetContentManager(Content);
        Globals.SetGraphicsDeviceManager(_graphics);

        Player.Texture = Globals.ContentManager.Load<Texture2D>("Player-1");
        bullet_sprite = Globals.CreateTexture(10, 10, Color.OrangeRed, Globals.CircleShader);
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState keyboard = Keyboard.GetState();
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboard.IsKeyDown(Keys.Escape))
            Exit();

        // handle directional input and move player, TODO: controller support
        Microsoft.Xna.Framework.Vector2 direction = new(
            x: (keyboard.IsKeyDown(Keys.D) ? 1f : 0f) - (keyboard.IsKeyDown(Keys.A) ? 1f : 0f),
            y: (keyboard.IsKeyDown(Keys.S) ? 1f : 0f) - (keyboard.IsKeyDown(Keys.W) ? 1f : 0f));
        if (direction.Length() != 0) direction.Normalize();
        Player.Position.Value += direction * _player_speed * GameTimeToDelta(gameTime);

        // Update timer
        _bullet_cooldown.Update(gameTime);

        // Shoot bullets, TODO: controller support
        MouseState mouse = Mouse.GetState();
        if (mouse.LeftButton == ButtonState.Pressed && _bullet_cooldown.Finished)
        {
            Microsoft.Xna.Framework.Vector2 bullet_dir = mouse.Position.ToVector2() - Player.Position.Value;
            bullet_dir.Normalize();
            _bullets.Add(new Bullet(bullet_sprite, new Classes.Vector2(Player.Position.Value + Player.Texture.Bounds.Size.ToVector2() / 2), new Classes.Vector2(bullet_dir * _bullet_speed)));
            _bullet_cooldown.StartTimer();
        }

        // Update every bullet and remove dead ones
        _bullets.RemoveAll((bullet) => {
            bullet.Update(gameTime);
            return bullet.Object_is_dying;
        });

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        _graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

        // Draw player
        Player.Draw();
        
        // Draw bullets
        foreach(Sprite bullet in _bullets) { bullet.Draw(); }

        base.Draw(gameTime);
    }
}
