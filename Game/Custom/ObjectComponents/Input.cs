using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Game.Custom.ObjectComponents;

public class InputComponent()
{
    //public int _change = change; // The amount of movement
    public static Vector2 GetMovementInput()
    {
        Vector2 movement = Vector2.Zero;
        if (Keyboard.GetState().IsKeyDown(Keys.W)) movement.Y -= 1;
        if (Keyboard.GetState().IsKeyDown(Keys.S)) movement.Y += 1;
        if (Keyboard.GetState().IsKeyDown(Keys.A)) movement.X -= 1;
        if (Keyboard.GetState().IsKeyDown(Keys.D)) movement.X += 1;

        return movement;
    }
}