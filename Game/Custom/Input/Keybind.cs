using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;

namespace Game.Custom.Input;

public class Keybind(Keys? key = null, MouseButton? mouseButton = null, Buttons? gamePadButton = null)
{
    public Keys? Key = key;
    public MouseButton? MouseBtn = mouseButton;
    public Buttons? GamePadButton = gamePadButton;
}


public class CustomKeybind(Action<GameTime> action, Keys? key = null, MouseButton? mouseButton = null, Buttons? gamePadButton = null) : Keybind(key, mouseButton, gamePadButton)
{
    public Action<GameTime> Action { get; private set; } = action;
}
