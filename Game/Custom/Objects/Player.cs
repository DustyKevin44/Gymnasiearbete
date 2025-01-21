using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Game.Custom.ObjectComponents;

namespace Game.Custom.Objects;
/* 
public class Player(Texture2D texture, Vector2 startPosition) : Object(texture, startPosition)
{
    private readonly VelocityComponent _input = new();
    private readonly Physics _physics = new(startPosition);
    private readonly Texture2D _texture = texture;

    public override void Update(GameTime gameTime)
    {
        var input = VelocityComponent.GetMovementInput();
        _physics.PosUpdate(gameTime, _Position, input); // 200f is the movement speed
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_texture, _Position, Color.White);
    }
} */
