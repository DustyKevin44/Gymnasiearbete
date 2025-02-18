using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Game.Custom.Components;
using Microsoft.Xna.Framework.Input;
using Game.Custom.Input;

namespace Game.Custom.Objects;

public class Player(Texture2D texture, Vector2 startPosition) : Object(texture, startPosition)
{
    private readonly VelocityComponent _input = new(Vector2.Zero);
    private readonly PhysicsOutdated _physics = new(startPosition);
    private readonly Texture2D _texture = texture;

    public override void Update(GameTime gameTime)
    {
        _physics.Velocity = InputManager.GetDirection(Keys.W, Keys.S, Keys.A, Keys.D);
        _physics.Update(gameTime);
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_texture, Position, Color.White);
    }
}
