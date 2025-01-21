using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using MonoGame.Extended.ViewportAdapters;
using Game.Custom.Input;
using Game.Custom.Tilemap;

namespace Game.Custom.States
{
    public class GameState : State
    {
        private OrthographicCamera _camera;
        private TiledMap _map;
        private TiledMapRenderer _mapRenderer;


        // Utility function to get movement direction
        private static Vector2 GetMovementDirection()
        {
            var state = Keyboard.GetState();
            return new Vector2(
                Utils.GetInputDirection(state.IsKeyDown(Keys.Left), state.IsKeyDown(Keys.Right)),
                Utils.GetInputDirection(state.IsKeyDown(Keys.Up), state.IsKeyDown(Keys.Down))
            );
        }

        public GameState(Game game, GraphicsDevice graphicsDevice, ContentManager content)
            : base(game, graphicsDevice, content)
        {
            Initialize();
            LoadContent();
        }

        private void Initialize()
        {
            // Load the Tiled map
            _map = _content.Load<TiledMap>("tilemap"); // Use the name of your Tiled map file

            // Initialize the TiledMapRenderer
            _mapRenderer = new TiledMapRenderer(_graphicsDevice);

            var viewportAdapter = new BoxingViewportAdapter(_game.Window, _graphicsDevice, 500, 500);
            _camera = new OrthographicCamera(viewportAdapter);
        }

        public void LoadContent()
        {
            // No content loading needed here for now
        }

        public override void Update(GameTime gameTime)
        {
            // Get elapsed time for smooth movement
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Get the movement direction from input
            var movementDirection = GetMovementDirection();

            // Define camera speed
            float cameraSpeed = 300f; // Adjust speed as necessary

            // Update the camera's position with scaled movement direction
            _camera.Move(movementDirection * cameraSpeed * deltaTime);

            // Update other game elements
            var mouseState = Mouse.GetState();
            var mousePosition = _camera.ScreenToWorld(mouseState.Position.ToVector2());
            // Do something with mousePosition if necessary
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Camera logic
            var transformMatrix = _camera.GetViewMatrix();
            spriteBatch.Begin(transformMatrix: transformMatrix, sortMode: SpriteSortMode.Immediate, blendState: BlendState.AlphaBlend);
            System.Console.WriteLine(transformMatrix);

            // Draw the Tiled map
            _mapRenderer.Draw(transformMatrix); // Draw the map with camera transformation

            spriteBatch.End();
        }

        public override void PostUpdate(GameTime gameTime)
        {
            // Example cleanup or post-update logic if needed
        }
    }
}
