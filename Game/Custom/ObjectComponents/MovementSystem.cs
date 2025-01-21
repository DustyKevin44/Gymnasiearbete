
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Custom.ObjectComponents
{
    public class MovementSystem : EntityUpdateSystem
    {
        private ComponentMapper<TransformComponent> _transformMapper;
        private ComponentMapper<VelocityComponent> _velocityMapper;

        public MovementSystem() 
            : base(Aspect.All(typeof(TransformComponent), typeof(VelocityComponent)))
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _transformMapper = mapperService.GetMapper<TransformComponent>();
            _velocityMapper = mapperService.GetMapper<VelocityComponent>();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var entity in ActiveEntities)
            {
                var transform = _transformMapper.Get(entity);
                var velocity = _velocityMapper.Get(entity);

                transform.Position += velocity.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }
    }
}