using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Graphics;

namespace Game.Custom.Components.Systems;

public class RenderSystem() : EntityDrawSystem(Aspect.All(typeof(Transform2)).One(typeof(SpriteComponent), typeof(AnimatedSprite)))
{
    private ComponentMapper<Transform2> _transformMapper;
    private ComponentMapper<SpriteComponent> _spriteMapper;
    private ComponentMapper<AnimatedSprite> _animatedSpriteMapper;
    private ComponentMapper<Item> _itemMapper;
    private ComponentMapper<Equipable> _equipableMapper;

    public override void Initialize(IComponentMapperService mapperService)
    {
        // Map to all components for drawing sprites and animatedSprites
        _transformMapper = mapperService.GetMapper<Transform2>();
        _spriteMapper = mapperService.GetMapper<SpriteComponent>();
        _animatedSpriteMapper = mapperService.GetMapper<AnimatedSprite>();
        _equipableMapper = mapperService.GetMapper<Equipable>();
        _itemMapper = mapperService.GetMapper<Item>();
    }

    public override void Draw(GameTime gameTime)
    {
        foreach (var entity in ActiveEntities)
        {
            // Don't draw items that are inside an inventory
            if (_itemMapper.Has(entity) && _itemMapper.Get(entity).InInventory)
                continue;

            var offset = new Transform2();

            if (_equipableMapper.Has(entity))
            {
                var eq = _equipableMapper.Get(entity);
                if (eq.Parent is not null && eq.Parent.Has<Transform2>())
                {
                    offset = eq.Parent.Get<Transform2>();
                }
            }

            var transform = _transformMapper.Get(entity);
            
            if (_spriteMapper.Has(entity))
            {
                var sprite = _spriteMapper.Get(entity);

                // Draw each entity using its sprite and transform
                Global.SpriteBatch.Draw(
                    sprite.Texture,
                    transform.Position + offset.Position,
                    sprite.SourceRectangle,                 // Source rectangle (null uses the whole texture)
                    Color.White,                            // Tint color
                    transform.Rotation + offset.Rotation,   // Rotation
                    new Vector2(
                        sprite.Texture.Width / 2f,
                        sprite.Texture.Height / 2f
                    ),                                      // Origin (center of the texture)
                    transform.Scale * offset.Scale,         // Scale factor
                    SpriteEffects.None,                     // Effects
                    0f                                      // Layer depth
                );
            }
            else if (_animatedSpriteMapper.Has(entity))
            {
                var animsprite = _animatedSpriteMapper.Get(entity);
                animsprite.Update(gameTime);
                Global.SpriteBatch.Draw(animsprite, transform.Position + offset.Position, transform.Rotation + offset.Rotation, transform.Scale * offset.Scale);
            }
        }
    }
}
