using AStarSharp;
using MonoGame.Extended.ECS;
using System.Collections.Generic;
using System;

namespace Game.Custom.Components;

public class Behavior(int type, TimeSpan elapsed = default, Entity target = null)
{
    public enum EnemyState
    {
        Idle, Active, Moseying
    }

    public enum EnemySearchState
    {
        Searching, Found, Alerted, Unreachable
    }

    public EnemyState State;
    public EnemySearchState SearchState;
    
    public int StepsTraveled;
    public float EnemyMoveSpeed;
    
    public Entity Target = target;
    public int Type { get; set; } = type;
    
    public TimeSpan Elapsed { get; set; } = elapsed;
    public enum DirectionFacing;
}
