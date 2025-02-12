using System;
using Game.Custom.Input;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;

namespace Game.Custom.Components.Systems;

[Flags]
public enum StdActions {
    MOVE_LEFT   = 2 << 0,
    MOVE_RIGHT  = 2 << 1,
    MOVE_UP     = 2 << 2,
    MOVE_DOWN   = 2 << 3,
    DASH        = 2 << 4,
}

public class PlayerSystem : EntityUpdateSystem
{
    private ComponentMapper<PlayerComponent<StdActions>> playerMapper;
    private ComponentMapper<VelocityComponent> velocityMapper;

    public PlayerSystem() : base(Aspect.All(typeof(PlayerComponent<StdActions>), typeof(VelocityComponent))) { }

    public override void Initialize(IComponentMapperService mapperService)
    {
        playerMapper = mapperService.GetMapper<PlayerComponent<StdActions>>();
        velocityMapper = mapperService.GetMapper<VelocityComponent>();
    }

    public override void Update(GameTime gameTime)
    {
        foreach (int entity in ActiveEntities)
        {
            var player = playerMapper.Get(entity);
            var velocity = velocityMapper.Get(entity);

            player.DashTimer.Update(gameTime);

            // Player logic

            if (!player.IsInControl)
            {
                // Temporary fix to prevent friction in Movement System
                velocity.Velocity = player.Direction * 400f;
                continue;
            }

            Vector2 direction = InputManager.GetDirection(
                player.GetKey(StdActions.MOVE_UP),
                player.GetKey(StdActions.MOVE_DOWN),
                player.GetKey(StdActions.MOVE_LEFT),
                player.GetKey(StdActions.MOVE_RIGHT)
            );
            if (direction != Vector2.Zero)
                direction.Normalize();

            player.Direction = direction;

            velocity.Velocity += direction * 1000f * gameTime.GetElapsedSeconds();

            if (velocity.Velocity.X < 0.01 && velocity.Velocity.X > -0.01)
                velocity.Velocity.X = 0f;

            if (velocity.Velocity.Y < 0.01 && velocity.Velocity.Y > -0.01)
                velocity.Velocity.Y = 0f;

            if (player.IsActionJustPressed(StdActions.DASH) && velocity.Velocity.LengthSquared() > 0.01f)
            {
                player.IsInControl = false;
                player.DashTimer.Restart();
            }
        }
    }
}