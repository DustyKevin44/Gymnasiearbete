using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Particles.Modifiers;
using System;

namespace Game.Custom.ObjectComponent;

public class Physics(Vector2 position) : Component
{
	#region Fields
    public Vector2 _velocity { get; private set; }

    public Vector2 _position = position; // Objektets position

	#endregion

    #region Methods

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
	{

		
	}

	public override void Update(GameTime gameTime)
	{ 
    }
    public void PosUpdate(GameTime gameTime, Vector2 _position, Vector2 _velocity)
	{ 
        _position += _velocity * gameTime.GetElapsedSeconds(); 
    }
	#endregion
}

