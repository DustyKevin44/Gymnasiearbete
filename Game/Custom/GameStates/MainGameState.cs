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
using MonoGame.Extended.Collisions;

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
        private CollisionComponent _collisionComponent = new CollisionComponent(new RectangleF(int.MinValue, int.MinValue, int.MaxValue, int.MaxValue));
        //private EntityManager _entityManager;
        private List<Entity> enemyList;
        private Entity targetdeath;

        private Entity obstacle;
        private Entity _slime;

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
            #region World
            // Initialize the world
            _world = new WorldBuilder()
                .AddSystem(new MovementSystem(_collisionComponent))
                .AddSystem(new RenderSystem(_graphicsDevice, _spriteBatch))
                .AddSystem(new BehaviorSystem())
                .AddSystem(new PlayerSystem())
                .AddSystem(new AliveSystem())
                //.AddSystem(new ColliderSystem())
                .Build();

            _collisionComponent.Initialize();
            #endregion
            /*
                Världen är uppdelad in many systems using the monogame extendeds ecs system. 
                Och detta makes it very easy to 
                hold is separerat and not have it be clustered even with many processes going on.


            */
            #region Player      
            playerTexture = _content.Load<Texture2D>("player2"); // Ensure you have a "player" texture

            _player = _world.CreateEntity();
            _player.Attach(new Transform2(Vector2.Zero));
            _player.Attach(new VelocityComponent(Vector2.Zero));
            _player.Attach(new SpriteComponent(playerTexture));
            _player.Attach(new CollisionBox(new RectangleF(0, 0, 20, 20), _collisionComponent));
            _player.Attach(new PlayerComponent<StdActions>(
                "Player", new Dictionary<StdActions, Keybind> {
                    { StdActions.MOVE_UP,    new Keybind(key: Keys.W) },
                    { StdActions.MOVE_DOWN,  new Keybind(key: Keys.S) },
                    { StdActions.MOVE_LEFT,  new Keybind(key: Keys.A) },
                    { StdActions.MOVE_RIGHT, new Keybind(key: Keys.D) },
                    { StdActions.DASH,       new Keybind(key: Keys.Space) },
                    { StdActions.CUSTOM,     new CustomKeybind(ZoomIn, mouseButton: MouseButton.Right)},
                    { StdActions.CUSTOM2,    new CustomKeybind(ZoomOut, mouseButton: MouseButton.Left)}
                })
            );

            void ZoomOut(GameTime gameTime)
            {
                _camera.ZoomOut(0.5f * gameTime.GetElapsedSeconds());
            }

            void ZoomIn(GameTime gameTime)
            {
                _camera.ZoomIn(0.5f * gameTime.GetElapsedSeconds());
            }


            #endregion
            /*   
                The player is an ECS entity with a specific Player component that makes it so you can control it.




            */
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
            for (int i = 0; i < 5; i++)
            {
                Random rnd = new Random();
                _slime = _world.CreateEntity();
                _slime.Attach(new Transform2(new Vector2(rnd.Next(-100, 100), rnd.Next(-100, 100))));
                _slime.Attach(new VelocityComponent(new(0, 0)));
                _slime.Attach(new Behavior(0, target: _player));
                _slime.Attach(new AnimatedSprite(spriteSheet, "slimeAnimation"));
                _slime.Attach(new HealthComponent(100));
                _slime.Attach(new CollisionBox(new RectangleF(0, 0, 16, 16), _collisionComponent));
                List<Color> colors = [Color.Black, Color.White, Color.Aqua, Color.Green, Color.Yellow];
                _slime.Get<AnimatedSprite>().Color = colors[rnd.Next(0, 5)];

            }


            #endregion


            obstacle = _world.CreateEntity();
            obstacle.Attach(new Transform2(new(200, 200)));
            obstacle.Attach(new CollisionBox(new RectangleF(0f, 0f, 50f, 50f), _collisionComponent));


            // Load the Tiled map
            _map = _content.Load<TiledMap>("tileSetWith2Tileset"); // Use the name of your Tiled map file

            // Initialize the TiledMapRenderer
            _mapRenderer = new TiledMapRenderer(_graphicsDevice, _map);

            var viewportAdapter = new ScalingViewportAdapter(_graphicsDevice, 200, 150);
            _camera = new OrthographicCamera(viewportAdapter);

            for (int i = 0; i < _world.EntityCount; i++)
            {
                Console.WriteLine("try");

                var entity = _world.GetEntity(i);
                if (entity.Has<CollisionBox>())
                {
                    _collisionComponent.Insert(entity.Get<CollisionBox>());
                }
                Console.WriteLine("entity" + entity);
            }
        }
        #endregion
        #region LoadContent
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
                            //Console.WriteLine($"Tile at ({x}, {y}): ID = {tile.Value.GlobalIdentifier} was added");

                        }
                        //Console.WriteLine($"Tile at ({x}, {y}): ID = {tile.Value.GlobalIdentifier}");

                    }
                }
            }
        }
        #endregion

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

            _chain.Anchor = _slime.Get<Transform2>().Position + new Vector2(20, 20);
            _chain.Target = playerPos;
            _chain.Update(gameTime);

            InputManager.Update();
        }
        #endregion
        #region Draw
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Get the camera view
            var transformMatrix = _camera.GetViewMatrix();

            _spriteBatch.Begin(transformMatrix: transformMatrix, sortMode: default, blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);

            // Render the tilemap
            _mapRenderer.Draw(transformMatrix);

            var Pcollider = _player.Get<CollisionBox>();
            var Ppos = _player.Get<Transform2>().Position;

            _spriteBatch.DrawRectangle(Pcollider.Shape.Position, Pcollider.Shape.BoundingRectangle.Size, Color.Black, 2f);

            var collider = obstacle.Get<CollisionBox>();
            var pos = obstacle.Get<Transform2>().Position;
            _spriteBatch.DrawRectangle(collider.Shape.Position, collider.Shape.BoundingRectangle.Size, Color.Black, 2f);

            var Scollider = _slime.Get<CollisionBox>();
            var Spos = _slime.Get<Transform2>().Position;
            _spriteBatch.DrawRectangle(Scollider.Shape.Position, Scollider.Shape.BoundingRectangle.Size, Color.Black, 2f);

            // Render all entities (handled by RenderSystem)
            _world.Draw(gameTime);
            #endregion

            _chain.Draw(gameTime, _spriteBatch);

            _spriteBatch.End();
        }

        public override void PostUpdate(GameTime gameTime) { }
    }

}
