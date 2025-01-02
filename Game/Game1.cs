using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using Game.Custom.Input;
using Game.Custom.Graphics;
using MonoGame.Extended.ECS;
using Game.Custom.Graphics.Procedural;
using System;
using MonoGame.Extended.Input;
using System.Linq;
using Game.States;

namespace Game;


public class Game : Microsoft.Xna.Framework.Game
{
    // Move camera function
    private static Vector2 GetMovementDirection()
    {
        var state = Keyboard.GetState();
        return new Vector2(
            Utils.getInputDirection(state.IsKeyDown(Keys.Left), state.IsKeyDown(Keys.Right)),
            Utils.getInputDirection(state.IsKeyDown(Keys.Up), state.IsKeyDown(Keys.Down))
        );
    }
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private OrthographicCamera _camera;

    // Game states
    private State _currentState;
    private State _nextState;
    public void ChangeState(State state)
    {
        _nextState = state;
    }
    public Game()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }
    protected override void Initialize()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

    _camera = new OrthographicCamera(GraphicsDevice);  // Initialize the camera
    _currentState = new MenuState(this, GraphicsDevice, Content);
    base.Initialize();
    }
    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // State logic
        if (_nextState != null)
        {
            _currentState = _nextState;

            _nextState = null;
        }
        _currentState.Update(gameTime);
        _currentState.PostUpdate(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        // Draw state
        _currentState.Draw(gameTime, _spriteBatch);
      

        base.Draw(gameTime);
    }
}
