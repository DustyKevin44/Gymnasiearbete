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


    public MovementSystem()
        : base(Aspect.All(typeof(_meleeMapper)))
    {
    }

    public override void Initialize(IComponentMapperService mapperService)
    {
        _meleeMapper = mapperService.GetMapper<_meleeMapper>();
    }

    public override void Update(GameTime gameTime)
    {
        foreach (var entity in ActiveEntities)
        {
            var melee = _meleeMapper.Get(entity);
            if(melee. > 0)
            {
                
            }

        }

    }
}
