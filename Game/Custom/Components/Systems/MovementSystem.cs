using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Tweening;
using MonoGame.Extended;
using Game.Custom.Components;

public class MovementSystem : EntityUpdateSystem
{
    private ComponentMapper<Transform2> _transformMapper;
    private ComponentMapper<ColliderComponent> _colliderMapper;
    private ComponentMapper<VelocityComponent> _velocityMapper;

    private readonly CollisionComponent _collisionWorld;

    public MovementSystem(CollisionComponent collisionWorld)
        : base(Aspect.All(typeof(Transform2), typeof(ColliderComponent), typeof(VelocityComponent)))
    {
        _collisionWorld = collisionWorld;
    }

    public override void Initialize(IComponentMapperService mapperService)
    {
        _transformMapper = mapperService.GetMapper<Transform2>();
        _velocityMapper = mapperService.GetMapper<VelocityComponent>();

        _colliderMapper = mapperService.GetMapper<ColliderComponent>();
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
            transform.Position += velocity.Velocity  * (float)gameTime.ElapsedGameTime.TotalSeconds;
            velocity.Velocity -= velocity.Velocity / 5f;
            // Update collider bounds
            if (collider.Bounds is RectangleF rect)
            {
                rect.Position = transform.Position;
            }

            // Check for collision
            _collisionWorld.Update(gameTime);
        }
    }
}
