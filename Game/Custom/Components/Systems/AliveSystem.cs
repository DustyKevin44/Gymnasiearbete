using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Graphics;
using System;

namespace Game.Custom.Components.Systems;

public class AliveSystem : EntityUpdateSystem
{
    private ComponentMapper<HealthComponent> _healthMapper;
    private ComponentMapper<AnimatedSprite> _animatedSpriteMapper;

    public AliveSystem() : base(Aspect.All(typeof(Transform2), typeof(VelocityComponent), typeof(Behavior), typeof(AnimatedSprite))) { }

    public override void Initialize(IComponentMapperService mapperService)
    {
        _healthMapper = mapperService.GetMapper<HealthComponent>();
        _animatedSpriteMapper = mapperService.GetMapper<AnimatedSprite>();
    }

    public override void Update(GameTime gameTime)
    {
        foreach (var entity in ActiveEntities)
        {
            var health = _healthMapper.Get(entity);

            if(health.IsAlive == false){
                //Kill the entity and remove it from the game as well as 
            }
            if(_animatedSpriteMapper.Has(entity)){
                AnimatedSprite animation = _animatedSpriteMapper.Get(entity);
                try{
                    //Console.WriteLine("Death animation");
                    //animation.SetAnimation("Death");
                }catch{
                    Console.WriteLine("No death animation");
                    // fade out
                     // Fade out effect (lerp color to black)
                    Color targetColor = Color.Black;
                    float lerpSpeed = 0.01f; // Adjust speed of transition

                    if (animation.Color != targetColor)
                    {
                        Console.WriteLine("Dead");
                        animation.Color = Color.Lerp(animation.Color, targetColor, lerpSpeed);
                    }
                } 
            }
           
        }
    }
}
