using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using Game.Custom.GameStates;

namespace Game;


public class Game : Microsoft.Xna.Framework.Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private OrthographicCamera _camera;

    // Game states
    private GameState _currentState;
    private GameState _nextState;

    public void ChangeState(GameState state) { _nextState = state; }

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
        Point WindowSize = new(1200, 900);
        _graphics.PreferredBackBufferWidth = WindowSize.X;
        _graphics.PreferredBackBufferHeight = WindowSize.Y;
        _graphics.ApplyChanges();

        base.Initialize();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
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
        _currentState.Draw(gameTime, _spriteBatch);  // Draw state

        base.Draw(gameTime);
    }
}
