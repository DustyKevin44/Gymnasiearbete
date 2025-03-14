using Microsoft.Xna.Framework;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Tweening;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.ECS;

namespace Game.Custom.Components.Systems;


public class AliveSystem : EntityUpdateSystem
{
    private ComponentMapper<HealthComponent> _healthMapper;
    private ComponentMapper<AnimatedSprite> _animatedSpriteMapper;
    private ComponentMapper<SpriteComponent> _spriteMapper;

    public AliveSystem() : base(Aspect.All(typeof(HealthComponent))) { }

    public override void Initialize(IComponentMapperService mapperService)
    {
        _healthMapper = mapperService.GetMapper<HealthComponent>();
        _animatedSpriteMapper = mapperService.GetMapper<AnimatedSprite>();
        _spriteMapper = mapperService.GetMapper<SpriteComponent>();
    }

    public override void Update(GameTime gameTime)
    {
        foreach (var entity in ActiveEntities)
        {
            var health = _healthMapper.Get(entity);

            if (!health.IsAlive)
            {
                Global.World.DestroyEntity(entity);
                if (_animatedSpriteMapper.Has(entity))
                {
                    AnimatedSprite animation = _animatedSpriteMapper.Get(entity);
                    try
                    {
                        animation.SetAnimation("Death");
                    }
                    catch
                    {
                        Entity entityObj = Global.World.GetEntity(entity);
                        if (entityObj.Has<Tweener>())
                        {
                            var tweener = entityObj.Get<Tweener>();
                            tweener.TweenTo(animation, (animation) => animation.Color, Color.Black, 2);
                        }
                        else
                        {
                            var tweener = new Tweener();
                            tweener.TweenTo(animation, (animation) => animation.Color, Color.Black, 2);
                            entityObj.Attach(tweener);
                        }
                    }
                }

                if (_spriteMapper.Has(entity))
                {
                    SpriteComponent sprite = _spriteMapper.Get(entity);
                    Entity entityObj = Global.World.GetEntity(entity);
                    if (entityObj.Has<Tweener>())
                    {
                        var tweener = entityObj.Get<Tweener>();
                        tweener.TweenTo(sprite, (sprite) => sprite.Color, Color.Black, 2);
                    }
                    else
                    {
                        var tweener = new Tweener();
                        tweener.TweenTo(sprite, (sprite) => sprite.Color, Color.Black, 2);
                        entityObj.Attach(tweener);
                    }
                }
            }


        }
    }
}
