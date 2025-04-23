using MonoGame.Extended.ECS;
using Game.Custom.Components.Systems;
using MonoGame.Extended.Timers;

namespace Game.Custom.Components;

public class Behavior
{
    public EnemyState State = EnemyState.Idle;
    public TargetingBehaviour Type;
    public float MoveSpeed;
    public float ViewRadius;
    public float Distance;
    public Entity Target;

    public CountdownTimer IdleTimer = new(5);

    public Behavior(TargetingBehaviour type, float moveSpeed, float viewRadius, float distance, Entity target = null)
    {
        Type = type;
        MoveSpeed = moveSpeed;
        ViewRadius = viewRadius;
        Distance = distance;
        Target = target;

        IdleTimer.Completed += (_, _) => { State = (State == EnemyState.Idle) ? EnemyState.Moseying : EnemyState.Idle; };
    }
}
