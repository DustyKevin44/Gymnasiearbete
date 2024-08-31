using Microsoft.Xna.Framework;

namespace SpeletGymnasiearbete.Classes;

// Blueprint for GameObjects
public interface IGameObject
{
    // If marked for deletion
    public bool Object_is_dying { get; }

    // Updating each frame
    void Update(GameTime gameTime);
    // Drawing on screen
    void Draw();
    // To be able to delete GameObjects
    void Queue_kill();
}