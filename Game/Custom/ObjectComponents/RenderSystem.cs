using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;

namespace Game.Custom.ObjectComponents
{
    public class RenderSystem : EntityDrawSystem
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly SpriteBatch _spriteBatch;

        private ComponentMapper<TransformComponent> _transformMapper;
        private ComponentMapper<SpriteComponent> _spriteMapper;

        public RenderSystem(GraphicsDevice graphicsDevice)
            : base(Aspect.All(typeof(TransformComponent), typeof(SpriteComponent))) // Entities must have both Transform2 and Sprite components
        {
            _graphicsDevice = graphicsDevice;
            _spriteBatch = new SpriteBatch(graphicsDevice);
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _transformMapper = mapperService.GetMapper<TransformComponent>();
            _spriteMapper = mapperService.GetMapper<SpriteComponent>(); // Mapper for Sprite component
        }

        public override void Draw(GameTime gameTime)
        {
            _graphicsDevice.Clear(Color.CornflowerBlue); // Set the background color
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            foreach (var entity in ActiveEntities)
            {
                var transform = _transformMapper.Get(entity);
                var sprite = _spriteMapper.Get(entity);

                // Draw each entity using its sprite and transform
                _spriteBatch.Draw(
                    sprite.Texture,               // The sprite's texture
                    transform.Position,           // Position to draw the sprite at
                    null,                         // Source rectangle (null uses the whole texture)
                    Color.White,                  // Tint color
                    0f,                           // Rotation (if any)
                    new Vector2(sprite.Texture.Width / 2f, sprite.Texture.Height / 2f), // Origin (center of the texture)
                    transform.Scale,              // Scale factor
                    SpriteEffects.None,           // Effects (e.g., flipping)
                    0f                            // Layer depth
                );
            }

            _spriteBatch.End();
        }
    }
}
