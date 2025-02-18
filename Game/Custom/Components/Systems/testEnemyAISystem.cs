using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended;
using System;
using Game.Custom.Components;



// Base AI Behavior Interface
public interface IAIBehavior {
    void Idle(GameTime gameTime, Transform2 position, VelocityComponent velocity);
    void Chase(GameTime gameTime, Transform2 position, VelocityComponent velocity, Vector2 playerPosition);
    void Attack(GameTime gameTime, Transform2 position, VelocityComponent velocity);
}

// Slime AI Behavior
public class SlimeBehavior : IAIBehavior {
    private float idleTime = 2f;
    private float idleTimer;

    public SlimeBehavior() {
        idleTimer = idleTime;
    }

    public void Idle(GameTime gameTime, Transform2 position, VelocityComponent velocity) {
        idleTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (idleTimer <= 0) {
            idleTimer = idleTime;
            velocity.Velocity = new Vector2((float)(new Random().NextDouble() * 2 - 1), (float)(new Random().NextDouble() * 2 - 1));
            velocity.Velocity.Normalize();
            velocity.Velocity *= velocity.Velocity;
        }
    }

    public void Chase(GameTime gameTime, Transform2 position, VelocityComponent velocity, Vector2 playerPosition) {
        Vector2 direction = Vector2.Normalize(playerPosition - position.Position);
        velocity.Velocity = direction * velocity.Velocity;
    }

    public void Attack(GameTime gameTime, Transform2 position, VelocityComponent velocity) {
        velocity.Velocity = Vector2.Zero;
    }
}

// Zombie AI Behavior
public class ZombieBehavior : IAIBehavior {
    public void Idle(GameTime gameTime, Transform2 position, VelocityComponent velocity) {
        velocity.Velocity = Vector2.Zero;
    }

    public void Chase(GameTime gameTime, Transform2 position, VelocityComponent velocity, Vector2 playerPosition) {
        Vector2 direction = Vector2.Normalize(playerPosition - position.Position);
        velocity.Velocity = direction * (velocity.Velocity * 0.5f); // Zombies move slower
    }

    public void Attack(GameTime gameTime, Transform2 position, VelocityComponent velocity) {
        velocity.Velocity = Vector2.Zero;
    }
}

// AI Component
public class AIComponent {
    public IAIBehavior Behavior;
    public float DetectionRange = 150f;
    public float AttackRange = 50f;
    public Vector2 PlayerPosition;

    public AIComponent(IAIBehavior behavior) {
        Behavior = behavior;
    }
}

// AI System
public class AISystem : EntityUpdateSystem {
    private ComponentMapper<Transform2> positionMapper;
    private ComponentMapper<VelocityComponent> velocityMapper;
    private ComponentMapper<AIComponent> aiMapper;

    public AISystem() : base(Aspect.All(typeof(Transform2), typeof(VelocityComponent), typeof(AIComponent))) { }

    public override void Initialize(IComponentMapperService mapperService) {
        positionMapper = mapperService.GetMapper<Transform2>();
        velocityMapper = mapperService.GetMapper<VelocityComponent>();
        aiMapper = mapperService.GetMapper<AIComponent>();
    }

    public override void Update(GameTime gameTime) {
        foreach (var entityId in ActiveEntities) {
            var position = positionMapper.Get(entityId);
            var velocity = velocityMapper.Get(entityId);
            var ai = aiMapper.Get(entityId);

            float distanceToPlayer = Vector2.Distance(position.Position, ai.PlayerPosition);

            if (distanceToPlayer < ai.AttackRange) {
                ai.Behavior.Attack(gameTime, position, velocity);
            } else if (distanceToPlayer < ai.DetectionRange) {
                ai.Behavior.Chase(gameTime, position, velocity, ai.PlayerPosition);
            } else {
                ai.Behavior.Idle(gameTime, position, velocity);
            }
        }
    }
}

