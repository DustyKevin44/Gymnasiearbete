
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
    public class BehaviorSystem : EntityUpdateSystem
    {
        private ComponentMapper<Transform2> _transformMapper;
        private ComponentMapper<VelocityComponent> _velocityMapper;
        private ComponentMapper<Behavior> _behaviorMapper;

        public BehaviorSystem() 
            : base(Aspect.All(typeof(Transform2), typeof(VelocityComponent), typeof(Behavior)))
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _transformMapper = mapperService.GetMapper<Transform2>();
            _velocityMapper = mapperService.GetMapper<VelocityComponent>();
            _behaviorMapper = mapperService.GetMapper<Behavior>();
        }

        public override void Update(GameTime gameTime)
        {
            //System.Console.WriteLine(ActiveEntities.Count);
            foreach (var entity in ActiveEntities)
            {
                var transform = _transformMapper.Get(entity);
                var velocity = _velocityMapper.Get(entity);
                var behavior = _behaviorMapper.Get(entity);
                behavior.Elapsed += gameTime.ElapsedGameTime;
                if (behavior.Elapsed.TotalSeconds > 1f){
                    if(behavior.Type == 0)
                    {
                        velocity.Velocity += new Vector2(60, 60);
                        behavior.Type = 1;
                        behavior.Elapsed = TimeSpan.Zero;
                    }else
                    {
                        velocity.Velocity -= new Vector2(60, 60);
                        behavior.Type = 0;
                        behavior.Elapsed = TimeSpan.Zero;
                    }
                }
            
                //System.Console.WriteLine(transform.Position);
                //System.Console.WriteLine(transform.Position);

            }
        }
    }
}