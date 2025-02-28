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
    private ComponentMapper<VelocityComponent> _velocityMapper;


    public MovementSystem()
        : base(Aspect.All(typeof(Transform2), typeof(CollisionBox), typeof(VelocityComponent)))
    {
    }

    public override void Initialize(IComponentMapperService mapperService)
    {
        _transformMapper = mapperService.GetMapper<Transform2>();
        _velocityMapper = mapperService.GetMapper<VelocityComponent>();
    }

    public override void Update(GameTime gameTime)
    {
        foreach (var entity in ActiveEntities)
        {
            var transform = _transformMapper.Get(entity);
            var velocity = _velocityMapper.Get(entity);
            
            // Example movement logic (move right at 100 pixels per second)
            transform.Position += velocity.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            velocity.Velocity -= velocity.Velocity / 5f;
            if (velocity.Velocity.LengthSquared() < 0.01f)
                velocity.Velocity = Vector2.Zero; // Prevent tiny drift

        }

    }
}
