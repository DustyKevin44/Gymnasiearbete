using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;


using Game.Custom.Input;
using Game.Custom.Graphics;
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
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private OrthographicCamera _camera;

    private Texture2D _pixel;
    private Vector2 _line;
    private Vector2 _line2;

    public const float radie = 50;
    public const float radieSQR = radie*radie;

    public Game()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _line = GraphicsDevice.PresentationParameters.Bounds.Size.ToVector2() / 2f;
        _line2 = _line;
        base.Initialize();

        var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
        _camera = new OrthographicCamera(viewportAdapter);
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _pixel = new(GraphicsDevice, 1, 1);
        _pixel.SetData([Color.White]);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        var mouse_pos = Mouse.GetState().Position.ToVector2();
        _line = Vector2.Lerp(_line, mouse_pos, 0.1f);
        if (Vector2.DistanceSquared(_line, _line2) >= radieSQR)
        {
            var delta = _line2 - _line;
            delta.Normalize();
            _line2 = _line + delta * radie;
        }

        // Camera Update
        const float movementSpeed = -200;
        _camera.Move(GetMovementDirection() * movementSpeed * gameTime.GetElapsedSeconds());

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();
        Pixel.DrawPerfectLine(_spriteBatch, _line, _line2, _pixel, 8, Color.Red);
        _spriteBatch.End();

        // Camera logic
         var transformMatrix = _camera.GetViewMatrix();
        _spriteBatch.Begin(transformMatrix: transformMatrix);
        _spriteBatch.DrawRectangle(new RectangleF(250,250,50,50), Color.Black, 1f);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
