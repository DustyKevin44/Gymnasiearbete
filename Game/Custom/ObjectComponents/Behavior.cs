using System;
using System.Collections.Generic;
using AStarSharp;
using MonoGame.Extended.Timers;

namespace Game.Custom.ObjectComponents;

public class Behavior(int type, TimeSpan elapsed = default)
{
    public enum eState
    {
        Idle, Active, Moseying
    }

    public enum EnemySearchState
    {
        Searching, Found, Alerted, Unreachable
    }
    public eState EnemyState;
    public EnemySearchState SearchState;
    public int stepsTraveled;
    public float EnemyMoveSpeed;
    public Stack<Node> Path;
    public int Type { get; set; } = type;
    public TimeSpan Elapsed { get; set; } = elapsed;
    public enum DirectionFacing;
   
}
