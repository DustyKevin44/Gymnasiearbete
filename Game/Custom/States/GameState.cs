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
using Game.Custom.Tilemap;
using Game.Custom.ObjectComponents;

namespace Game.Custom.States
{
    public class GameState : State
    {
        private OrthographicCamera _camera;
        private TiledMap _map;
        private TiledMapRenderer _mapRenderer;
        private Entity _player;
        private World _world;
  

        private Vector2 _position;
        private Vector2 playerVelocity;
        private EntityManager _entityManager;
        private Texture2D playerTexture;
        private SpriteBatch _spriteBatch;

        // Utility function to get movement direction
        private static Vector2 GetMovementDirection()
        {
            var state = Keyboard.GetState();
            return new Vector2(
                Utils.GetInputDirection(state.IsKeyDown(Keys.Left), state.IsKeyDown(Keys.Right)),
                Utils.GetInputDirection(state.IsKeyDown(Keys.Up), state.IsKeyDown(Keys.Down))
            );
        }

        public GameState(Game game, GraphicsDevice graphicsDevice, ContentManager content, SpriteBatch spriteBatch)
            : base(game, graphicsDevice, content)
        {
            _spriteBatch = spriteBatch;
            Initialize();
            LoadContent();
        }


        private void Initialize()
        {
                // Initialize the world
            _world = new WorldBuilder()
                .AddSystem(new MovementSystem())
                .AddSystem(new RenderSystem(_spriteBatch))
                .Build();


            _player = _world.CreateEntity();

            // Add the components
            _player.Attach(new TransformComponent(_position));
            _player.Attach(new VelocityComponent(playerVelocity = new Vector2(0, 0))); // Moving right
            _player.Attach(new SpriteComponent(playerTexture));


            // Load the Tiled map
            _map = _content.Load<TiledMap>("tilemap"); // Use the name of your Tiled map file

            // Initialize the TiledMapRenderer
            _mapRenderer = new TiledMapRenderer(_graphicsDevice, _map);

            var viewportAdapter = new BoxingViewportAdapter(_game.Window, _graphicsDevice, 500, 500);
            _camera = new OrthographicCamera(viewportAdapter);
        }

        public void LoadContent()
        {
            // No content loading needed here for now
            playerTexture = _content.Load<Texture2D>("cursorButton"); // Ensure you have a "player" texture

        }

        public override void Update(GameTime gameTime)
        {
            // Get elapsed time for smooth movement
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Get the movement direction from input
            var movementDirection = GetMovementDirection();

            // Define camera speed
            float cameraSpeed = 300f; // Adjust speed as necessary
            TransformComponent playerTransform = _player.Get<TransformComponent>();
            // Update the camera's position with scaled movement direction
            _camera.LookAt(movementDirection * cameraSpeed * deltaTime);

            // Update other game elements
            var mouseState = Mouse.GetState();
            var mousePosition = _camera.ScreenToWorld(mouseState.Position.ToVector2());
            // Do something with mousePosition if necessary
        }

        public override void Draw(GameTime gameTime, SpriteBatch _spriteBatch)
        {
            // Camera logic
            var transformMatrix = _camera.GetViewMatrix();
            _spriteBatch.Begin(transformMatrix: transformMatrix, sortMode: SpriteSortMode.Immediate, blendState: BlendState.AlphaBlend);
            System.Console.WriteLine(transformMatrix);

            // Render the tilemap
            _mapRenderer.Draw(transformMatrix);

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
