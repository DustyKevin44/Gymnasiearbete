using Game;
using Game.Custom.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;

public class DebugRenderSystem() : EntityDrawSystem(Aspect.All(typeof(Transform2)))
{
    private ComponentMapper<Transform2> _transformMapper;
    private ComponentMapper<CollisionBox> _collisionMapper;
    private ComponentMapper<HurtBox> _hurtboxMapper;
    private ComponentMapper<HitBox> _hitboxMapper;

    public override void Initialize(IComponentMapperService mapperService)
    {
        _transformMapper = mapperService.GetMapper<Transform2>();
        _collisionMapper = mapperService.GetMapper<CollisionBox>();
        _hurtboxMapper = mapperService.GetMapper<HurtBox>();
        _hitboxMapper = mapperService.GetMapper<HitBox>();
    }

    public override void Draw(GameTime gameTime)
    {
        foreach (var entity in ActiveEntities)
        {
            var transform = _transformMapper.Get(entity);

            if (_collisionMapper.Has(entity))
            {
                var collisionbox = _collisionMapper.Get(entity);
                if (collisionbox.Shape is RectangleF)
                    Global.SpriteBatch.DrawRectangle(collisionbox.Bounds.Position + transform.Position, collisionbox.Bounds.BoundingRectangle.Size, Color.Black, 2f);
            }

            if (_hurtboxMapper.Has(entity))
            {
                var hurtbox = _hurtboxMapper.Get(entity);
                if (hurtbox.Shape is RectangleF)
                    Global.SpriteBatch.DrawRectangle(hurtbox.Bounds.Position + transform.Position, hurtbox.Bounds.BoundingRectangle.Size, Color.Green, 2f);
            }

            if (_hitboxMapper.Has(entity))
            {
                var hitbox = _hitboxMapper.Get(entity);
                if (hitbox.Shape is RectangleF)
                    Global.SpriteBatch.DrawRectangle(hitbox.Bounds.Position + transform.Position, hitbox.Bounds.BoundingRectangle.Size, Color.Red, 2f);
            }
        }
    }
}