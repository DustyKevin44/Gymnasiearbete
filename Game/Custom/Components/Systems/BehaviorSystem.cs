using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Graphics;
using System;

namespace Game.Custom.Components.Systems;

public class BehaviorSystem : EntityUpdateSystem
{
    private ComponentMapper<Transform2> _transformMapper;
    private ComponentMapper<VelocityComponent> _velocityMapper;
    private ComponentMapper<Behavior> _behaviorMapper;
    private ComponentMapper<AnimatedSprite> _animatedSpriteMapper;

    public BehaviorSystem() : base(Aspect.All(typeof(Transform2), typeof(VelocityComponent), typeof(Behavior), typeof(AnimatedSprite))) { }

    public override void Initialize(IComponentMapperService mapperService)
    {
        _transformMapper = mapperService.GetMapper<Transform2>();
        _velocityMapper = mapperService.GetMapper<VelocityComponent>();
        _behaviorMapper = mapperService.GetMapper<Behavior>();
        _animatedSpriteMapper = mapperService.GetMapper<AnimatedSprite>();
    }

    public override void Update(GameTime gameTime)
    {
        foreach (var entity in ActiveEntities)
        {
            var transform = _transformMapper.Get(entity);
            var velocity = _velocityMapper.Get(entity);
            var behavior = _behaviorMapper.Get(entity);
            behavior.Elapsed += gameTime.ElapsedGameTime;
            
            if (behavior.Elapsed.TotalSeconds > 5f)
            {
                if(behavior.Type == 0)
                {
                    velocity.Velocity += new Vector2(60, 60);
                    behavior.Type = 1;
                    behavior.Elapsed = TimeSpan.Zero;
                    
                    if(_animatedSpriteMapper.Has(entity))
                    {
                        AnimatedSprite animation = _animatedSpriteMapper.Get(entity);
                        animation.SetAnimation("slimeAnimation");
                    }
                }
                else
                {
                    behavior.Type = 0;
                    behavior.Elapsed = TimeSpan.Zero;

                    if(_animatedSpriteMapper.Has(entity))
                    {
                        AnimatedSprite animation = _animatedSpriteMapper.Get(entity);
                        animation.SetAnimation("idleAnimation");
                    }
                }
            }

            if (behavior.Type == 0)
            {
                if (behavior.Target.Has<Transform2>())
                {
                    var delta = behavior.Target.Get<Transform2>().Position - transform.Position;

                    if (delta != Vector2.Zero)
                        delta.Normalize();
                    velocity.Velocity += delta * gameTime.GetElapsedSeconds() * 1500;
                }
            }
        }
    }
}
