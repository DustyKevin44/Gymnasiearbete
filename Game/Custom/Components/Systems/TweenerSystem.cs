using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Tweening;

namespace Game.Custom.Components.Systems;


public class TweenerSystem() : EntityUpdateSystem(Aspect.All(typeof(Tweener)))
{
    private ComponentMapper<Tweener> _tweenerMapper;

    public override void Initialize(IComponentMapperService mapperService)
    {
        _tweenerMapper = mapperService.GetMapper<Tweener>();
    }

    public override void Update(GameTime gameTime)
    {
        foreach (var entity in ActiveEntities)
        {
            var tweener = _tweenerMapper.Get(entity);
            tweener.Update(gameTime.GetElapsedSeconds());
        }
    }
}
