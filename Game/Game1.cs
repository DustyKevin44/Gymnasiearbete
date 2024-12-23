using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Game.Custom.Graphics;
namespace Game;

public class Game : Microsoft.Xna.Framework.Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

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

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        Utils.DrawPixelPerfectLine(_spriteBatch, _line, _line2, _pixel, 16, Color.Red);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
