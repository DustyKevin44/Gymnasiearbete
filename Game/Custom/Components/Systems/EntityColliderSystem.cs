using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Collisions;
using MonoGame.Extended;
using System.Reflection;

namespace Game.Custom.Components.Systems;


public class EntityColliderSystem() : EntityUpdateSystem(Aspect.All(typeof(Transform2), typeof(CollisionBox)))
{
    private ComponentMapper<Transform2> _transformMapper;
    private ComponentMapper<CollisionBox> _colliderMapper;

    public override void Initialize(IComponentMapperService mapperService)
    {
        _transformMapper = mapperService.GetMapper<Transform2>();
        _colliderMapper = mapperService.GetMapper<CollisionBox>();
    }

    public override void Update(GameTime gameTime)
    {
        for (int i = 0; i < 10; i++)
        {
            foreach (int entity in ActiveEntities)
            {
                var collider = _colliderMapper.Get(entity);
                collider.previousPosition = _transformMapper.Get(entity).Position;
                collider.Bounds.Position += collider.previousPosition;
            }

            Global.CollisionSystem.Update(gameTime);
            
            foreach (int entity in ActiveEntities)
            {
                var collider = _colliderMapper.Get(entity);
                collider.Bounds.Position -= collider.previousPosition;
            }
        }
    }
}
