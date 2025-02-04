using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Graphics;

namespace Game.Custom.ObjectComponents
{
    public class RenderSystem : EntityDrawSystem
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly SpriteBatch _spriteBatch;

        private ComponentMapper<Transform2> _transformMapper;
        private ComponentMapper<SpriteComponent> _spriteMapper;
        private ComponentMapper<AnimatedSprite> _animatedSpriteMapper;


        public RenderSystem(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
            : base(Aspect.All(typeof(Transform2)).One(typeof(SpriteComponent), typeof(AnimatedSprite))) // Entities must have both Transform2 and Sprite components
        {
            _graphicsDevice = graphicsDevice;
            _spriteBatch = spriteBatch;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _transformMapper = mapperService.GetMapper<Transform2>();
            _spriteMapper = mapperService.GetMapper<SpriteComponent>(); // Mapper for Sprite component
            _animatedSpriteMapper = mapperService.GetMapper<AnimatedSprite>(); // Mapper for Sprite component

        }

        public override void Draw(GameTime gameTime)
        {
            //_spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            foreach (var entity in ActiveEntities)
            {

                var transform = _transformMapper.Get(entity);
                var sprite = _spriteMapper.Get(entity);
                if (_spriteMapper.Has(entity))
                {
                    //System.Console.WriteLine("transform"+transform + " sprite:" + sprite.Texture);
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
                else if (_animatedSpriteMapper.Has(entity) != false)
                {
                    var animsprite = _animatedSpriteMapper.Get(entity);
                    _spriteBatch.Draw(animsprite, transform.Position, 0);

                }


            }

            //_spriteBatch.End();
        }
    }
}
