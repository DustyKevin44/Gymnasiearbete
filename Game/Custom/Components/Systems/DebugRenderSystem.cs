using Game;
using Game.Custom.Components;
using Game.Custom.Utilities;
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
    private ComponentMapper<Equipable> _equipableMapper;

    public override void Initialize(IComponentMapperService mapperService)
    {
        _transformMapper = mapperService.GetMapper<Transform2>();
        _collisionMapper = mapperService.GetMapper<CollisionBox>();
        _hurtboxMapper = mapperService.GetMapper<HurtBox>();
        _hitboxMapper = mapperService.GetMapper<HitBox>();
        _equipableMapper = mapperService.GetMapper<Equipable>();
    }

    public override void Draw(GameTime gameTime)
    {
        foreach (var entity in ActiveEntities)
        {
            var transform = _transformMapper.Get(entity);
            Vector2 offset = Vector2.Zero;
            
            // Equipable components are relative to parent
            if (Utils.TryGet(_equipableMapper, entity, out Equipable eq) &&
                Utils.TryGet(eq.Parent, out Transform2 t))
            {
                offset += t.Position;
            }

            if (_collisionMapper.Has(entity))
            {
                var collisionbox = _collisionMapper.Get(entity);
                if (collisionbox.Shape is RectangleF && collisionbox.IsEnabled)
                    Global.SpriteBatch.DrawRectangle(collisionbox.Bounds.Position + transform.Position + offset, collisionbox.Bounds.BoundingRectangle.Size, Color.Black, 2f);
            }

            if (_hurtboxMapper.Has(entity))
            {
                var hurtbox = _hurtboxMapper.Get(entity);
                if (hurtbox.Shape is RectangleF && hurtbox.IsEnabled)
                    Global.SpriteBatch.DrawRectangle(hurtbox.Bounds.Position + transform.Position + offset, hurtbox.Bounds.BoundingRectangle.Size, Color.Green, 2f);
            }

            if (_hitboxMapper.Has(entity))
            {
                var hitbox = _hitboxMapper.Get(entity);
                if (hitbox.Shape is RectangleF && hitbox.IsEnabled)
                    Global.SpriteBatch.DrawRectangle(hitbox.Bounds.Position + transform.Position + offset, hitbox.Bounds.BoundingRectangle.Size, Color.Red, 2f);
            }
        }
    }
}