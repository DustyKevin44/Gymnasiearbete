using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using SpeletGymnasiearbete.Classes;
using SpeletGymnasiearbete.Classes.Tilemap;
using static SpeletGymnasiearbete.Utils;  // Globals and utilities

namespace SpeletGymnasiearbete;

public class Game1 : Game
{
    private readonly GraphicsDeviceManager _graphics;
    // Player
    private Sprite Player;
    private float _player_speed = 400f;
    // Bullet
    private readonly List<Bullet> _bullets = [];
    private Texture2D bullet_sprite;
    private readonly float _bullet_speed = 400f;
    private readonly Timer _bullet_cooldown = new(0.1f, false);
    // Isometric Grid
    private Texture2D tileset1;
    //private IsometricGrid _IsoGrid;
    
    // Isometric Tilemap
    private Dictionary<Vector2, int> _mapLayer1;
    private Dictionary<Vector2, int> _mapLayer2;
    private Dictionary<Vector2, int> _mapLayer3;
    private Dictionary<Vector2, int> _mapLayerCollision;
    private readonly Sprite _testCube = new(null, Vector2.Zero);
    private bool pressed = false;

    private readonly TileLayer _tilemap = new(new(32, 16), new(32, 32), new(100, 100), new(), null);


    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.AllowUserResizing = true;
    }

    protected override void Initialize()
    {
        // Create the player
        Player = new Sprite(null, Vector2.Zero);
        _bullet_cooldown.StartTimer();
        
        // Create the isometric grid
        //_IsoGrid = new(new Vector2(300, 200), null, 100, 5, 32*3, 32*3);
        
        // Create Camera
        Globals.Active_Camera = new(new Vector2());
        
        //Load tilemap
        //Dictionary<Vector2, int> tilemap = TilemapCode.LoadMap("./playgroundtilemap.tmx");
        _mapLayer1 = TilemapCode.LoadMap("../../../playgroundtilemap_Tile Layer 1.csv");
        _mapLayer2 = TilemapCode.LoadMap("../../../playgroundtilemap_Tile Layer 2.csv");
        _mapLayer3 = TilemapCode.LoadMap("../../../playgroundtilemap_Tile Layer 3.csv");
        _mapLayerCollision = TilemapCode.LoadMap("../../../playgroundtilemap_Collision.csv");

        base.Initialize();
    }

    protected override void LoadContent()
    {
        // Add useful properties to Globals
        Globals.SetSpriteBatch(new SpriteBatch(GraphicsDevice));
        Globals.SetContentManager(Content);
        Globals.SetGraphicsDeviceManager(_graphics);

        tileset1 = Globals.ContentManager.Load<Texture2D>("tilesetImage");
        
        // get test cube
        //_testCube.Texture = Globals.ContentManager.Load<Texture2D>("IsoDebugTile");
        
        // Load player Texture
        Player.Texture = Globals.ContentManager.Load<Texture2D>("Player-1");
        
        // Create new bullet Texture (OrangeRed circle with the radius 5)
        bullet_sprite = Globals.CreateTexture(10, 10, Color.OrangeRed, Globals.CircleShader);
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState keyboard = Keyboard.GetState();
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboard.IsKeyDown(Keys.Escape))
            Exit();

        Vector2 direction = Vector2.Zero;
        if (keyboard.IsKeyDown(Keys.W)) direction.Y += -1;
        if (keyboard.IsKeyDown(Keys.S)) direction.Y += 1;
        if (direction.Y == 0)
        {
            if (keyboard.IsKeyDown(Keys.A)) direction.X += -1;
            if (keyboard.IsKeyDown(Keys.D)) direction.X += 1;
        } else {
            if (keyboard.IsKeyDown(Keys.A)) direction.Rotate(1.107149611f * direction.Y);
            if (keyboard.IsKeyDown(Keys.D)) direction.Rotate(1.107149611f * -direction.Y);
        }

        // Update the player position
        Player.Position += direction * _player_speed * GameTimeToDelta(gameTime);

        // Update timer
        _bullet_cooldown.Update(gameTime);

        // Shoot bullets, TODO: controller support
        MouseState mouse = Mouse.GetState();
        
        if (mouse.LeftButton == ButtonState.Pressed)
        {
            if (!pressed)
            {
                Vector2 index = _tilemap.WorldToIso(mouse.Position.ToVector2() + Globals.Active_Camera.Position).ToVector2();
                if (_mapLayer2.TryGetValue(index, out int value))
                {
                    if (value < 11) _mapLayer2[index] = ++value;
                    else _mapLayer2.Remove(index);
                }
                else _mapLayer2[index] = 5;
            }
            pressed = true;
        }
        else pressed = false;

        if (false && _bullet_cooldown.Finished)
        {
            // Get the distance from the bullet to the mouse
            Vector2 bullet_dir = mouse.Position.ToVector2() + Globals.Active_Camera.Position - Player.Position - Player.Texture.Bounds.Size.ToVector2() / 2;
            // Get the direction of the distance Vector
            if (bullet_dir != Vector2.Zero) bullet_dir.Normalize();
            // if the direction is to nowhere (0, 0) then Random direction
            else { bullet_dir = Vector2.One; bullet_dir.Rotate(new Random().NextSingle() * MathHelper.TwoPi); }  // TODO: use generated seed instead of default

            // Create a Vector representing the direction to the mouse with the amplitude '_bullet_speed'
            Vector2 bullet_velo = bullet_dir * _bullet_speed;
            // Add some of the The relative velocity from the player to the bullet
            bullet_velo += direction * _player_speed / 2f;

            // Spawn a bullet at the Player position with a velocity of 'bullet_velo'
            _bullets.Add(new Bullet(bullet_sprite, Player.Position + Player.Texture.Bounds.Size.ToVector2() / 2, bullet_velo));
            // Start the cooldown so that it doesn't create a Bullet every frame the left mouse button is held
            _bullet_cooldown.StartTimer();
        }

        // Update every bullet
        _bullets.RemoveAll((bullet) => {
            bullet.Update(gameTime);
            // If Bullet is marked for deletion remove it from the list so that it can be collected by the garbage collector
            return bullet.Object_is_dying;
        });

        // Move camera
        Globals.Active_Camera.Position = Vector2.SmoothStep(
            Globals.Active_Camera.Position,
            Player.Position + (Player.Texture.Bounds.Size.ToVector2() - Globals.GraphicsDeviceManager.GraphicsDevice.PresentationParameters.Bounds.Size.ToVector2()) / 2,
            0.2f
        );

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        // Clear previous draw calls and fill the background
        _graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
        
        // Draw Isometric grid
        //_IsoGrid.Draw();
        
        Vector2 scale = new(4);
        _tilemap.scale = scale;

        Globals.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
        foreach (KeyValuePair<Vector2, int> item in _mapLayer1)
        {
            // Draw the tile at the given position
            Globals.SpriteBatch.Draw(tileset1, _tilemap.IsoToWorld(item.Key) - Globals.Active_Camera.Position, new Rectangle(item.Value*32, 0, 32, 32), Color.White, rotation: 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        foreach (KeyValuePair<Vector2, int> item in _mapLayer2)
        {
            // Draw the tile at the given position
            Globals.SpriteBatch.Draw(tileset1, _tilemap.IsoToWorld(item.Key) - Globals.Active_Camera.Position, new Rectangle(item.Value*32, 0, 32, 32), Color.White, rotation: 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        foreach (KeyValuePair<Vector2, int> item in _mapLayer3)
        {
            // Draw the tile at the given position
            Globals.SpriteBatch.Draw(tileset1, _tilemap.IsoToWorld(item.Key) - Globals.Active_Camera.Position, new Rectangle(item.Value*32, 0, 32, 32), Color.White, rotation: 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
        Globals.SpriteBatch.Draw(tileset1, _tilemap.IsoToWorld(_tilemap.WorldToIso(Mouse.GetState().Position.ToVector2() + Globals.Active_Camera.Position).ToVector2()) - Vector2.UnitY * _tilemap.Tile_size.Y - Globals.Active_Camera.Position, new Rectangle(0, 0, 32, 32), Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        Globals.SpriteBatch.End();

        // Draw test cube
        //Globals.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
        //Globals.SpriteBatch.Draw(_testCube.Texture, position: -camera_pos, null, Color.White, rotation: 0f, Vector2.UnitX * 50, Vector2.One*5, default, 0f);
        //Globals.SpriteBatch.End();

        // Draw player
        Player.Draw();
        // Draw bullets
        foreach(Sprite bullet in _bullets) { bullet.Draw(); }

        base.Draw(gameTime);
    }
}
