using System;
using MonoGame.Extended.Timers;

namespace Game.Custom.ObjectComponents;

public class Behavior(int type, TimeSpan elapsed = default)
{
    public int Type { get; set; } = type;
    public TimeSpan Elapsed { get; set; } = elapsed;
}
