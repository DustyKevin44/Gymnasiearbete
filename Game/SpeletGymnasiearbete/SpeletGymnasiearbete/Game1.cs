using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using SpeletGymnasiearbete.Classes;
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

    // Isometric Tilemap
    private readonly TileMap tileMap = new(4, new Vector2(4, 4), new Point(32, 16), new Point(32, 32));
    private readonly Point chunk_size = new(2, 2);

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
        
        // test csv tileMap files
        //tileMap.LoadLayer("../../../test.csv", 0, new(2, 2)); // TODO: fix chunk loading
        //tileMap.LoadLayer("../../../test2.csv", 1, new(32, 32));

        // Create the isometric grid
        tileMap.LoadLayer("../../../playgroundtilemap_Tile Layer 1.csv", 0, chunk_size);
        tileMap.LoadLayer("../../../playgroundtilemap_Tile Layer 2.csv", 1, chunk_size);
        tileMap.LoadLayer("../../../playgroundtilemap_Tile Layer 3.csv", 2, chunk_size);
        //tileMap.LoadLayer("../../../playgroundtilemap_Collision.csv", 3, new(32, 32));

        // Create Camera
        Globals.Active_Camera = new(new Vector2());
        
        /*
        //Load tilemap
        //Dictionary<Vector2, int> tilemap = TilemapCode.LoadMap("./playgroundtilemap.tmx");
        _mapLayer1 = TilemapCode.LoadMap("../../../playgroundtilemap_Tile Layer 1.csv");
        _mapLayer2 = TilemapCode.LoadMap("../../../playgroundtilemap_Tile Layer 2.csv");
        _mapLayer3 = TilemapCode.LoadMap("../../../playgroundtilemap_Tile Layer 3.csv");
        _mapLayerCollision = TilemapCode.LoadMap("../../../playgroundtilemap_Collision.csv");
        */

        base.Initialize();
    }

    protected override void LoadContent()
    {
        // Add useful properties to Globals
        Globals.SetSpriteBatch(new SpriteBatch(GraphicsDevice));
        Globals.SetContentManager(Content);
        Globals.SetGraphicsDeviceManager(_graphics);

        // Load tileset
        tileMap.LoadTileset("tilesetImage");
        
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
        } else if (keyboard.IsKeyDown(Keys.A) ^ keyboard.IsKeyDown(Keys.D)) {
            if (keyboard.IsKeyDown(Keys.A)) direction.Rotate(1.107149611f * direction.Y);
            if (keyboard.IsKeyDown(Keys.D)) direction.Rotate(1.107149611f * -direction.Y);
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
        
        Point Player_Tile_pos = tileMap.WorldToIso(Player.Position);
        Point Player_chunk = new(Player_Tile_pos.X / chunk_size.X, Player_Tile_pos.Y / chunk_size.Y);
        Point Player_inner_pos = new(Player_Tile_pos.X % chunk_size.X, Player_Tile_pos.Y % chunk_size.Y);
        Point offset = new(1, 1);

        // Draw Isometric grid
        Globals.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
        tileMap.Layers[0].Draw(Player_chunk - offset, Player_chunk + offset, Globals.SpriteBatch, tileMap);
        tileMap.Layers[1].Draw(Player_chunk - offset, Player_chunk + offset, Globals.SpriteBatch, tileMap);
        tileMap.Layers[2].Draw(Player_chunk - offset, Player_chunk + offset, Globals.SpriteBatch, tileMap);
        Globals.SpriteBatch.End();

        // Draw player
        Player.Draw();
        // Draw bullets
        foreach(Sprite bullet in _bullets) { bullet.Draw(); }

        base.Draw(gameTime);
    }
}
