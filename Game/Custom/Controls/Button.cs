using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Game.Custom.Controls;

public class Button(Texture2D texture, SpriteFont font) : Component
{
	#region Fields

	private MouseState _currentMouse; // Vad musen gör nu
	private SpriteFont _font = font;
	private bool _isHovering; // Om musen är över knappen
	private MouseState _previousMouse; // Vad musen gjorde precis
	private Texture2D _texture = texture; // Knappens bild

	#endregion

	#region Properties

	public event EventHandler Click;
	public bool Clicked { get; private set; }
    public Color PenColour { get; set; } = Color.Black;
    public Vector2 Position { get; set; }
	public Rectangle Rectangle { get => new((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height); }
	public string Text { get; set; }

    #endregion

    #region Methods

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
	{
		var colour = Color.White;

		if (_isHovering)
		{ // Om musen håller över så blir knappen grå
			colour = Color.Gray;
		}
		spriteBatch.Draw(_texture, Rectangle, colour);

		if (!string.IsNullOrEmpty(Text))
		{
			var x = (Rectangle.X + (Rectangle.Width / 2)) - (_font.MeasureString(Text).X / 2);
			var y = (Rectangle.Y + (Rectangle.Height / 2)) - (_font.MeasureString(Text).Y / 2);

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

	#endregion
}
