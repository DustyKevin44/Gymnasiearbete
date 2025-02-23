using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Game.Custom.Input;
using Game.Custom.Components;
using Game.Custom.Components.Systems;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.Extended.Tiled.Renderers;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Input;
using MonoGame.Extended.ECS;
using MonoGame.Extended;
using System.Linq;
using Game.Custom.Experimental;
using System.Threading.Tasks.Dataflow;
using MonoGame.Extended.Collections;

namespace Game.Custom.GameStates
{
    public class MainGameState : GameState
    {
        private OrthographicCamera _camera;
        private TiledMap _map;
        private TiledMapRenderer _mapRenderer;
        private Entity _player;
        private World _world;
        private HashSet<Point> _solidTiles = new HashSet<Point>(); 
        private readonly Chain _chain = new(Vector2.Zero, Vector2.Zero, [
            new Joint(new Vector2(0, 0), 20f, 0f),
            new Joint(new Vector2(0, 0), 20f, 0f, -MathHelper.PiOver2, MathHelper.PiOver2),
            new Joint(new Vector2(0, 0), 20f, 0f, -MathHelper.PiOver2, MathHelper.PiOver2),
        ]);

        private Texture2D playerTexture;
        private Texture2D entityTexture;
        private SpriteBatch _spriteBatch;

        //private EntityManager _entityManager;
        private List<Entity> enemyList;
        private Entity targetdeath;


        public MainGameState(Game game, GraphicsDevice graphicsDevice, ContentManager content)
            : base(game, graphicsDevice, content)
        {
            Initialize(graphicsDevice);
            LoadContent();
        }
        #region Intitialize
        private void Initialize(GraphicsDevice _graphicsDevice)
        {
            _spriteBatch = new SpriteBatch(_graphicsDevice);

            // Initialize the world
            _world = new WorldBuilder()
                .AddSystem(new MovementSystem())
                .AddSystem(new RenderSystem(_graphicsDevice, _spriteBatch))
                .AddSystem(new BehaviorSystem())
                .AddSystem(new PlayerSystem())
                .AddSystem(new AliveSystem())
                .AddSystem(new TileCollisionSystem(_solidTiles))
                .Build();

            playerTexture = _content.Load<Texture2D>("player2"); // Ensure you have a "player" texture

            #region Player       
            _player = _world.CreateEntity();
            _player.Attach(new Transform2(Vector2.Zero));
            _player.Attach(new VelocityComponent(Vector2.Zero));
            _player.Attach(new SpriteComponent(playerTexture));
            _player.Attach(new PlayerComponent<StdActions>(
                "Player", new Dictionary<StdActions, Keybind> {
                    { StdActions.MOVE_UP,    new Keybind(key: Keys.W) },
                    { StdActions.MOVE_DOWN,  new Keybind(key: Keys.S) },
                    { StdActions.MOVE_LEFT,  new Keybind(key: Keys.A) },
                    { StdActions.MOVE_RIGHT, new Keybind(key: Keys.D) },
                    { StdActions.DASH,       new Keybind(key: Keys.Space) },
                    { StdActions.CUSTOM,     new CustomKeybind(RotateCamera, Keys.R)},
                })
            );

            void RotateCamera(GameTime gameTime)
            {
                targetdeath.Get<HealthComponent>().Health = 0; Console.WriteLine("Try to kill");
            }
            //_camera.Rotate((float)Math.PI * gameTime.GetElapsedSeconds());


            #endregion


            #region Entity
            entityTexture = _content.Load<Texture2D>("slimeSheet"); // Ensure you have a "player" texture
            Texture2DAtlas atlas = Texture2DAtlas.Create("Atlas/slime", entityTexture, 32, 32);
            SpriteSheet spriteSheet = new("SpriteSheet/slime", atlas);
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
            Entity entityTarget = null;
            for (int i = 0; i < 1; i++)
            {
                Random rnd = new Random();
                Entity entity = _world.CreateEntity();
                entity.Attach(new Transform2(new Vector2(rnd.Next(-100, 100), rnd.Next(-100, 100))));
                entity.Attach(new VelocityComponent(new(100, 0)));
                entity.Attach(new Behavior(0, target: _player));
                entity.Attach(new AnimatedSprite(spriteSheet, "slimeAnimation"));
                entity.Attach(new HealthComponent(100));
                entity.Attach(new ColliderComponent(entityTexture.Width,entityTexture.Height));
                List<Color> colors = [Color.Black, Color.White, Color.Aqua, Color.Green, Color.Yellow];
                entity.Get<AnimatedSprite>().Color = colors[rnd.Next(0, 5)];

            }


            #endregion


            // Load the Tiled map
            _map = _content.Load<TiledMap>("tileSetWith2Tileset"); // Use the name of your Tiled map file

            // Initialize the TiledMapRenderer
            _mapRenderer = new TiledMapRenderer(_graphicsDevice, _map);

            var viewportAdapter = new ScalingViewportAdapter(_graphicsDevice, 200, 150);
            _camera = new OrthographicCamera(viewportAdapter);
        }
        #endregion

        public override void LoadContent()
        {
            _solidTiles = new HashSet<Point>();

            var collisionLayer = _map.GetLayer<TiledMapTileLayer>("Tile Layer 1");
            Console.WriteLine(collisionLayer.Width);
            if (collisionLayer != null)
            {
                for (ushort x = 0; x < collisionLayer.Width; x++)
                {
                    for (ushort y = 0; y < collisionLayer.Height; y++)
                    {
                      
                        if (collisionLayer.TryGetTile(x, y, out var tile) && tile.Value.GlobalIdentifier != 0)
                        {

                            _solidTiles.Add(new Point(x, y)); // Store only tile coordinates
                            Console.WriteLine($"Tile at ({x}, {y}): ID = {tile.Value.GlobalIdentifier} was added");

                        }
                        Console.WriteLine($"Tile at ({x}, {y}): ID = {tile.Value.GlobalIdentifier}");

                    }
                }
            }
        }

        #region Update
        public override void Update(GameTime gameTime)
        {
            if (InputManager.MouseClicked)
            {
                Console.WriteLine("ObjectPoolIsFullPolicy");
            }
            foreach (var layer in _map.Layers)
            {
                Console.WriteLine($"Layer: {layer.Name}");
            }

            // Update the camera's position with scaled movement direction
            var playerPos = _player.Get<Transform2>().Position;
            _camera.LookAt(playerPos); // <-- should be in _world.Update() probably

            _world.Update(gameTime);

            /*foreach(Entity i in enemyList)
            {
                _chain.Anchor = targetdeath.Get<Transform2>().Position + new Vector2(20, 20);
                _chain.Target = playerPos;
                _chain.Update(gameTime);
            }*/
            InputManager.Update();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Get the camera view
            var transformMatrix = _camera.GetViewMatrix();

            _spriteBatch.Begin(transformMatrix: transformMatrix, sortMode: default, blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);

            // Render the tilemap
            _mapRenderer.Draw(transformMatrix);

            Vector2 position = _player.Get<Transform2>().Position - new Vector2(
                _player.Get<SpriteComponent>().Texture.Width / 2f,
                _player.Get<SpriteComponent>().Texture.Height / 2f
            );

            RectangleF hitbox = new(position, _player.Get<SpriteComponent>().Texture.Bounds.Size);

            _spriteBatch.DrawRectangle(hitbox, Color.Black, 2f);

            // Render all entities (handled by RenderSystem)
            _world.Draw(gameTime);

            _chain.Draw(gameTime, _spriteBatch);

            _spriteBatch.End();
        }
        #endregion

        public override void PostUpdate(GameTime gameTime) { }
    }
}
