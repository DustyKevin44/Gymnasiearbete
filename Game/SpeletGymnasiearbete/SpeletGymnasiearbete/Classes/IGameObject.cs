using Microsoft.Xna.Framework;

namespace SpeletGymnasiearbete.Classes;

public interface IGameObject
{
    public bool Object_is_dying { get; }

    void Update(GameTime gameTime);
    void Draw();
    void Queue_kill();
}