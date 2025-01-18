using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using Game.Custom.Input;
using Game.Custom.Tilemap;

namespace Game.Custom.States;

public class GameState : State
{
    private OrthographicCamera _camera;
    private Hero _hero;
    private Map _map;
    private readonly GraphicsDeviceManager _graphics;

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
        _map = new(_content);
        _hero = new(_content.Load<Texture2D>("cursorButton"), Vector2.Zero);
        Pathfinder.Init(_map, _hero);

        var viewportAdapter = new BoxingViewportAdapter(_game.Window, _graphicsDevice, 500, 500);
        _camera = new OrthographicCamera(viewportAdapter);
    }

    public void LoadContent()
    {

    // Load the map and pathfinding system
    var map = new Map(_content, _graphicsDevice);
    Pathfinder.Init(map.CollisionLayer);
    }   

    public override void Update(GameTime gameTime)
    { // Get elapsed time for smooth movement
    float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

    // Get the movement direction from input
    var movementDirection = GetMovementDirection();

    // Define camera speed
    float cameraSpeed = 300f; // Adjust speed as necessary

    // Update the camera's position
    _camera.Move(movementDirection * cameraSpeed * deltaTime);

    // Update other game elements
    var mouseState = Mouse.GetState();
    var mousePosition = _camera.ScreenToWorld(mouseState.Position.ToVector2());

    InputManager.Update();
    // BAD CODE REMOVE LATER
    var mouseState = Mouse.GetState();
    if (mouseState.LeftButton == ButtonState.Pressed)
    {
        var mapPosition = map.ScreenToMap(new Vector2(mouseState.X, mouseState.Y));
        var heroPosition = map.ScreenToMap(hero.Position);

        var path = Pathfinder.BFSearch(heroPosition, mapPosition);

        if (path != null)
        {
            hero.SetPath(path.Select(p => map.MapToScreen(new Point((int)p.X, (int)p.Y))).ToList());
        }
    }

    map.Update(gameTime);
    hero.Update(gameTime);

    base.Update(gameTime);
    // END OF BBBAAD CODE
    _map.Update();
    _hero.Update(gameTime);
}

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        // Camera logic
        var transformMatrix = _camera.GetViewMatrix();
        spriteBatch.Begin(transformMatrix: transformMatrix, sortMode: SpriteSortMode.Immediate, blendState: BlendState.AlphaBlend);

        // Draw hero and map
        _map.Draw(spriteBatch);
        _hero.Draw(spriteBatch);

        spriteBatch.End();
    }

    public override void PostUpdate(GameTime gameTime)
    {
        // Example cleanup or post-update logic
    }
}

