using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended;

namespace Game.Custom.Components.Systems;


public class ProceduralAnimationSystem : EntityUpdateSystem
{
    private ComponentMapper<Skeleton> _skeletonMapper;
    private ComponentMapper<Transform2> _transformMapper;

    public ProceduralAnimationSystem() : base(Aspect.All(typeof(Skeleton), typeof(Transform2))) { }


    public override void Initialize(IComponentMapperService mapperService)
    {
        _skeletonMapper = mapperService.GetMapper<Skeleton>();
        _transformMapper = mapperService.GetMapper<Transform2>();
    }

    public override void Update(GameTime gameTime)
    {
        foreach (var entity in ActiveEntities)
        {
            var transform = _transformMapper.Get(entity);
            var skeleton = _skeletonMapper.Get(entity);

            foreach (var chain in skeleton.Limbs)
            {
                chain.Anchor += transform.Position;
                Vector2? target = chain?.ETarget?.Get<Transform2>()?.Position;
                if (target.HasValue)
                {
                    chain.Target = target.Value;
                    chain.Update(gameTime);
                }
                else
                {
                    chain.FabrikB();
                }
                chain.Anchor -= transform.Position;
            }
        }
    }
}
