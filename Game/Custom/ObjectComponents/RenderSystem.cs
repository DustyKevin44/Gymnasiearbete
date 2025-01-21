
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Custom.ObjectComponents;

public class RenderSystem : EntityDrawSystem
{
    private SpriteBatch _spriteBatch;
    private ComponentMapper<TransformComponent> _transformMapper;
    private ComponentMapper<SpriteComponent> _spriteMapper;

    public RenderSystem(SpriteBatch spriteBatch)
        : base(Aspect.All(typeof(TransformComponent), typeof(SpriteComponent)))
    {
        _spriteBatch = spriteBatch;
    }

    public override void Initialize(IComponentMapperService mapperService)
    {
        _transformMapper = mapperService.GetMapper<TransformComponent>();
        _spriteMapper = mapperService.GetMapper<SpriteComponent>();
    }

    public override void Draw(GameTime gameTime)
    {
        _spriteBatch.Begin();

        foreach (var entity in ActiveEntities)
        {
            var transform = _transformMapper.Get(entity);
            var sprite = _spriteMapper.Get(entity);

            _spriteBatch.Draw(sprite.Texture, transform.Position, Color.White);
        }

        _spriteBatch.End();
    }
}
