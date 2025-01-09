using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Particles.Modifiers;
using System;

namespace Game.Custom.ObjectComponent;

public class VelictyComponent()
{
    //public int _change = change; // The amount of movement
    public Vector2 GetMovementInput()
    {
        Vector2 movement = Vector2.Zero;
        if (Keyboard.GetState().IsKeyDown(Keys.W)) movement.Y -= 1;
        if (Keyboard.GetState().IsKeyDown(Keys.S)) movement.Y += 1;
        if (Keyboard.GetState().IsKeyDown(Keys.A)) movement.X -= 1;
        if (Keyboard.GetState().IsKeyDown(Keys.D)) movement.X += 1;

        return movement;
    }
}