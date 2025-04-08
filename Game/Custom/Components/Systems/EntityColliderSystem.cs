using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.ECS;
using MonoGame.Extended;
using Game.Custom.Utilities;
using System.Collections.Generic;

namespace Game.Custom.Components.Systems;


public class EntityColliderSystem() : EntityUpdateSystem(Aspect.All(typeof(Transform2)).One(typeof(CollisionBox), typeof(HitBox), typeof(HurtBox)))
{
    private ComponentMapper<Transform2> _transformMapper;
    private ComponentMapper<CollisionBox> _collisionboxMapper;
    private ComponentMapper<HitBox> _hitboxMapper;
    private ComponentMapper<HurtBox> _hurtboxMapper;
    private ComponentMapper<Equipable> _equipableMapper;
    private const int _iterations = 10;

    public override void Initialize(IComponentMapperService mapperService)
    {
        _transformMapper = mapperService.GetMapper<Transform2>();
        _collisionboxMapper = mapperService.GetMapper<CollisionBox>();
        _hitboxMapper = mapperService.GetMapper<HitBox>();
        _hurtboxMapper = mapperService.GetMapper<HurtBox>();
        _equipableMapper = mapperService.GetMapper<Equipable>();
    }

    public override void Update(GameTime gameTime)
    {
        IEnumerable<ColliderBox> getColliders(int i)
        {
            if (_collisionboxMapper.Has(i)) yield return _collisionboxMapper.Get(i);
            if (_hitboxMapper.Has(i)) yield return _hitboxMapper.Get(i);
            if (_hurtboxMapper.Has(i)) yield return _hurtboxMapper.Get(i);
        }

        for (int i = 0; i < _iterations; i++)
        {
            foreach (int entity in ActiveEntities)
            {
                foreach (var collider in getColliders(entity))
                {
                    Vector2 offset = Vector2.Zero;

                    if (Utils.TryGet(_equipableMapper, entity, out Equipable eq) &&
                        Utils.TryGet(eq.Parent, out Transform2 transform))
                    {
                        offset = transform.Position;
                    }

                    collider.currentParentPosition = _transformMapper.Get(entity).Position + offset; // Parent position + (if weapon: users position)
                    collider.Bounds.Position += collider.currentParentPosition;
                }
            }

            Global.CollisionSystem.Update(gameTime);

            foreach (int entity in ActiveEntities)
            {
                foreach(var collider in getColliders(entity))
                {
                    collider.Bounds.Position -= collider.currentParentPosition;
                }
            }
        }
    }
}
