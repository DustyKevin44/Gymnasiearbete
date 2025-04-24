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
using Game.Custom.Experimental;
using System.Linq;
using Game.Custom.Utilities;
using MonoGame.Extended.Collisions.Layers;
using MonoGame.Extended.Collisions.QuadTree;
using Microsoft.Xna.Framework.Audio;

namespace Game.Custom.GameStates;


public class MainGameState : GameState
{
    private TiledMap _map;
    private TiledMapRenderer _mapRenderer;
    private HashSet<Point> _solidTiles = [];
    private SpriteBatch _spriteBatch;
    private Entity spawner;

    public MainGameState(Game game, GraphicsDevice graphicsDevice, ContentManager content, int? gameId = null)
        : base(game, graphicsDevice, content)
    {
        Initialize(graphicsDevice, gameId);
        LoadContent();
    }

    private void Initialize(GraphicsDevice _graphicsDevice, int? gameId)
    {
        Global.Unload(); // Remove all Global values

        var collisionSystem = new CollisionComponent(new RectangleF(0, 0, 32 * 50, 32 * 50));
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
            .AddSystem(new RenderSystem())          // Render game
            .Build();

        Global.SetWorld(world); // <-- some systems need Globals and thus this is here

        // Load Textures (Loading need to be done before initialization of entities)

        // Sword
        var swordsTexture = _content.Load<Texture2D>("swords");
        Global.ContentLibrary.SaveTexture(swordsTexture, "swords");

        // Slime
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

        var playerTexture = _content.Load<Texture2D>("playerTexture");
        Texture2DAtlas playerAtlas = Texture2DAtlas.Create("Atlas/player", playerTexture, 32, 64);
        var playerSpriteSheet = new SpriteSheet("SpriteSheet/player", playerAtlas);
        playerSpriteSheet.DefineAnimation("runLeft", builder =>
        {
            builder.IsLooping(true)
                .AddFrame(9, TimeSpan.FromSeconds(0.1))
                .AddFrame(8, TimeSpan.FromSeconds(0.1))
                .AddFrame(7, TimeSpan.FromSeconds(0.1))
                .AddFrame(6, TimeSpan.FromSeconds(0.1))
                .AddFrame(5, TimeSpan.FromSeconds(0.1))
                .AddFrame(4, TimeSpan.FromSeconds(0.1))
                .AddFrame(3, TimeSpan.FromSeconds(0.1))
                .AddFrame(2, TimeSpan.FromSeconds(0.1))
                .AddFrame(1, TimeSpan.FromSeconds(0.1));
        });
        playerSpriteSheet.DefineAnimation("runRight", builder =>
        {
            builder.IsLooping(true)
                .AddFrame(10, TimeSpan.FromSeconds(0.1))
                .AddFrame(11, TimeSpan.FromSeconds(0.1))
                .AddFrame(12, TimeSpan.FromSeconds(0.1))
                .AddFrame(13, TimeSpan.FromSeconds(0.1))
                .AddFrame(14, TimeSpan.FromSeconds(0.1))
                .AddFrame(15, TimeSpan.FromSeconds(0.1))
                .AddFrame(16, TimeSpan.FromSeconds(0.1))
                .AddFrame(17, TimeSpan.FromSeconds(0.1));
        });

        playerSpriteSheet.DefineAnimation("idle", builder =>
        {
            builder.IsLooping(true)
                .AddFrame(0, TimeSpan.FromSeconds(1));
        });

        Global.ContentLibrary.Animations.Add("player", playerSpriteSheet);

        // Old player
        Global.ContentLibrary.Textures["player"] = _content.Load<Texture2D>("player2");

        // Load Sound Effects
        Global.ContentLibrary.SaveSoundEffects(_content.Load<SoundEffect>("Hurt"), "Hurt");

        if (!gameId.HasValue)
        {
            throw new ArgumentException("Missing GameId.");
        }

        // Load saved game
        Global.SaveManager.LoadGame(gameId.Value);
        Global.SaveManager.PrintAllSavedData();
        Global.GameId = gameId.Value;

        // // Give player sword (TODO: change to store in database)
        // TODO: Fix sword attack moving the player to {Nan, Nan}
        // if (Global.Players.Count != 0)
        // {
        //     var sword = EntityFactory.CreateSwordAt(new(16, 32), null);
        //     Global.Players.First().Get<Equipment>().Equip("hand", sword);
        // }

        // Testing
        // var centipede = EntityFactory.CreateCentipedeAt(new(300, 300));
        spawner = Global.World.CreateEntity();
        spawner.Attach(new SpawnerComponent(new(22 * 32, 6 * 32), new(7 * 32, 6 * 32), "Slime", 7.0f, 2f));

        // Load the Tiled map
        _map = _content.Load<TiledMap>("tileMapGyar"); // Use the name of your Tiled map file

        // Initialize the TiledMapRenderer
        _mapRenderer = new TiledMapRenderer(_graphicsDevice, _map);

        var viewportAdapter = new ScalingViewportAdapter(_graphicsDevice, 200, 150);
        Global.Camera = new OrthographicCamera(viewportAdapter);

        var enemyHitLayer = new Layer(new QuadTreeSpace(new(0, 0, 32 * 50, 32 * 30)));
        var enemyCollisionLayer = new Layer(new QuadTreeSpace(new(0, 0, 32 * 50, 32 * 30)));
        var enemyHurtLayer = new Layer(new QuadTreeSpace(new(0, 0, 32 * 50, 32 * 30)));
        var playerHitLayer = new Layer(new QuadTreeSpace(new(0, 0, 32 * 50, 32 * 30)));
        var playerCollisionLayer = new Layer(new QuadTreeSpace(new(0, 0, 32 * 50, 32 * 30)));
        var playerHurtLayer = new Layer(new QuadTreeSpace(new(0, 0, 32 * 50, 32 * 30)));

        for (int i = 0; i < Global.World.EntityCount; i++)
        {
            var entity = Global.World.GetEntity(i);

            if (Utils.TryGet(entity, out HitBox hitBox))
            {
                if (Utils.TryGet(entity, out Equipable eq) && Global.Players.Contains(eq.Parent))
                {
                    playerHitLayer.Space.Insert(hitBox);
                }
                else if (entity.Has<Behavior>())
                {
                    enemyHitLayer.Space.Insert(hitBox);
                }
            }
            if (Utils.TryGet(entity, out CollisionBox collisionBox))
            {
                if (entity.Has<PlayerComponent<StdActions>>())
                {
                    playerCollisionLayer.Space.Insert(collisionBox);
                }
                else if (entity.Has<Behavior>())
                {
                    enemyCollisionLayer.Space.Insert(collisionBox);
                }
            }
            if (Utils.TryGet(entity, out HitBox hurtBox))
            {
                if (entity.Has<PlayerComponent<StdActions>>())
                {
                    playerHurtLayer.Space.Insert(hurtBox);
                }
                else if (entity.Has<Behavior>())
                {
                    enemyHurtLayer.Space.Insert(hurtBox);
                }
            }
        }

        Global.CollisionSystem.Add("PlayerHit", playerHitLayer);
        Global.CollisionSystem.Add("PlayerCollision", playerCollisionLayer);
        Global.CollisionSystem.Add("PlayerHurt", playerHurtLayer);
        Global.CollisionSystem.Add("EnemyHit", enemyHitLayer);
        Global.CollisionSystem.Add("EnemyCollision", enemyCollisionLayer);
        Global.CollisionSystem.Add("EnemyHurt", enemyHurtLayer);

        Global.CollisionSystem.AddCollisionBetweenLayer(playerHitLayer, enemyHurtLayer);
        Global.CollisionSystem.AddCollisionBetweenLayer(playerHurtLayer, enemyHitLayer);
    }

    public override void LoadContent() { }

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

        // // Draw spawner bounds
        // if (Utils.TryGet(spawner, out SpawnerComponent C))
        // {
        //     _spriteBatch.DrawRectangle(C.Position, C.Size.ToSize(), Color.Red, 2f);
        // }

        // Render all entities (handled by RenderSystem)
        Global.World.Draw(gameTime);

        _spriteBatch.End();
    }

    public override void PostUpdate(GameTime gameTime) { }
}
