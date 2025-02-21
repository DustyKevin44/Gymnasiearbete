using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Game.Custom.UI;

public class Button(Texture2D texture, SpriteFont font) : UIElement
{
	private MouseState _currentMouse;
	private readonly SpriteFont _font = font;
	private bool _isHovering;
	private MouseState _previousMouse;
	private readonly Texture2D _texture = texture;

	public event EventHandler Click;
	public bool Clicked { get; private set; }
	public Color PenColour = Color.Black;
	public Vector2 Position;
	public Rectangle Rectangle { get => new((int)Position.X, (int)Position.Y, _texture.Width + (int)(2 * xPadding), _texture.Height + (int)(2 * yPadding)); }
	public string Text;

	public float xPadding = 10f;
	public float yPadding = 5f;

	public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
	{
		var color = _isHovering ? Color.Gray : Color.White;
		var rectangle = Rectangle;

		spriteBatch.Draw(_texture, rectangle, color);

		if (!string.IsNullOrEmpty(Text))
		{
			var x = rectangle.X + (rectangle.Width / 2) - (_font.MeasureString(Text).X / 2);
			var y = rectangle.Y + (rectangle.Height / 2) - (_font.MeasureString(Text).Y / 2);

			spriteBatch.DrawString(_font, Text, new Vector2(x, y), PenColour);
		}
	}

	public override void Update(GameTime gameTime)
	{
		_previousMouse = _currentMouse;
		_currentMouse = Mouse.GetState();

		var mouseRectangle = new Rectangle(_currentMouse.X, _currentMouse.Y, 1, 1);

		_isHovering = false;

		if (mouseRectangle.Intersects(Rectangle))
		{
			_isHovering = true;

			if (_currentMouse.LeftButton == ButtonState.Released && _previousMouse.LeftButton == ButtonState.Pressed)
			{
				Click?.Invoke(this, new EventArgs());
			}
		}
	}
}
