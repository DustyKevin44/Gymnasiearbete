using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MonoGame.Extended;

using MonoGame.Extended.ViewportAdapters;
namespace Game;


public class Game : Microsoft.Xna.Framework.Game
{
    private Vector2 GetMovementDirection() // Move camera function
{
    var movementDirection = Vector2.Zero;
    var state = Keyboard.GetState();
    if (state.IsKeyDown(Keys.Down))
    {
        movementDirection += Vector2.UnitY;
    }
    if (state.IsKeyDown(Keys.Up))
    {
        movementDirection -= Vector2.UnitY;
    }
    if (state.IsKeyDown(Keys.Left))
    {
        movementDirection -= Vector2.UnitX;
    }
    if (state.IsKeyDown(Keys.Right))
    {
        movementDirection += Vector2.UnitX;
    }
    return movementDirection;
}
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private OrthographicCamera _camera;

    public Game()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();

         var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
        _camera = new OrthographicCamera(viewportAdapter);
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here
        // Camera Update
        const float movementSpeed = -200;
        _camera.Move(GetMovementDirection() * movementSpeed * gameTime.GetElapsedSeconds());

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here
        // Camera logic
         var transformMatrix = _camera.GetViewMatrix();
        _spriteBatch.Begin(transformMatrix: transformMatrix);
        _spriteBatch.DrawRectangle(new RectangleF(250,250,50,50), Color.Black, 1f);
        _spriteBatch.End();


        base.Draw(gameTime);
    }
}
