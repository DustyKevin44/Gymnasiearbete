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
using Game.Custom.Saving;
using Game.Custom.Utilities;
using Game.Custom.Experimental;

namespace Game.Custom.GameStates;


public class MainGameState : GameState
{
    private TiledMap _map;
    private TiledMapRenderer _mapRenderer;
    private HashSet<Point> _solidTiles = [];

    private SpriteBatch _spriteBatch;
    private Entity obstacle;

    public MainGameState(Game game, GraphicsDevice graphicsDevice, ContentManager content, int? gameId = null)
        : base(game, graphicsDevice, content)
    {
        Initialize(graphicsDevice, gameId);
        LoadContent();
    }

    private void Initialize(GraphicsDevice _graphicsDevice, int? gameId)
    {
        Global.Unload();

        var collisionSystem = new CollisionComponent(new RectangleF(int.MinValue / 2, int.MinValue / 2, int.MaxValue, int.MaxValue));
        collisionSystem.Initialize();

        _spriteBatch = new SpriteBatch(_graphicsDevice);
        Global.Initialize(_game, new Random(), collisionSystem, _content, _graphicsDevice, _spriteBatch);

        // Initialize the world
        World world = new WorldBuilder()
            .AddSystem(new MovementSystem())
            .AddSystem(new EntityColliderSystem())
            .AddSystem(new BehaviorSystem())
            .AddSystem(new PlayerSystem())
            .AddSystem(new AliveSystem())
            .AddSystem(new TweenerSystem())
            .AddSystem(new SpawnerSystem())
            .AddSystem(new SegmentSystem())
            .AddSystem(new DebugSystem())
            .AddSystem(new RenderSystem())
            .AddSystem(new DebugRenderSystem())
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

        #region Player Animations
        
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
        Global.ContentLibrary.Animations.Add("player", spriteSheet);

        Global.ContentLibrary.Textures["player"] = _content.Load<Texture2D>("player2"); // Ensure you have a "player" texture

        #endregion

        var spawner = Global.World.CreateEntity();
        spawner.Attach(new SpawnerComponent(new(200,200), new(100,100), "Slime", 2.0f));
        

        if (gameId.HasValue)
        {
            Global.SaveManager.LoadGame(gameId.Value);
            Global.SaveManager.PrintAllSavedData();
            Global.GameId = gameId.Value;

            EntityFactory.CreateCentipedeAt(new Vector2(300, 300));
        }
        else
        {

            for (int i = 0; i < 1; i++)
            {
                EntityFactory.CreateSlimeAt(new Vector2(
                    Global.Random.Next(-10, 10),
                    Global.Random.Next(-10, 10)), 100f
                );
            }

            var player = EntityFactory.CreatePlayerAt(Vector2.Zero, 100f);
            var eq = player.Get<Equipment>();
            eq.Equip("hand", EntityFactory.CreateSwordAt(Vector2.Zero));
            var collisionbox = new CollisionBox(new RectangleF(10f, 10f, 50f, 50f));
            var hurtbox = new HurtBox(new RectangleF(0, 0, 50, 50));
            
            obstacle = Global.World.CreateEntity();
            obstacle.Attach(new Transform2(new(100, 100)));
            obstacle.Attach(collisionbox);
            obstacle.Attach(hurtbox);
            obstacle.Attach(new HealthComponent(100, 100f));
            collisionbox.Parent = obstacle;
            hurtbox.Parent = obstacle;
        }






        // var slimeSpawner = Global.World.CreateEntity();
        // slimeSpawner.Attach(new SpawnerComponent(new(0,0), new(500,500), new("slime"), 1f));

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
            {
                Global.CollisionSystem.Insert(entity.Get<CollisionBox>());
            }
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
