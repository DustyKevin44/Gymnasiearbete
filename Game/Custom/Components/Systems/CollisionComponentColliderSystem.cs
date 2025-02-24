using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;

namespace Game.Custom.Components.Systems;


[Flags]
public enum Layers
{
    Default = 1 << 0,
    Player = 1 << 1,
    Enemy = 1 << 2,
    Tile = 1 << 3,
}


public class ColliderSystems : EntityUpdateSystem
{
    private ComponentMapper<HurtBox<Layer>> _hurtBoxMapper;
    private ComponentMapper<HitBox<Layer>> _hitBoxMapper;
    private ComponentMapper<CollisionBox<Layer>> _collisionBoxMapper;
    private ComponentMapper<Transform2> _transformMapper;

    public ColliderSystems() : base(Aspect.All(typeof(Transform2)).One(typeof(HurtBox<Layer>), typeof(HitBox<Layer>), typeof(CollisionBox<Layer>))) { }

    public override void Initialize(IComponentMapperService mapperService)
    {
        _hitBoxMapper = mapperService.GetMapper<HitBox<Layer>>();
        _hurtBoxMapper = mapperService.GetMapper<HurtBox<Layer>>();
        _collisionBoxMapper = mapperService.GetMapper<CollisionBox<Layer>>();
        _transformMapper = mapperService.GetMapper<Transform2>();
    }

    public override void Update(GameTime gameTime)
    {
        
    }
    
}