using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Collisions;
using MonoGame.Extended;
using System.Reflection;

namespace Game.Custom.Components.Systems;


public class EntityColliderSystem() : EntityUpdateSystem(Aspect.All(typeof(Transform2)).One(typeof(CollisionBox), typeof(HitBox), typeof(HurtBox)))
{
    private ComponentMapper<Transform2> _transformMapper;
    private ComponentMapper<CollisionBox> _collisionboxMapper;
    private ComponentMapper<HitBox> _hitboxMapper;
    private ComponentMapper<HurtBox> _hurtboxMapper;
    private const int _iterations = 10;

    public override void Initialize(IComponentMapperService mapperService)
    {
        _transformMapper = mapperService.GetMapper<Transform2>();
        _collisionboxMapper = mapperService.GetMapper<CollisionBox>();
        _hitboxMapper = mapperService.GetMapper<HitBox>();
        _hurtboxMapper = mapperService.GetMapper<HurtBox>();
    }

    public override void Update(GameTime gameTime)
    {
        ColliderBox getCollider(int i)
        {
            if (_collisionboxMapper.Has(i)) return _collisionboxMapper.Get(i);
            if (_hitboxMapper.Has(i)) return _hitboxMapper.Get(i);
            return _hurtboxMapper.Get(i);
        }

        for (int i = 0; i < _iterations; i++)
        {
            foreach (int entity in ActiveEntities)
            {
                var collider = getCollider(entity);
                collider.previousPosition = _transformMapper.Get(entity).Position;
                collider.Bounds.Position += collider.previousPosition;
            }

            Global.CollisionSystem.Update(gameTime);

            foreach (int entity in ActiveEntities)
            {
                var collider = getCollider(entity);
                collider.Bounds.Position -= collider.previousPosition;
            }
        }
    }
}
