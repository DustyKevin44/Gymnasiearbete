using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Animations;
using System;
using System.Reflection;

namespace Game.Custom.UI;

public class Button : UIElement
{
    private MouseState _currentMouse;
    private readonly SpriteFont _font;
    private bool _isHovering;
    private MouseState _previousMouse;
    private readonly AnimatedSprite _animatedSprite;
	private readonly float _scale = 6f;


    public event EventHandler Click;
    public bool Clicked { get; private set; }
    public Color PenColour = Color.Black;
    public Vector2 Position;
    public string Text;

    public float xPadding = 10f;
    public float yPadding = 5f;

    public Rectangle Rectangle
        => new((int)Position.X, (int)Position.Y, (int)_animatedSprite.TextureRegion.Width + (int)(2 * xPadding), (int)_animatedSprite.TextureRegion.Height + (int)(2 * yPadding));

    public Button(AnimatedSprite animatedSprite, SpriteFont font)
    {
        _animatedSprite = animatedSprite;
        _font = font;
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        var rectangle = Rectangle;

		spriteBatch.Draw(_animatedSprite, Position,0, new Vector2(_scale, _scale));
        if (!string.IsNullOrEmpty(Text))
        {
            var x = rectangle.X + (rectangle.Width * _scale / 2) - (_font.MeasureString(Text).X / 2);
            var y = rectangle.Y + (rectangle.Height * _scale / 2) - (_font.MeasureString(Text).Y / 2);

            spriteBatch.DrawString(_font, Text, new Vector2(x, y), PenColour, 0, 0, 0, SpriteEffects.None, 1);
        }
    }

    public override void Update(GameTime gameTime)
    {
        _previousMouse = _currentMouse;
        _currentMouse = Mouse.GetState();
		_animatedSprite.Update(gameTime);
        var mouseRectangle = new Rectangle(_currentMouse.X, _currentMouse.Y, 1, 1);
        _isHovering = mouseRectangle.Intersects(Rectangle);

        if (_isHovering)
        {
            if (_currentMouse.LeftButton == ButtonState.Released && _previousMouse.LeftButton == ButtonState.Pressed)
            {
                Click?.Invoke(this, new EventArgs());
            }
        }
    }
}
