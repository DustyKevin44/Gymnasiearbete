
namespace Game.Custom.Input;


public static class Utils {
    public static int getInputDirection(bool here, bool there) {
        return (there ? 1 : 0) - (here ? 1 : 0);
    }
}
