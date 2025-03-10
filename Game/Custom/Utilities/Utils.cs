using MonoGame.Extended.ECS;
using MonoGame.Extended.Tweening;

namespace Game.Custom.Utilities;


public static class Utils
{
    public static Tweener GetTweener(Entity entity)
    {
        if (entity.Has<Tweener>())
            return entity.Get<Tweener>();
        var tweener = new Tweener();
        entity.Attach(tweener);
        return tweener;
    }
}
