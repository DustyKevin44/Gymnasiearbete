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
using Game.Custom.Experimental;
using MonoGame.Extended.Collisions;
using Game.Custom.Factories;
using System.Linq;

namespace Game.Custom.GameStates;


public class MainGameState : GameState
{
    private TiledMap _map;
    private TiledMapRenderer _mapRenderer;
    private HashSet<Point> _solidTiles = [];

    private SpriteBatch _spriteBatch;

    public MainGameState(Game game, GraphicsDevice graphicsDevice, ContentManager content)
        : base(game, graphicsDevice, content)
    {
        Initialize(graphicsDevice);
        LoadContent();
    }

    private void Initialize(GraphicsDevice _graphicsDevice)
    {
        var collisionSystem = new CollisionComponent(new RectangleF(int.MinValue / 2, int.MinValue / 2, int.MaxValue, int.MaxValue));
        collisionSystem.Initialize();

        _spriteBatch = new SpriteBatch(_graphicsDevice);

        // Initialize the world
        World world = new WorldBuilder()
            .AddSystem(new MovementSystem())
            .AddSystem(new EntityColliderSystem())
            .AddSystem(new BehaviorSystem())
            .AddSystem(new PlayerSystem())
            .AddSystem(new AliveSystem())
            .AddSystem(new ProceduralAnimationSystem())
            .AddSystem(new RenderSystem(_graphicsDevice, _spriteBatch))
            .Build();

            Global.Initialize(_game, world, new Random(), collisionSystem, _content, _graphicsDevice);

        Global.ContentLibrary.Sprites["player"] = _content.Load<Texture2D>("player2"); // Ensure you have a "player" texture
        EntityFactory.CreatePlayerAt(Vector2.Zero);

        var entityTexture = _content.Load<Texture2D>("slimeSheet"); // Ensure you have a "player" texture
        Texture2DAtlas atlas = Texture2DAtlas.Create("Atlas/slime", entityTexture, 32, 32);
        var spriteSheet = new SpriteSheet("SpriteSheet/slime", atlas);
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

        for (int i = 0; i < 1; i++)
        {
            EntityFactory.CreateSlimySlimeAt(new Vector2(
                Global.Random.Next(-100, 100),
                Global.Random.Next(-100, 100))
            );
        }

        var obstacle = Global.World.CreateEntity();
        obstacle.Attach(new Transform2(new(500, 500)));
        obstacle.Attach(new CollisionBox(new RectangleF(0f, 0f, 50f, 50f)));
        obstacle.Get<CollisionBox>().Parent = obstacle;

        // Load the Tiled map
        _map = _content.Load<TiledMap>("tileSetWith2Tileset"); // Use the name of your Tiled map file

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
        _solidTiles = [];
        var collisionLayer = _map.GetLayer<TiledMapTileLayer>("Tile Layer 1");
        if (collisionLayer != null)
        {
            for (ushort x = 0; x < collisionLayer.Width; x++)
            {
                for (ushort y = 0; y < collisionLayer.Height; y++)
                {
                    if (collisionLayer.TryGetTile(x, y, out var tile) && tile.Value.GlobalIdentifier != 0)
                    {
                        _solidTiles.Add(new Point(x, y)); // Store only tile coordinates
                    }
                }
            }
        }
    }

        public override void Update(GameTime gameTime)
        {
           
            if (InputManager.MouseClicked)
            {
                Console.WriteLine("ObjectPoolIsFullPolicy");
                 /*for (int i = 0; i < 5; i++)
            {
                Random Global.Random = new Random();
                _slime = Global.World.CreateEntity();
                _slime.Attach(new Transform2(new Vector2(Global.Random.Next(-100, 100), Global.Random.Next(-100, 100))));
                _slime.Attach(new VelocityComponent(new(0, 0)));
                _slime.Attach(new Behavior(0, target: _player));
                _slime.Attach(new AnimatedSprite(spriteSheet, "slimeAnimation"));
                _slime.Attach(new HealthComponent(100));
                _slime.Attach(new CollisionBox(new RectangleF(0, 0, 16, 16), _collisionComponent));
                List<Color> colors = [Color.Black, Color.White, Color.Aqua, Color.Green, Color.Yellow];
                _slime.Get<AnimatedSprite>().Color = colors[Global.Random.Next(0, 5)];
                _slime.Get<CollisionBox>().Initialize(_slime);

            }*/

            }
          
            // Update the camera's position with scaled movement direction
            var playerPos = Global.Players.First().Get<Transform2>().Position;
            Global.Camera.LookAt(playerPos); // <-- should be in Global.World.Update() probably

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

        // Debug drawing
        for (int i=0; i < Global.World.EntityCount; i++)
        {
            Entity e = Global.World.GetEntity(i);

            if (e.Has<Transform2>() && e.Has<CollisionBox>())
            {
                var transform = e.Get<Transform2>();
                var collisionBox = e.Get<CollisionBox>();

                _spriteBatch.DrawRectangle(collisionBox.Bounds.Position + transform.Position, collisionBox.Bounds.BoundingRectangle.Size, Color.Black, 2f);

                if (e.Has<Skeleton>())
                    e.Get<Skeleton>().Draw(gameTime, _spriteBatch);
            }
        }

        // Render all entities (handled by RenderSystem)
        Global.World.Draw(gameTime);

        _spriteBatch.End();
    }

    public override void PostUpdate(GameTime gameTime) { }
}
