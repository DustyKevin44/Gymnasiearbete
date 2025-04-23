using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Game.Custom.Input;
using Game.Custom.Components;
using Game.Custom.Components.Systems;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.Extended.Tiled.Renderers;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.ECS;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using Game.Custom.Factories;
using Game.Custom.Debug;
using Game.Custom.Experimental;

namespace Game.Custom.GameStates;


public class MainGameState : GameState
{
    private TiledMap _map;
    private TiledMapRenderer _mapRenderer;
    private HashSet<Point> _solidTiles = [];
    private SpriteBatch _spriteBatch;

    public MainGameState(Game game, GraphicsDevice graphicsDevice, ContentManager content, int? gameId = null)
        : base(game, graphicsDevice, content)
    {
        Initialize(graphicsDevice, gameId);
        LoadContent();
    }

    private void Initialize(GraphicsDevice _graphicsDevice, int? gameId)
    {
        Global.Unload(); // Remove all Global values

        var collisionSystem = new CollisionComponent(new RectangleF(int.MinValue / 2, int.MinValue / 2, int.MaxValue, int.MaxValue));
        collisionSystem.Initialize();

        _spriteBatch = new SpriteBatch(_graphicsDevice);
        Global.Initialize(_game, new Random(), collisionSystem, _content, _graphicsDevice, _spriteBatch);

        // Initialize the world
        World world = new WorldBuilder()
            .AddSystem(new SpawnerSystem())         // Spawn enemies first
            .AddSystem(new BehaviorSystem())        // Handle behaviours
            .AddSystem(new PlayerSystem())          //      - player behaviour
            .AddSystem(new MovementSystem())        // Handle movement
            .AddSystem(new TweenerSystem())         //      - tween (position, ...)
            .AddSystem(new SegmentSystem())         //      - move segments
            .AddSystem(new EntityColliderSystem())  // Handle Collisions
            .AddSystem(new AliveSystem())           // Remove dead entities
            .AddSystem(new DebugSystem())           // Debug (select entities)
            .AddSystem(new RenderSystem())          // Render game
            .AddSystem(new DebugRenderSystem())     //      - Debug render (colliders)
            .Build();

        Global.SetWorld(world); // <-- some systems need Globals and thus this is here

        var swordsTexture = _content.Load<Texture2D>("swords");
        Global.ContentLibrary.SaveTexture(swordsTexture, "swords");

        var entityTexture = _content.Load<Texture2D>("slimeSheet");
        Texture2DAtlas slimeAtlas = Texture2DAtlas.Create("Atlas/slime", entityTexture, 32, 32);
        var spriteSheet = new SpriteSheet("SpriteSheet/slime", slimeAtlas);
        spriteSheet.DefineAnimation("slimeAnimation", builder =>
        {
            builder.IsLooping(true)
                .AddFrame(0, TimeSpan.FromSeconds(0.4))
                .AddFrame(1, TimeSpan.FromSeconds(0.4))
                .AddFrame(2, TimeSpan.FromSeconds(0.4))
                .AddFrame(3, TimeSpan.FromSeconds(0.4))
                .AddFrame(4, TimeSpan.FromSeconds(0.4));
        });

        spriteSheet.DefineAnimation("idleAnimation", builder =>
        {
            builder.IsLooping(true)
                .AddFrame(3, TimeSpan.FromSeconds(0.2))
                .AddFrame(4, TimeSpan.FromSeconds(0.2));
        });

        Global.ContentLibrary.Animations.Add("slime", spriteSheet);

        var playerTexture = _content.Load<Texture2D>("playerRunRight");
        Texture2DAtlas playerAtlas = Texture2DAtlas.Create("Atlas/player", playerTexture, 32, 32);
        var playerSpriteSheet = new SpriteSheet("SpriteSheet/player", playerAtlas);
        playerSpriteSheet.DefineAnimation("runRight", builder =>
        {
            builder.IsLooping(true)
                .AddFrame(0, TimeSpan.FromSeconds(0.4))
                .AddFrame(1, TimeSpan.FromSeconds(0.4))
                .AddFrame(2, TimeSpan.FromSeconds(0.4))
                .AddFrame(3, TimeSpan.FromSeconds(0.4))
                .AddFrame(4, TimeSpan.FromSeconds(0.4))
                .AddFrame(5, TimeSpan.FromSeconds(0.4))
                .AddFrame(6, TimeSpan.FromSeconds(0.4))
                .AddFrame(7, TimeSpan.FromSeconds(0.4))
                .AddFrame(8, TimeSpan.FromSeconds(0.4));
        });

        playerSpriteSheet.DefineAnimation("idleAnimation", builder =>
        {
            builder.IsLooping(true)
                .AddFrame(3, TimeSpan.FromSeconds(0.2))
                .AddFrame(4, TimeSpan.FromSeconds(0.2));
        });
        Global.ContentLibrary.Animations.Add("player", playerSpriteSheet);
        Global.ContentLibrary.Textures["player"] = _content.Load<Texture2D>("player2"); // Ensure you have a "player" texture

        if (!gameId.HasValue)
        {
            throw new ArgumentException("Missing GameId");
        }

        // Load saved game
        Global.SaveManager.LoadGame(gameId.Value);
        Global.SaveManager.PrintAllSavedData();
        Global.GameId = gameId.Value;

        // Testing
        var zombie = EntityFactory.CreateZombieAt(new(200, 200), 100);
        var centipede = EntityFactory.CreateCentipedeAt(new(300, 300));
        Global.World.CreateEntity().Attach(new SpawnerComponent(new(200, 200), new(100, 100), "Slime", 2.0f));

        // Load the Tiled map
        _map = _content.Load<TiledMap>("tileMapGyar"); // Use the name of your Tiled map file

        // Initialize the TiledMapRenderer
        _mapRenderer = new TiledMapRenderer(_graphicsDevice, _map);

        var viewportAdapter = new ScalingViewportAdapter(_graphicsDevice, 200, 150);
        Global.Camera = new OrthographicCamera(viewportAdapter);

        for (int i = 0; i < Global.World.EntityCount; i++)
        {
            var entity = Global.World.GetEntity(i);
            if (entity.Has<CollisionBox>())
                Global.CollisionSystem.Insert(entity.Get<CollisionBox>());
        }
    }

    public override void LoadContent()
    {

    }

    public override void Update(GameTime gameTime)
    {
        Global.World.Update(gameTime);
        InputManager.Update();
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        // Get the camera view
        var transformMatrix = Global.Camera.GetViewMatrix();

        _spriteBatch.Begin(transformMatrix: transformMatrix, sortMode: default, blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);

        // Render the tilemap
        _mapRenderer.Draw(transformMatrix);

        // Render all entities (handled by RenderSystem)
        Global.World.Draw(gameTime);

        _spriteBatch.End();
    }

    public override void PostUpdate(GameTime gameTime) { }
}
