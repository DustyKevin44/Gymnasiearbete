using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#nullable enable

using static SpeletGymnasiearbete.Utils;
namespace SpeletGymnasiearbete.Classes;


public abstract class IObject2D(Texture2D? texture, Vector2 position) : IGameObject
{
    public bool Object_is_dying { get; private set; } = false;

    public Vector2 Position = position;
    public Texture2D? Texture = texture;

    public void Queue_kill() { Object_is_dying = true; }
    public abstract void Draw();
    public abstract void Update(GameTime gameTime);
}

public class Projectile(Texture2D? texture, Vector2 position, Vector2 velocity, float speed, float damage = 0f, Projectile[]? payload = null) : IObject2D(texture, position)
{
    public Vector2 Velocity { get; private set; } = velocity;
    public Vector2 Acceleration { get; private set; } = Vector2.Zero;

    public float Speed { get; private set; } = speed;
    public float Damage { get; private set; } = damage;
    private readonly Projectile[] _payload = (payload is null) ? [] : payload;

    public override void Draw()
    {
        Vector2 camera_pos = (Globals.Active_Camera is Camera camera) ? camera.Position : Vector2.Zero;

        Globals.SpriteBatch.Begin();
        Globals.SpriteBatch.Draw(Texture, Position + camera_pos, Color.White);
        Globals.SpriteBatch.End();
    }

    public override void Update(GameTime gameTime)
    {
        float delta = GameTimeToDelta(gameTime);

        Velocity += Acceleration * delta;
        Position += Velocity * delta;
    }
}

public class Grenade(Vector2 position, Vector2 velocity) : Projectile(null, position, velocity, 0f, 0f, 
[
    new Projectile(null, position,  Vector2.UnitX,     100f, 0f),
    new Projectile(null, position, -Vector2.UnitX,     100f, 0f),
    new Projectile(null, position,  Vector2.UnitY,     100f, 0f),
    new Projectile(null, position, -Vector2.UnitY,     100f, 0f),
    new Projectile(null, position,  Vector2.One,       100f, 0f),
    new Projectile(null, position, -Vector2.One,       100f, 0f),
    new Projectile(null, position, new Vector2(-1, 1), 100f, 0f),
    new Projectile(null, position, new Vector2(1, -1), 100f, 0f)
]);