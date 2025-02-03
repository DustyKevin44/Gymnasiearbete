using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using MonoGame.Extended.ViewportAdapters;
using Game.Custom.Input;
using Game.Custom.ObjectComponents;
using System;
using Game.Custom.Graphics;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Graphics;
using System.IO;

namespace Game.Custom.States
{
    public class GameState : State
    {
        private OrthographicCamera _camera;
        private TiledMap _map;
        private TiledMapRenderer _mapRenderer;
        private Entity _player;
        private World _world;

        private AnimatedSprite slime;
        private Entity entity;
        private Vector2 _position;
        private Vector2 playerVelocity;
        private EntityManager _entityManager;
        private Texture2D playerTexture;
        private Texture2D entityTexture;

        private SpriteBatch _spriteBatch;

        // Utility function to get movement direction
        private static Vector2 GetMovementDirection()
        {
            var state = Keyboard.GetState();
            return new Vector2(
                Utils.GetInputDirection(state.IsKeyDown(Keys.Left), state.IsKeyDown(Keys.Right)),
                Utils.GetInputDirection(state.IsKeyDown(Keys.Up), state.IsKeyDown(Keys.Down))
            ) * 10;
        }

        public GameState(Game game, GraphicsDevice graphicsDevice, ContentManager content)
            : base(game, graphicsDevice, content)
        {
            Initialize(graphicsDevice);
            LoadContent();
        }


        private void Initialize(GraphicsDevice _graphicsDevice)
        {
            _spriteBatch = new SpriteBatch(_graphicsDevice);
            // Initialize the world
            _world = new WorldBuilder()
                .AddSystem(new MovementSystem())
                .AddSystem(new RenderSystem(_graphicsDevice, _spriteBatch))
                .AddSystem(new BehaviorSystem())
                .Build();

            playerTexture = _content.Load<Texture2D>("player2"); // Ensure you have a "player" texture
            entityTexture = _content.Load<Texture2D>("slimeSheet"); // Ensure you have a "player" texture

            _player = _world.CreateEntity();

            _player.Attach(new Transform2(_position));
            _player.Attach(new VelocityComponent(playerVelocity)); // Moving right
            _player.Attach(new SpriteComponent(playerTexture));
            
            
            Texture2DAtlas atlas = Texture2DAtlas.Create("Atlas/slime", entityTexture, 16, 16);

            SpriteSheet spriteSheet = new SpriteSheet("SpriteSheet/slime", atlas);
            spriteSheet.DefineAnimation("slimeAnimation", builder =>
            {
                builder.IsLooping(true)
                    .AddFrame(0, TimeSpan.FromSeconds(0.1))
                    .AddFrame(1, TimeSpan.FromSeconds(0.1))
                    .AddFrame(2, TimeSpan.FromSeconds(0.1))
                    .AddFrame(3, TimeSpan.FromSeconds(0.1))
                    .AddFrame(4, TimeSpan.FromSeconds(0.1));
            });
            slime = new AnimatedSprite(spriteSheet, "slimeAnimation");
            entity = _world.CreateEntity();
            entity.Attach(new Transform2(new(100,100)));
            entity.Attach(new VelocityComponent(new())); // Moving right
            entity.Attach(new SpriteComponent(entityTexture,new(new(0,0), new(16,16))));
            entity.Attach(new Behavior(0));




            // Load the Tiled map
            _map = _content.Load<TiledMap>("tileSetWith2Tileset"); // Use the name of your Tiled map file


            // Initialize the TiledMapRenderer
            _mapRenderer = new TiledMapRenderer(_graphicsDevice, _map);

            var viewportAdapter = new ScalingViewportAdapter(_graphicsDevice, 200, 150);
            _camera = new OrthographicCamera(viewportAdapter);
        }

        public void LoadContent()
        {
            // No content loading needed here for now
            //playerTexture = _content.Load<Texture2D>("cursorButton"); // Ensure you have a "player" texture

        }

        public override void Update(GameTime gameTime)
        {
            // Get elapsed time for smooth movement
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Get the movement direction from input
            var movementDirection = GetMovementDirection();

                        
            if (InputManager.MouseClicked)
            {
                _camera.ZoomOut(0.2f);
            }           

            Transform2 playerTransform = _player.Get<Transform2>();
            VelocityComponent playerVelocity = _player.Get<VelocityComponent>();
            playerVelocity.Velocity += movementDirection;
            if (playerVelocity.Velocity.LengthSquared() < 0.01)
                playerVelocity.Velocity = Vector2.Zero;
            if (Keyboard.GetState().IsKeyDown(Keys.Space)) 
            {
                
                Vector2 newVelocity = playerVelocity.Velocity * 2;

                // Ensure each component is at most 100
                newVelocity.X = Math.Min(newVelocity.X, 100);
                newVelocity.Y = Math.Min(newVelocity.Y, 100);
                // If you also want to allow negative velocity, you should handle that separately
                newVelocity.X = Math.Max(newVelocity.X, -100);  // Allowing negative speeds like a backward dash
                newVelocity.Y = Math.Max(newVelocity.Y, -100);  // Same for the Y component

                playerVelocity.Velocity = newVelocity;
                Console.WriteLine(playerVelocity.Velocity);
            }
            // Update the camera's position with scaled movement direction
            _camera.LookAt(playerTransform.Position);

           slime.Update(gameTime);

            _world.Update(gameTime);
            // Update other game elements
            var mouseState = Mouse.GetState();
            var mousePosition = _camera.ScreenToWorld(mouseState.Position.ToVector2());
            // Do something with mousePosition if necessary
            InputManager.Update();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Camera logic
            var transformMatrix = _camera.GetViewMatrix();
            _spriteBatch.Begin(transformMatrix: transformMatrix, sortMode: default, blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);

            // Render the tilemap
            _mapRenderer.Draw(transformMatrix);
            //Console.WriteLine(_player.Get<Transform2>().Position.ToString() );
            //Console.WriteLine(_player.Get<SpriteComponent>());

            //_spriteBatch.DrawRectangle(new(_player.Get<Transform2>().Position, _player.Get<SpriteComponent>().Texture.Bounds.Size), Color.Black, 2f);
            Vector2 position = _player.Get<Transform2>().Position - 
                   new Vector2(_player.Get<SpriteComponent>().Texture.Width / 2f, 
                            _player.Get<SpriteComponent>().Texture.Height / 2f);

            RectangleF hitbox = new RectangleF(position, _player.Get<SpriteComponent>().Texture.Bounds.Size);

            _spriteBatch.DrawRectangle(hitbox, Color.Black, 2f);
            
            // Render all entities (handled by RenderSystem)
            _world.Draw(gameTime);

            _spriteBatch.End();
        }

        public override void PostUpdate(GameTime gameTime)
        {
            // Example cleanup or post-update logic if needed
        }


    }
}
