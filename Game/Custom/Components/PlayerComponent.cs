using System;
using System.Collections.Generic;
using Game.Custom.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using MonoGame.Extended.Timers;

namespace Game.Custom.Components;

public class PlayerComponent<T> where T : Enum
{
    public Dictionary<T, Keybind> _keybinds;
    public string Name { get; private set; }
    public PlayerIndex _playerIndex { get; private set; }
    public bool IsInControl = true;
    public CountdownTimer DashTimer = new(0.3f);
    public Vector2 Direction = Vector2.Zero;

    public PlayerComponent(string name, Dictionary<T, Keybind> keybinds)
    {
        _keybinds = keybinds;
        Name = name;
        DashTimer.Completed += (_, _) => IsInControl = true;
        DashTimer.Enabled = true;
    }

    // Helper Functions

    public Keys GetKey(T action)
    {
        return _keybinds.TryGetValue(action, out Keybind keybind) ? (keybind.Key ?? Keys.None) : Keys.None;
    }

    public Buttons GetGamePadButton(T action)
    {
        return _keybinds.TryGetValue(action, out Keybind keybind) ? (keybind.GamePadButton ?? Buttons.None) : Buttons.None;
    }

    public MouseButton GetMouseButton(T action)
    {
        return _keybinds.TryGetValue(action, out Keybind keybind) ? (keybind.MouseBtn ?? MouseButton.None) : MouseButton.None;
    }
    
    public bool IsActionPressed(T action)
    {
        if (!_keybinds.TryGetValue(action, out Keybind keybind))
            return false;
        
        if (keybind.Key.HasValue && Keyboard.GetState().IsKeyDown(keybind.Key.Value))
            return true;
        
        if (keybind.MouseBtn.HasValue && MouseExtended.GetState().IsButtonDown(keybind.MouseBtn.Value))
            return true;
        
        if (keybind.GamePadButton.HasValue && GamePad.GetState(_playerIndex).IsButtonDown(keybind.GamePadButton.Value))
            return true;
        
        return false;
    }

    public bool IsActionJustPressed(T action)
    {
        if (!_keybinds.TryGetValue(action, out Keybind keybind))
            return false;
        
        if (keybind.Key.HasValue && KeyboardExtended.GetState().WasKeyPressed(keybind.Key.Value))
            return true;

        if (keybind.MouseBtn.HasValue && MouseExtended.GetState().WasButtonPressed(keybind.MouseBtn.Value))
            return true;

        if (keybind.GamePadButton.HasValue && InputManager.GamePadButtonJustPressed(keybind.GamePadButton.Value, _playerIndex))
            return true;

        return false;
    }
}
