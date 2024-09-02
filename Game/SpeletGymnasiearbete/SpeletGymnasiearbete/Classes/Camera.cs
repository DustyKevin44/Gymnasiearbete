using Microsoft.Xna.Framework;

namespace SpeletGymnasiearbete.Classes;

public class Camera(Vector2 position) : IGameObject
{
    public bool Object_is_dying { get; private set; } = false;
    public Vector2 Position = position;

    public void Draw() {}
    public void Queue_kill() { Object_is_dying = true; }
    public void Update(Microsoft.Xna.Framework.GameTime gameTime) {}
}
