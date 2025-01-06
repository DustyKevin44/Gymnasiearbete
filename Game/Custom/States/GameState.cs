using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using Game.Custom.Graphics;
using Game.Custom.Graphics.Procedural;
using System.Linq;


namespace Game.Custom.States;

public class GameState : State
{
    // Fields for required variables
    private OrthographicCamera _camera;
    private Texture2D _pixel;
    private Vector2 _line;
    private Vector2 _line2;
    private Skeleton _worm;
    private Effect _circleEffect;
    private const int _pixelSize = 8;
    private const float _minRadius = 2f;
    private const float _radie = 50f;
    private const float _radieSquared = _radie * _radie;
    private const float _minRadiusSquared = _minRadius * _minRadius;

    public GameState(Game game, GraphicsDevice graphicsDevice, ContentManager content)
        : base(game, graphicsDevice, content)
    {
        Initialize();
        LoadContent();
    }

    private void Initialize()
    {
        // Initialize your variables
        _line = new Vector2(300, 300); // Example starting position
        _line2 = _line;

        var viewportAdapter = new BoxingViewportAdapter(_game.Window, _graphicsDevice, 800, 480);
        _camera = new OrthographicCamera(viewportAdapter);

        _worm = new Skeleton(new Vector2(200f, 100f))
            .AttachBone(new Bone(Vector2.Zero, 30f, 0f, 3.14f)
                .AttachBone(new Bone(Vector2.Zero, 30f, 0f, 3.14f)
                    .AttachBone(new Bone(Vector2.Zero, 30f, 0f, 3.14f)
                        .AttachBone(new Bone(Vector2.Zero, 30f, 0f, 3.14f)))));
    }

    public void LoadContent()
    {
        // Load necessary resources
        _pixel = new Texture2D(_graphicsDevice, 1, 1);
        _pixel.SetData([Color.White]);

        _circleEffect = _content.Load<Effect>("CircleShader");
    }

    public override void Update(GameTime gameTime)
    {
        // Example: Update _line and _line2 based on mouse input
        var mouseState = Mouse.GetState();
        var mousePosition = _camera.ScreenToWorld(mouseState.Position.ToVector2());

        var delta = mousePosition - _line;
        if (delta.LengthSquared() > _minRadiusSquared)
        {
            delta.Normalize();
            _line += delta * 200f * (float)gameTime.ElapsedGameTime.TotalSeconds; // Example speed
        }

        if (Vector2.DistanceSquared(_line, _line2) >= _radieSquared)
        {
            var delta2 = _line2 - _line;
            delta2.Normalize();
            _line2 = _line + delta2 * _radie;
        }

        _worm.Update(gameTime);
        _worm.SolveIK(mousePosition, _worm.Bones.Last());
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        // Camera logic
        var transformMatrix = _camera.GetViewMatrix();
        spriteBatch.Begin(transformMatrix: transformMatrix, sortMode: SpriteSortMode.Immediate, blendState: BlendState.AlphaBlend);

        // Line drawing (example commented out)
        // Pixel.DrawPerfectLine(spriteBatch, _line, _line2, _pixel, _pixelSize, Color.Red);

        // Test rectangle
        spriteBatch.DrawRectangle(new RectangleF(250, 250, 50, 50), Color.Black, 1f);

        // Worm
        _worm.Draw(spriteBatch);

        // Debug arc
        var rot = MathHelper.PiOver2 * (float)gameTime.TotalGameTime.TotalSeconds;
        Debug.DrawArc(spriteBatch, new Vector2(200, 200), 30f, 20, rot + 2, rot, Color.Red);

        spriteBatch.End();

        // Test circle
        _circleEffect.Parameters["radius"].SetValue(0.5f);
        _circleEffect.Parameters["pixelSize"].SetValue(_pixelSize);
        _circleEffect.Parameters["textureSize"].SetValue(new Vector2(200f, 200f));

        spriteBatch.Begin(transformMatrix: transformMatrix, sortMode: SpriteSortMode.Immediate, blendState: BlendState.AlphaBlend, effect: _circleEffect);
        spriteBatch.FillRectangle(new RectangleF(8 * 8 * 8, 8 * 8f, 200f, 200f), Color.White);
        spriteBatch.End();
    }

    public override void PostUpdate(GameTime gameTime)
    {
        // Example cleanup or post-update logic
    }
}

