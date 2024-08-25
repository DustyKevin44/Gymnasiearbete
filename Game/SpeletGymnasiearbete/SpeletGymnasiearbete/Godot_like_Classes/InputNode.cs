using static SpeletGymnasiearbete.Node;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;


namespace SpeletGymnasiearbete;


public class InputNode : Node
{
    private static MouseState _lastMouseState;
    private static Vector2 _direction;
    public static Vector2 Direction {
        get => _direction;
        set {}
    }
    public static Vector2 DirectionNormalized {
        get {
            Vector2 dir = _direction;
            if (dir != Vector2.Zero) dir.Normalize();
            return dir;
        }
        set {}
    }
    public static bool MouseJustClicked { get; private set; }

    public static void Update()
    {
        var KeyboardState = Keyboard.GetState();

        _direction = Vector2.Zero;
        if (KeyboardState.IsKeyDown(Keys.W)) _direction.Y--;
        if (KeyboardState.IsKeyDown(Keys.S)) _direction.Y++;
        if (KeyboardState.IsKeyDown(Keys.A)) _direction.X--;
        if (KeyboardState.IsKeyDown(Keys.D)) _direction.X++;
    
        MouseJustClicked = (Mouse.GetState().LeftButton == ButtonState.Pressed) && (_lastMouseState.LeftButton == ButtonState.Released);
        _lastMouseState = Mouse.GetState();
    }
}