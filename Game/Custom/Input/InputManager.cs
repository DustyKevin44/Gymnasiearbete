using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Game.Custom.Input;

public static class InputManager{
    private static MouseState _lastMouseState;
    public static bool MouseClicked { get; private set; }
    public static bool MouseRightClicked {get; private set;}
    public static Rectangle MouseRectangle {get; private set;}
    public static float ScreenScale { get; private set;}

    public static void Update()
    {
        var mouseState = Mouse.GetState();
        
        MouseClicked = mouseState.LeftButton == ButtonState.Pressed && _lastMouseState.LeftButton == ButtonState.Released;
        MouseRightClicked = mouseState.RightButton == ButtonState.Pressed && _lastMouseState.RightButton == ButtonState.Released;
        MouseRectangle = new(mouseState.Position.X/2, mouseState.Position.Y/2, 1, 1);

        _lastMouseState = mouseState;
    }

}
