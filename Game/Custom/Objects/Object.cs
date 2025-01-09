using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Game.Custom.Objects;

public class Object(Texture2D texture, Vector2 position) : Component
{
	#region Fields
	private Texture2D _texture = texture; // Objektets bild

    public Vector2 _Position = position; // Objektets position

	#endregion

    #region Methods

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
	{

		
	}

	public override void Update(GameTime gameTime)
	{ 

    }
	#endregion
}
