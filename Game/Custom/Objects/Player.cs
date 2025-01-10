using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Particles.Modifiers;
using System;
using Game.Custom.ObjectComponent;
namespace Game.Custom.Objects;

public class Player : Object
{
    private readonly VelictyComponent _input;
    private readonly Physics _physics;
    private readonly Texture2D _texture;


    public Player(Texture2D texture, Vector2 startPosition) : base(texture, startPosition)
    {
        _input = new VelictyComponent();
        _physics = new Physics(startPosition);
        _texture = texture;
  
    }

    public override void Update(GameTime gameTime)
    {
        var input = _input.GetMovementInput();
        _physics.PosUpdate(gameTime, _Position, input); // 200f is the movement speed
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        
        spriteBatch.Draw(_texture, _Position, Color.White);
    }
}
