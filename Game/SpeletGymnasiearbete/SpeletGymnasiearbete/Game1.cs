using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Content;

using SpeletGymnasiearbete.Classes;

using static SpeletGymnasiearbete.Utils;
using System.Linq;  // Globals and utilities
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
    private TilemapCode _tilemapCode;
    private Dictionary<Vector2, int> _mapLayer1;
    private Dictionary<Vector2, int> _mapLayer2;
    private Dictionary<Vector2, int> _mapLayer3;
    private Dictionary<Vector2, int> _mapLayerCollision;
    private readonly Sprite _testCube = new(null, Vector2.Zero);
    private Vector2 camera_pos;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.AllowUserResizing = true;
    }

    protected override void Initialize()
    {
        // Create the player at the center of the screen with the sprite to be loaded later
        Player = new Sprite(null, _graphics.GraphicsDevice.PresentationParameters.Bounds.Center.ToVector2());
        _bullet_cooldown.StartTimer();
        
        // Create the isometric grid
        //_IsoGrid = new(new Vector2(300, 200), null, 100, 5, 32*3, 32*3);
        
        // Create Camera
        Globals.Active_Camera = new(new Vector2());
        
        // Initialize tilemap
        _tilemapCode = new TilemapCode();
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
        //_tilemapCode.LoadContent(GraphicsDevice, Content, "playgroundtilemap");

        // Load debug tile Texture
        //_IsoGrid._missing_texture = Globals.ContentManager.Load<Texture2D>("IsoDebugTile");
        
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
        if (direction.Y != 0)
        {
            if (keyboard.IsKeyDown(Keys.A))
            {
                direction.Rotate(1.107149611f * direction.Y);
            }
            if (keyboard.IsKeyDown(Keys.D))
            {
                direction.Rotate(1.107149611f * -direction.Y);
            }
        } else {
        if (keyboard.IsKeyDown(Keys.A))
            {
                direction.X += -1;
            }
            if (keyboard.IsKeyDown(Keys.D))
            {
                direction.X += 1;
            }
        }

        // Update the player position
        Player.Position += direction * _player_speed * GameTimeToDelta(gameTime);

        // Update timer
        _bullet_cooldown.Update(gameTime);

        // Shoot bullets, TODO: controller support
        MouseState mouse = Mouse.GetState();
        if (mouse.LeftButton == ButtonState.Pressed && _bullet_cooldown.Finished)
        {
            // Get the distance from the bullet to the mouse
            Vector2 camera_pos = (Globals.Active_Camera is Camera camera) ? camera.Position : new Vector2();
            Vector2 bullet_dir = mouse.Position.ToVector2() + camera_pos - Player.Position - Player.Texture.Bounds.Size.ToVector2() / 2;
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
            Player.Position - Globals.GraphicsDeviceManager.GraphicsDevice.PresentationParameters.Bounds.Size.ToVector2() / 2 + Player.Texture.Bounds.Size.ToVector2() / 2,
            0.2f
        );
        camera_pos = Globals.Active_Camera.Position;

        // Update map
        _tilemapCode.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        // Clear previous draw calls and fill the background
        _graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
        
        // Draw Isometric grid
        //_IsoGrid.Draw();
        
        Globals.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
        int scale = 4;
        int tile_size = 128;
        int real_size = tile_size * scale;
                
        foreach (KeyValuePair<Vector2, int> item in _mapLayer1)
            {
                // item.Key is the position (Vector2) of the tile
                // item.Value is the index or type of the tile
               
                Vector2 position = item.Key; // The position of the tile               
                Vector2 worldpos =  new(
                    (position.X - position.Y) * (tile_size / 2),
                    (position.X + position.Y) * (tile_size / 4)
                );
                int tileIndex = item.Value;  // The index of the tile in the tileset
                //Console.WriteLine(item.Key+ " and "+item.Value);
                // Draw the tile at the given position
                Globals.SpriteBatch.Draw(tileset1, worldpos-camera_pos, new Rectangle(item.Value*32, 0, 32, 32), Color.White, rotation: 0f, Vector2.Zero, Vector2.One*4, SpriteEffects.None, 0f);
            }
             foreach (KeyValuePair<Vector2, int> item in _mapLayer2)
            {
                // item.Key is the position (Vector2) of the tile
                // item.Value is the index or type of the tile
               
                Vector2 position = item.Key; // The position of the tile               
                Vector2 worldpos =  new(
                    (position.X - position.Y) * (tile_size / 2),
                    (position.X + position.Y) * (tile_size / 4)
                );
                int tileIndex = item.Value;  // The index of the tile in the tileset
                //Console.WriteLine(item.Key+ " and "+item.Value);
                // Draw the tile at the given position
                Globals.SpriteBatch.Draw(tileset1, worldpos-camera_pos, new Rectangle(item.Value*32, 0, 32, 32), Color.White, rotation: 0f, Vector2.Zero, Vector2.One*4, SpriteEffects.None, 0f);
            }
             foreach (KeyValuePair<Vector2, int> item in _mapLayer3)
            {
                // item.Key is the position (Vector2) of the tile
                // item.Value is the index or type of the tile
               
                Vector2 position = item.Key; // The position of the tile               
                Vector2 worldpos =  new(
                    (position.X - position.Y) * (tile_size / 2),
                    (position.X + position.Y) * (tile_size / 4)
                );
                int tileIndex = item.Value;  // The index of the tile in the tileset
                //Console.WriteLine(item.Key+ " and "+item.Value);
                // Draw the tile at the given position
                Globals.SpriteBatch.Draw(tileset1, worldpos-camera_pos, new Rectangle(item.Value*32, 0, 32, 32), Color.White, rotation: 0f, Vector2.Zero, Vector2.One*4, SpriteEffects.None, 0f);
            }
        Globals.SpriteBatch.End();

        // Draw test cube
        //Vector2 camera_pos = (Globals.Active_Camera is Camera camera) ? camera.Position : Vector2.Zero;
        //Globals.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
        //Globals.SpriteBatch.Draw(_testCube.Texture, position: -camera_pos, null, Color.White, rotation: 0f, Vector2.UnitX * 50, Vector2.One*5, default, 0f);
        //Globals.SpriteBatch.End();

        // Draw map
        _tilemapCode.Draw(gameTime);

        // Draw player
        Player.Draw();
        // Draw bullets
        foreach(Sprite bullet in _bullets) { bullet.Draw(); }

        base.Draw(gameTime);
    }
}
