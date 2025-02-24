using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Tweening;
using MonoGame.Extended;
using Game.Custom.Components;
using System;

public class MovementSystem : EntityUpdateSystem
{
    private ComponentMapper<Transform2> _transformMapper;
    private ComponentMapper<CollisionBox> _colliderMapper;
    private ComponentMapper<VelocityComponent> _velocityMapper;
    private bool _collidersAdded = false;

    private readonly CollisionComponent _collisionWorld;

    public MovementSystem(CollisionComponent collisionWorld)
        : base(Aspect.All(typeof(Transform2), typeof(CollisionBox), typeof(VelocityComponent)))
    {
        _collisionWorld = collisionWorld;
    }

    public override void Initialize(IComponentMapperService mapperService)
    {
        _transformMapper = mapperService.GetMapper<Transform2>();
        _velocityMapper = mapperService.GetMapper<VelocityComponent>();

        _colliderMapper = mapperService.GetMapper<CollisionBox>();

    }

    public override void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        foreach (var entity in ActiveEntities)
        {

            var transform = _transformMapper.Get(entity);
            var collider = _colliderMapper.Get(entity);
            var velocity = _velocityMapper.Get(entity);
            // Example movement logic (move right at 100 pixels per second)
            transform.Position += velocity.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            velocity.Velocity -= velocity.Velocity / 5f;
            if (velocity.Velocity.LengthSquared() < 0.01f)
                velocity.Velocity = Vector2.Zero; // Prevent tiny drift
            // Update collider bounds
            collider.Shape.Position = transform.Position;
            // Check for collision
        }
        //Console.WriteLine($"CollisionComponent has {ColliderBox.} colliders registered.");

        _collisionWorld.Update(gameTime);

    }
}
