using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Tweening;
using MonoGame.Extended;
using Game.Custom.Components;
using System;

public class EntityColliderSystem : EntityUpdateSystem
{
    private ComponentMapper<Transform2> _transformMapper;
    private ComponentMapper<CollisionBox> _colliderMapper;
    private ComponentMapper<VelocityComponent> _velocityMapper;

    private readonly CollisionComponent _collisionWorld;

    public EntityColliderSystem(CollisionComponent collisionWorld)
        : base(Aspect.All(typeof(Transform2), typeof(CollisionBox)))
    {
        _collisionWorld = collisionWorld;
    }

    public override void Initialize(IComponentMapperService mapperService)
    {
        _transformMapper = mapperService.GetMapper<Transform2>();
        _colliderMapper = mapperService.GetMapper<CollisionBox>();
        _velocityMapper = mapperService.GetMapper<VelocityComponent>();
    }

    public override void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        foreach (var entity in ActiveEntities)
        {

        }

        _collisionWorld.Update(gameTime);

        // Second pass: Resolve overlap by moving entities apart
        foreach (var entity in ActiveEntities)
        {
            var transform = _transformMapper.Get(entity);
            var collider = _colliderMapper.Get(entity);


            if (collider.onCollisionBool)
            {
            
                if (_velocityMapper.Has(entity))
                {
                    // Resolve overlap by moving the entity out of collision
                    transform.Position -= collider.CollisionInfo.PenetrationVector;
                    collider.Shape.Position = transform.Position; // Sync collider position
                    var velocity = _velocityMapper.Get(entity);
                    velocity.Velocity = Vector2.Zero;

                }
                // Reset collision state
                collider.onCollisionBool = false;
                //collider.CollisionInfo.PenetrationVector = Vector2.Zero;
            }
            collider.Bounds.Position -= transform.Position;
        }

        //Console.WriteLine($"CollisionComponent has {ColliderBox.} colliders registered.");
    }
}
