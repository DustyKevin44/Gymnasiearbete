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
    private AnimatedSprite Player;
    private float _player_speed = 200f;
    // Slime 
    //private AnimatedSprite SlimeSprite;
    private Slime slime;

    private SpriteFont _font;


    // Bullet
    private readonly List<Bullet> _bullets = [];
    private Texture2D bullet_sprite;
    private readonly float _bullet_speed = 400f;
    private readonly Timer _bullet_cooldown = new(0.1f, false);

    // Isometric Tilemap
    private readonly TileMap tileMap = new(1, new Vector2(2, 2), new Point(32, 32), new Point(32, 32));
    private readonly TileMap tileMapFoilage = new(1, new Vector2(2, 2), new Point(32, 32), new Point(32, 32));
    private readonly TileMap tileMapFoilage_grass = new(1, new Vector2(2, 2), new Point(32, 32), new Point(32, 32));
    private readonly Point chunk_size = new(16, 16);

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
        Player = new AnimatedSprite(null, Vector2.Zero, 4, 200f, true);
        _bullet_cooldown.StartTimer();
        slime= new Slime(null, new Vector2(20,20));
       
        
        // test csv tileMap files
        //tileMap.LoadLayer("../../../test.csv", 0, chunk_size);
        //tileMap.LoadLayer("../../../test2.csv", 1, chunk_size);
    
        // Create the isometric grid
        //tileMap.LoadLayer("../../../playgroundtilemap_Tile Layer 1.csv", 0, chunk_size);
        //tileMap.LoadLayer("../../../playgroundtilemap_Tile Layer 2.csv", 1, chunk_size);
        //tileMap.LoadLayer("../../../playgroundtilemap_Tile Layer 3.csv", 2, chunk_size);
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
        tileMap.LoadLayer("../../../tileLayer1.csv", 0, chunk_size);
        tileMapFoilage.LoadLayer("../../../tileLayer2.csv", 0, chunk_size);
        tileMapFoilage_grass.LoadLayer("../../../tileLayer1.csv", 0, chunk_size);

        tileMap.LoadTileset("grassSet");
        tileMapFoilage.LoadTileset("Stone_Medium_1_And_2");
        tileMapFoilage_grass.LoadTileset("grass_foilage");

        // Load player Texture
        Player.Texture = Globals.ContentManager.Load<Texture2D>("playerTest");
        // Load slime texture
        slime.sprite.Texture = Globals.ContentManager.Load<Texture2D>("panther");
        // Create new bullet Texture (OrangeRed circle with the radius 5)
        bullet_sprite = Globals.CreateTexture(10, 10, Color.OrangeRed, Globals.CircleShader);

        _font = Content.Load<SpriteFont>("font"); // Match the name from the Content Pipeline

        

    }

    protected override void Update(GameTime gameTime)
    {{
    KeyboardState keyboard = Keyboard.GetState();
    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboard.IsKeyDown(Keys.Escape))
        Exit();

    // Player movement
    Vector2 direction = Vector2.Zero;
    if (keyboard.IsKeyDown(Keys.W)) direction.Y -= 1;
    if (keyboard.IsKeyDown(Keys.S)) direction.Y += 1;
    if (keyboard.IsKeyDown(Keys.A)) direction.X -= 1;
    if (keyboard.IsKeyDown(Keys.D)) direction.X += 1;
    if (direction != Vector2.Zero) direction.Normalize();

    // Update player position
    Vector2 step = direction * _player_speed * GameTimeToDelta(gameTime);
    Player.Position += step;
    Player.Update(gameTime);

    // Ensure the player stays within valid tiles
    Point playerTilePos = tileMap.WorldToTile(Player.Position);
    Point playerChunkPos = new(playerTilePos.X / chunk_size.X, playerTilePos.Y / chunk_size.Y);
    Point playerLocalTilePos = new(playerTilePos.X % chunk_size.X, playerTilePos.Y % chunk_size.Y);

    //if (tileMap.Layers[0].GetChunk(playerChunkPos) is not IChunk chunk ||
    //    chunk.GetTile(playerLocalTilePos) is not int tileType || tileType == -1)
    //{
        // Revert movement if tile is invalid
    //    Player.Position -= step;
    //}

    // Update bullets
    _bullet_cooldown.Update(gameTime);
    MouseState mouse = Mouse.GetState();
    if (mouse.LeftButton == ButtonState.Pressed && _bullet_cooldown.Finished)
    {
        Vector2 mouseWorldPos = mouse.Position.ToVector2() + Globals.Active_Camera.Position;
        Vector2 bulletDir = mouseWorldPos - (Player.Position + Player.Texture.Bounds.Size.ToVector2() / 2);

        if (bulletDir != Vector2.Zero) bulletDir.Normalize();

        Vector2 bulletVelocity = bulletDir * _bullet_speed + direction * _player_speed / 2f;
        _bullets.Add(new Bullet(bullet_sprite, Player.Position + Player.Texture.Bounds.Size.ToVector2() / 2, bulletVelocity));
        _bullet_cooldown.StartTimer();
    }

    // Update each bullet
    _bullets.RemoveAll(bullet =>
    {
        bullet.Update(gameTime);
        return bullet.Object_is_dying; // Remove bullets marked for deletion
    });

    // Update slime behavior
    slime.Update(gameTime);

    // Move camera to follow the player
    Globals.Active_Camera.Position = Vector2.SmoothStep(
        Globals.Active_Camera.Position,
        Player.Position + (Player.Texture.Bounds.Size.ToVector2() - Globals.GraphicsDeviceManager.GraphicsDevice.PresentationParameters.Bounds.Size.ToVector2()) / 2,
        0.2f
    );

    base.Update(gameTime);
}
    }
protected override void Draw(GameTime gameTime)
{
    // Clear the screen
    _graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

    // Determine player tile and chunk positions
    Point playerTilePos = tileMap.WorldToTile(Player.Position);
    Point playerChunkPos = new(playerTilePos.X / chunk_size.X, playerTilePos.Y / chunk_size.Y);
    Point renderDistance = new(2, 2);

    // Draw tile layers
    Globals.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
    tileMap.Layers[0].Draw(playerChunkPos - renderDistance, playerChunkPos + renderDistance, Globals.SpriteBatch, tileMap);
        tileMapFoilage_grass.Layers[0].Draw(playerChunkPos - renderDistance, playerChunkPos + renderDistance, Globals.SpriteBatch, tileMapFoilage_grass);
    tileMapFoilage.Layers[0].Draw(playerChunkPos - renderDistance, playerChunkPos + renderDistance, Globals.SpriteBatch, tileMapFoilage);


    //.Layers[1].Draw(playerChunkPos - renderDistance, playerChunkPos + renderDistance, Globals.SpriteBatch, tileMap);
    Globals.SpriteBatch.End();

    // Draw the player
    Globals.SpriteBatch.Begin();
    Player.Draw(gameTime);
    Globals.SpriteBatch.End();

    // Draw slime
    Globals.SpriteBatch.Begin();
    slime.sprite.rotation = 1.5f;
    Console.WriteLine($"Angle: {slime.angle}, Direction: {slime.direction}");

    slime.Draw();
    Globals.SpriteBatch.End();

    // Draw bullets
    Globals.SpriteBatch.Begin();
    foreach (Bullet bullet in _bullets)
    {
        bullet.Draw();
    }
    Globals.SpriteBatch.End();

    Globals.SpriteBatch.Begin();
    Globals.SpriteBatch.DrawString(_font, "Animationframe" + Player.Position , new Vector2(100, 100), Color.White);
    Globals.SpriteBatch.End();

    base.Draw(gameTime);
}}
