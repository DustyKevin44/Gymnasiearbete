using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Game.Custom.Input;

public static class InputManager{
    private static MouseState _lastMouseState;
    public static bool MouseClicked { get; private set; }
    public static bool MouseRightClicked {get; private set;}
    public static Rectangle MouseRectangle {get; private set;}
    public static float ScreenScale { get; private set;}

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
        var mouseState = Mouse.GetState();
        
        MouseClicked = mouseState.LeftButton == ButtonState.Pressed && _lastMouseState.LeftButton == ButtonState.Released;
        MouseRightClicked = mouseState.RightButton == ButtonState.Pressed && _lastMouseState.RightButton == ButtonState.Released;
        MouseRectangle = new(mouseState.Position.X/2, mouseState.Position.Y/2, 1, 1);

        _lastMouseState = mouseState;
    }

}
