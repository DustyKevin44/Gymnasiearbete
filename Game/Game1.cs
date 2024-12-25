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

    // Red worm drawing
    private const int _pixelSize = 8;
    private Texture2D _pixel;
    private Vector2 _line;
    private Vector2 _line2;

    // Long White worm
    private Skeleton _worm;

    // Properties
    private float _lineSpeed = 200f;
    private const float _minRadius = 2f;
    private const float _minRadiusSquared = _minRadius*_minRadius;
    private const float _radie = 50;
    private const float _radieSquared = _radie*_radie;

    // Circle
    private Effect _circleEffect;

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

        _circleEffect = Content.Load<Effect>("CircleShader");

        var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
        _camera = new OrthographicCamera(viewportAdapter);

        _worm = new(new(), [
            new Bone(new(), [new DistanceConstraint(30f)], [
                new Bone(new(), [new DistanceConstraint(30f)], [
                    new Bone(new(), [new DistanceConstraint(30f)], [
                        new Bone(new(), [new DistanceConstraint(30f)], [
                            new Bone(new(), [new DistanceConstraint(30f)], [
                                new Bone(new(), [new DistanceConstraint(30f)], null)
                            ])
                        ])
                    ])
                ])
            ])
        ]);

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

        // Camera Update
        const float movementSpeed = 200;
        _camera.Move(GetMovementDirection() * movementSpeed * gameTime.GetElapsedSeconds());

        var keyboard = Keyboard.GetState();
        var zoom = Utils.getInputDirection(keyboard.IsKeyDown(Keys.Z), keyboard.IsKeyDown(Keys.X));

        // For natural feeling zoom
        _camera.ZoomIn(zoom * _camera.Zoom / 10f);

        // Line Update
        var mouse_pos = Mouse.GetState().Position.ToVector2();
        var mouse_world_pos = _camera.ScreenToWorld(mouse_pos);

        var delta = mouse_world_pos - _line;
        if (delta.LengthSquared() > _minRadiusSquared)
        {
            delta.Normalize();
            _line += delta * _lineSpeed * gameTime.GetElapsedSeconds();
        }

        if (Vector2.DistanceSquared(_line, _line2) >= _radieSquared)
        {
            var delta2 = _line2 - _line;
            delta2.Normalize();
            _line2 = _line + delta2 * _radie;
        }

        _worm.LocalTransform.Position = mouse_world_pos;
        _worm.Update();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // Camera logic
         var transformMatrix = _camera.GetViewMatrix();
        _spriteBatch.Begin(transformMatrix: transformMatrix, sortMode: SpriteSortMode.Immediate, blendState: BlendState.AlphaBlend);

        // Line drawing
        //Pixel.DrawPerfectLine(_spriteBatch, _line, _line2, _pixel, _pixelSize, Color.Red);

        // Test rectangle
        _spriteBatch.DrawRectangle(new RectangleF(250,250,50,50), Color.Black, 1f);

        // Worm
        _worm.Draw(_spriteBatch);

        _spriteBatch.End();

        // Test circle
        _circleEffect.Parameters["radius"].SetValue(0.5f);
        _circleEffect.Parameters["pixelSize"].SetValue(_pixelSize);
        _circleEffect.Parameters["textureSize"].SetValue(new Vector2(200f, 200f));

        _spriteBatch.Begin(transformMatrix: transformMatrix, sortMode: SpriteSortMode.Immediate, blendState: BlendState.AlphaBlend, effect: _circleEffect);
        _spriteBatch.FillRectangle(new RectangleF(8*8*8, 8*8f, 200f, 200f), Color.White);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
