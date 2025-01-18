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
        // Load necessary resources
        //var playerTexture = _content.Load<Texture2D>("player");       
    }

    public override void Update(GameTime gameTime)
    {
        var mouseState = Mouse.GetState();
        var mousePosition = _camera.ScreenToWorld(mouseState.Position.ToVector2());

        InputManager.Update();
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

