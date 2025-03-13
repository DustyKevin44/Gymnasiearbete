using System;
using System.Runtime.InteropServices;
using Autofac;
using Microsoft.Xna.Framework;

namespace Game.Custom.Components;

public class SpawnerComponent(Vector2 position, Vector2 size, string type, float cooldown, float? cooldownVariance=null)
{
    public Vector2 Position = position; // Where the center of the spawner is.
    public Vector2 Size = size;
    public string Type = type; // What type the spawner will spawn. Each spawner entity only spawns one type.
    public float Cooldown = cooldown; // How often a entity will spawn in the spawner. 
    public float? CooldownVariance = cooldownVariance; // The variance in spawning on cooldown. Will increase or decrease the next cooldown.
    public float NextSpawn = cooldown;
    public TimeSpan Elapsed = TimeSpan.Zero; // Time elapsed since last spawn.
    
}
