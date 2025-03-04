using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Tweening;
using MonoGame.Extended;
using Game.Custom.Components;
using System;

public class AttackSystem : EntityUpdateSystem
{
    private ComponentMapper<MeleeAttack> _meleeMapper;


    public AttackSystem()
        : base(Aspect.All(typeof(MeleeAttack)))
    {
    }

    public override void Initialize(IComponentMapperService mapperService)
    {
        _meleeMapper = mapperService.GetMapper<MeleeAttack>();
    }

    public override void Update(GameTime gameTime)
    {
        foreach (var entity in ActiveEntities)
        {
            var melee = _meleeMapper.Get(entity);
            if(melee.Cooldown == 0)
            {
                
            }

        }

    }
}
