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
	public Rectangle Rectangle { get => new((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height); }
	public string Text;

	public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
	{
		var colour = _isHovering ? Color.Gray : Color.White;

		spriteBatch.Draw(_texture, Rectangle, colour);

		if (!string.IsNullOrEmpty(Text))
		{
			var x = Rectangle.X + (Rectangle.Width / 2) - (_font.MeasureString(Text).X / 2);
			var y = Rectangle.Y + (Rectangle.Height / 2) - (_font.MeasureString(Text).Y / 2);

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
