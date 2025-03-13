using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Graphics;

namespace Game.Custom.Components.Systems;

public class RenderSystem(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
    : EntityDrawSystem(Aspect.All(typeof(Transform2)).One(typeof(SpriteComponent), typeof(AnimatedSprite)))
{
    private readonly GraphicsDevice _graphicsDevice = graphicsDevice;
    private readonly SpriteBatch _spriteBatch = spriteBatch;

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
            if (_itemMapper.Has(entity) && _itemMapper.Get(entity).InInventory) continue;

            Vector2 localPosition = Vector2.Zero;
            float localRotation = 0f;
            Vector2 localScale = Vector2.One;

            if (_equipableMapper.Has(entity))
            {
                var eq = _equipableMapper.Get(entity);
                localPosition += eq.LocalTransform.Position;
                localRotation += eq.LocalTransform.Rotation;
                localScale *= eq.LocalTransform.Scale;
            }

            var transform = _transformMapper.Get(entity);
            var sprite = _spriteMapper.Get(entity);
            
            if (_spriteMapper.Has(entity))
            {
                // Draw each entity using its sprite and transform
                _spriteBatch.Draw(
                    sprite.Texture,
                    transform.Position + localPosition,
                    null,                                   // Source rectangle (null uses the whole texture)
                    Color.White,                            // Tint color
                    localRotation,                          // Rotation
                    new Vector2(
                        sprite.Texture.Width / 2f,
                        sprite.Texture.Height / 2f
                    ),                                      // Origin (center of the texture)
                    transform.Scale * localScale,           // Scale factor
                    SpriteEffects.None,                     // Effects
                    0f                                      // Layer depth
                );
            }
            else if (_animatedSpriteMapper.Has(entity))
            {
                var animsprite = _animatedSpriteMapper.Get(entity);
                animsprite.Update(gameTime);
                _spriteBatch.Draw(animsprite, transform.Position + localPosition, localRotation, localScale);
            }
        }
    }
}
