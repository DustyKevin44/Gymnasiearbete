using Microsoft.Xna.Framework;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Tweening;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.ECS;
using System.Threading;
using System;
using Game.Custom.Factories;

namespace Game.Custom.Components.Systems;


public class SpawnerSystem : EntityUpdateSystem
{
    private ComponentMapper<SpawnerComponent> _spawnerMapper;

    public SpawnerSystem() : base(Aspect.All(typeof(SpawnerComponent))) { }

    public override void Initialize(IComponentMapperService mapperService)
    {
        _spawnerMapper = mapperService.GetMapper<SpawnerComponent>();
    }

    public override void Update(GameTime gameTime)
    {
        foreach (var entity in ActiveEntities)
        {
            SpawnerComponent spawner = _spawnerMapper.Get(entity);
            if (spawner.Elapsed.Seconds >= spawner.NextSpawn)
            {
                if (spawner.Type.ToLower() == "slime")
                {
                    if (spawner.CooldownVariance.HasValue)
                    {
                        spawner.NextSpawn = spawner.Cooldown + Global.Random.NextSingle() * spawner.CooldownVariance.Value;
                    }
                    Vector2 spawnPosition = new(
                        Global.Random.Next((int)spawner.Position.X, (int)spawner.Position.X+(int)spawner.Size.X),
                        Global.Random.Next((int)spawner.Position.Y, (int)spawner.Position.Y+(int)spawner.Size.Y)
                    );
                    EntityFactory.CreateSlimeAt(spawnPosition, 100f);
                    spawner.Elapsed = TimeSpan.Zero;
                }
                
            }
            spawner.Elapsed += gameTime.ElapsedGameTime;
        }
    }
}
