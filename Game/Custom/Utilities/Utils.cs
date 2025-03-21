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

    public static bool TryGet<T>(int entityId, out T component) where T : class
    {
        return TryGet(Global.World.GetEntity(entityId), out component);
    }

    public static bool TryGet<T>(Entity entity, out T component) where T : class
    {
        component = entity.Get<T>(); // component is null if entity does not have T
        return entity.Has<T>();
    }

    public static bool TryGet<T>(ComponentMapper<T> mapper, int entityId, out T component) where T : class
    {
        component = mapper.Get(entityId);
        return mapper.Has(entityId);
    }
}
