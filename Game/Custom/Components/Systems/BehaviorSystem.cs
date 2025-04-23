using Game.Custom.Utilities;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Graphics;

namespace Game.Custom.Components.Systems;


public enum TargetingBehaviour
{
    None,
    GoTowards,
    Circle,
    Avoid,
}

public enum EnemyState
{
    None,
    Idle,
    Targeting,
    Moseying,
}

public class BehaviorSystem : EntityUpdateSystem
{
    private ComponentMapper<Transform2> _transformMapper;
    private ComponentMapper<VelocityComponent> _velocityMapper;
    private ComponentMapper<Behavior> _behaviorMapper;
    private ComponentMapper<AnimatedSprite> _animatedSpriteMapper;

    public BehaviorSystem() : base(Aspect.All(typeof(Transform2), typeof(VelocityComponent), typeof(Behavior))) { }

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

            if (behavior.Target is not null && Utils.TryGet(behavior.Target, out Transform2 targetTransform))
            {
                float distance = Vector2.Distance(transform.Position, targetTransform.Position);
                Vector2 direction = (targetTransform.Position - transform.Position).NormalizedCopy();

                if (behavior.State == EnemyState.Targeting)
                {
                    if (distance > behavior.ViewRadius)
                    {
                        behavior.State = EnemyState.Idle;
                        behavior.IdleTimer.Restart();
                    }
                }
                else
                {
                    if (distance <= behavior.ViewRadius)
                    {
                        behavior.State = EnemyState.Targeting;
                        behavior.IdleTimer.Stop();
                    }
                }

                switch (behavior.Type)
                {
                    case TargetingBehaviour.GoTowards:
                        velocity.Velocity = Vector2.Lerp(velocity.Velocity, direction * behavior.MoveSpeed * gameTime.GetElapsedSeconds(), 0.2f);
                        break;
                    case TargetingBehaviour.Avoid:
                            velocity.Velocity = Vector2.Lerp(velocity.Velocity, -direction * behavior.MoveSpeed * gameTime.GetElapsedSeconds(), 0.2f);
                        break;
                    case TargetingBehaviour.Circle:
                        if (distance > behavior.Distance)
                            velocity.Velocity = Vector2.Lerp(velocity.Velocity, direction * behavior.MoveSpeed * gameTime.GetElapsedSeconds(), 0.2f);
                        else
                            velocity.Velocity = Vector2.Lerp(velocity.Velocity, -direction * behavior.MoveSpeed * gameTime.GetElapsedSeconds(), 0.2f);
                        velocity.Velocity = Vector2.Lerp(velocity.Velocity, Vector2.Rotate(direction, MathHelper.PiOver2) * behavior.MoveSpeed * gameTime.GetElapsedSeconds(), 0.2f);
                        break;
                    default: // Treat unknown type as BehaviourType.None
                        return;
                }
            }
        }

        // {
        //     var transform = _transformMapper.Get(entity);
        //     var velocity = _velocityMapper.Get(entity);
        //     var behavior = _behaviorMapper.Get(entity);
        //     behavior.Elapsed += gameTime.ElapsedGameTime;

        //     if (behavior.Elapsed.TotalSeconds > 5f)
        //     {
        //         if (behavior.Type == 0) // Turn to next which is 1, 1 is idle
        //         {
        //             Console.WriteLine("Now turning to idle");

        //             velocity.Velocity += new Vector2(60, 60);
        //             behavior.Type = 1;
        //             behavior.Elapsed = TimeSpan.Zero;

        //             if (_animatedSpriteMapper.Has(entity))
        //             {
        //                 AnimatedSprite animation = _animatedSpriteMapper.Get(entity);
        //                 animation.SetAnimation("slimeAnimation");
        //             }
        //         }
        //         else if (behavior.Type == 1)
        //         {
        //             Console.WriteLine("Now turning to wander");

        //             behavior.Type = 2;
        //             behavior.Elapsed = TimeSpan.Zero;

        //             if (_animatedSpriteMapper.Has(entity))
        //             {
        //                 AnimatedSprite animation = _animatedSpriteMapper.Get(entity);
        //                 animation.SetAnimation("idleAnimation");
        //             }
        //         }
        //     }

        //     if (behavior.Type == 0)
        //     {
        //         if (behavior.Target is not null)
        //         {
        //             if (_transformMapper.Has(behavior.Target.Id))
        //             {
        //                 // Go towards target
        //                 var delta = behavior.Target.Get<Transform2>().Position - transform.Position;
        //                 Console.WriteLine(behavior.Target is null);
        //                 if (delta != Vector2.Zero)
        //                     delta.Normalize();
        //                 velocity.Velocity += delta * gameTime.GetElapsedSeconds() * 200 * Global.Random.Next(10, 12);
        //             }
        //         }
        //     }
        //     else if (behavior.Type == 2 && behavior.Target is not null && behavior.Target.Has<Transform2>())
        //     {
        //         Global.Random.NextUnitVector(out Vector2 vel);
                
        //         velocity.Velocity += vel * 100 * gameTime.GetElapsedSeconds();

        //         //velocity.Velocity += new Vector2(rnd.Next(-1000, 1000), rnd.Next(-1000, 1000));
        //         var delta = behavior.Target.Get<Transform2>().Position - transform.Position;

        //         if (delta.Length() < 100f)
        //         {
        //             Console.WriteLine("Now found player");
        //             behavior.Type = 0;
        //         }
        //     }
        // }
    }
}
