using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;

namespace Game.Custom.Input;

public class Keybind(Keys? key = null, MouseButton? mouseButton = null, Buttons? gamePadButton = null)
{
    public Keys? Key = key;
    public MouseButton? MouseBtn = mouseButton;
    public Buttons? GamePadButton = gamePadButton;
}
