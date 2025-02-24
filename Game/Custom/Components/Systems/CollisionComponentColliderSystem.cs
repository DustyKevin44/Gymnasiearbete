using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;

namespace Game.Custom.Components.Systems;


public class ColliderSystems : EntityUpdateSystem
{
    private ComponentMapper<HurtBox> _hurtBoxMapper;
    private ComponentMapper<HitBox> _hitBoxMapper;
    private ComponentMapper<CollisionBox> _collisionBoxMapper;
    private ComponentMapper<Transform2> _transformMapper;

    public ColliderSystems() : base(Aspect.All(typeof(Transform2)).One(typeof(HurtBox), typeof(HitBox), typeof(CollisionBox))) { }

    public override void Initialize(IComponentMapperService mapperService)
    {
        _hitBoxMapper = mapperService.GetMapper<HitBox>();
        _hurtBoxMapper = mapperService.GetMapper<HurtBox>();
        _collisionBoxMapper = mapperService.GetMapper<CollisionBox>();
        _transformMapper = mapperService.GetMapper<Transform2>();
    }

    public override void Update(GameTime gameTime)
    {
        
    }
    
}