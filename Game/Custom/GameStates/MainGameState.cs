using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Game.Custom.Input;
using Game.Custom.ObjectComponents;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.Extended.Tiled.Renderers;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.ECS;
using MonoGame.Extended;

namespace Game.Custom.GameStates
{
    public class MainGameState : GameState
    {
        private OrthographicCamera _camera;
        private TiledMap _map;
        private TiledMapRenderer _mapRenderer;
        private Entity _player;
        private World _world;

        private Texture2D playerTexture;
        private Texture2D entityTexture;
        private SpriteBatch _spriteBatch;

        private EntityManager _entityManager;
        private List<Entity> enemyList;

        public MainGameState(Game game, GraphicsDevice graphicsDevice, ContentManager content)
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

            _player.Attach(new Transform2(Vector2.Zero));
            _player.Attach(new VelocityComponent(Vector2.Zero));
            _player.Attach(new SpriteComponent(playerTexture));

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
            Entity entity = _world.CreateEntity();
            entity.Attach(new Transform2(new Vector2(100, 100)));
            entity.Attach(new VelocityComponent(new(100, 0)));
            entity.Attach(new Behavior(0, target: _player));
            entity.Attach(new AnimatedSprite(spriteSheet, "slimeAnimation"));

            // Load the Tiled map
            _map = _content.Load<TiledMap>("tileSetWith2Tileset"); // Use the name of your Tiled map file

            // Initialize the TiledMapRenderer
            _mapRenderer = new TiledMapRenderer(_graphicsDevice, _map);

            var viewportAdapter = new ScalingViewportAdapter(_graphicsDevice, 200, 150);
            _camera = new OrthographicCamera(viewportAdapter);
        }

        public override void LoadContent() { }

        public override void Update(GameTime gameTime)
        {
            // Get the movement direction from input
            var movementDirection = InputManager.GetDirection(Keys.W, Keys.S, Keys.A, Keys.D);

            if (InputManager.MouseClicked)
                _camera.ZoomOut(0.2f);

            Transform2 playerTransform = _player.Get<Transform2>();
            VelocityComponent playerVelocity = _player.Get<VelocityComponent>();
            playerVelocity.Velocity += movementDirection * 1000f * gameTime.GetElapsedSeconds(); // 200 u/s^2

            if (playerVelocity.Velocity.LengthSquared() < 0.01)
                playerVelocity.Velocity = Vector2.Zero;

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
                playerVelocity.Velocity = Vector2.Clamp(playerVelocity.Velocity * 2f, Vector2.One * -100f, Vector2.One * 100f);

            // Update the camera's position with scaled movement direction
            _camera.LookAt(playerTransform.Position); // <-- should be in _world.Update() probably
            _world.Update(gameTime);
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

            _spriteBatch.End();
        }

        public override void PostUpdate(GameTime gameTime) { }
    }
}
