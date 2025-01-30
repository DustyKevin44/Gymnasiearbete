
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
        private ComponentMapper<Transform2> _transformMapper;
        private ComponentMapper<VelocityComponent> _velocityMapper;

        public MovementSystem() 
            : base(Aspect.All(typeof(Transform2), typeof(VelocityComponent)))
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _transformMapper = mapperService.GetMapper<Transform2>();
            _velocityMapper = mapperService.GetMapper<VelocityComponent>();
        }

        public override void Update(GameTime gameTime)
        {
            //System.Console.WriteLine(ActiveEntities);
            foreach (var entity in ActiveEntities)
            {
                var transform = _transformMapper.Get(entity);
                var velocity = _velocityMapper.Get(entity);
                //System.Console.WriteLine(transform.Position);
                transform.Position += velocity.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds * 360;
                //System.Console.WriteLine(transform.Position);

            }
        }
    }
}