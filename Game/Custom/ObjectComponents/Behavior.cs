using System;
using System.Collections.Generic;
using System.Numerics;
using AStarSharp;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Timers;

namespace Game.Custom.ObjectComponents;

public class Behavior(int type, TimeSpan elapsed = default, Entity target = null)
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
    public Entity Target = target;
    public Stack<Node> Path;
    public int Type { get; set; } = type;
    public TimeSpan Elapsed { get; set; } = elapsed;
    public enum DirectionFacing;
   
}
