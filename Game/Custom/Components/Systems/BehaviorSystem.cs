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
                if (behavior.Type == 0) // Turn to next which is 1, 1 is idle
                {
                    velocity.Velocity += new Vector2(60, 60);
                    behavior.Type = 1;
                    behavior.Elapsed = TimeSpan.Zero;
                    Console.WriteLine("Now turning to idle");
                    if (_animatedSpriteMapper.Has(entity))
                    {
                        AnimatedSprite animation = _animatedSpriteMapper.Get(entity);
                        animation.SetAnimation("slimeAnimation");
                    }
                }
                else if (behavior.Type == 1)
                {
                    Console.WriteLine("Now turning to wander");

                    behavior.Type = 2;
                    behavior.Elapsed = TimeSpan.Zero;

                    if (_animatedSpriteMapper.Has(entity))
                    {
                        AnimatedSprite animation = _animatedSpriteMapper.Get(entity);
                        animation.SetAnimation("idleAnimation");
                    }
                }
            }

            if (behavior.Type == 0)
            {
                try
                {

                    if (_transformMapper.Has(behavior.Target.Id))
                    {
                        // Åk mot spelaren. 
                        var delta = behavior.Target.Get<Transform2>().Position - transform.Position - behavior.Target.Get<SpriteComponent>().Texture.Bounds.Center.ToVector2();
                        Random rnd = new Random();
                        if (delta != Vector2.Zero)
                            delta.Normalize();
                        velocity.Velocity += delta * gameTime.GetElapsedSeconds() * 200 * rnd.Next(10, 12);
                    }
                }
                catch
                {
                    Console.WriteLine("Error" + behavior.Target);
                }
            }
            else if (behavior.Type == 2)
            {
                Random rnd = new Random();

                Vector2 vel;
                rnd.NextUnitVector(out vel);
                velocity.Velocity += vel * 1000 * gameTime.GetElapsedSeconds();

                //velocity.Velocity += new Vector2(rnd.Next(-1000, 1000), rnd.Next(-1000, 1000));
                var delta = behavior.Target.Get<Transform2>().Position - transform.Position;

                if (delta.Length() < 100f)
                {
                    Console.WriteLine("Now found player");
                    behavior.Type = 0;
                }

            }

        }
    }
}
