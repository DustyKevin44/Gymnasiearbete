using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;

namespace Game.Custom.Input;

public static class InputManager
{
    private static Dictionary<PlayerIndex, GamePadState> _lastGamePadStates = [];
    private static MouseStateExtended _lastMouseState;
    
    public static bool MouseClicked { get; private set; }
    public static bool MouseRightClicked {get; private set;}
    public static Rectangle MouseRectangle {get; private set;}
    public static float ScreenScale { get; private set;}


    public static bool GamePadButtonJustPressed(Buttons button, PlayerIndex playerIndex)
    {
        return WasGamePadButtonReleased(button, playerIndex) && GamePad.GetState(playerIndex).IsButtonDown(button);
    }

    public static bool GamePadButtonJustReleased(Buttons button, PlayerIndex playerIndex)
    {
        return WasGamePadButtonPressed(button, playerIndex) && GamePad.GetState(playerIndex).IsButtonUp(button);
    }

    public static bool WasGamePadButtonPressed(Buttons button, PlayerIndex playerIndex)
    {
        return _lastGamePadStates.TryGetValue(playerIndex, out GamePadState state) && state.IsButtonDown(button);
    }

    public static bool WasGamePadButtonReleased(Buttons button, PlayerIndex playerIndex)
    {
        return _lastGamePadStates.TryGetValue(playerIndex, out GamePadState state) && state.IsButtonUp(button);
    }

    public static float GetAxis(Keys a, Keys b)
    {
        var kState = Keyboard.GetState();
        return (kState.IsKeyDown(a) ? 1 : 0) - (kState.IsKeyDown(b) ? 1 : 0);
    }

    public static Vector2 GetDirection(Keys Up, Keys Down, Keys Left, Keys Right)
    {
        return new Vector2(GetAxis(Right, Left), GetAxis(Down, Up));
    }

    public static void Update()
    {
        var mouseState = MouseExtended.GetState();
        MouseClicked = mouseState.LeftButton == ButtonState.Pressed && _lastMouseState.LeftButton == ButtonState.Released;
        MouseRightClicked = mouseState.RightButton == ButtonState.Pressed && _lastMouseState.RightButton == ButtonState.Released;
        MouseRectangle = new(mouseState.Position.X/2, mouseState.Position.Y/2, 1, 1);

        // Save state for next frame
        _lastMouseState = mouseState;
        foreach (PlayerIndex index in Enum.GetValues(typeof(PlayerIndex)))
            _lastGamePadStates[index] = GamePad.GetState(index);

        KeyboardExtended.Update();
        MouseExtended.Update();
    }
}
