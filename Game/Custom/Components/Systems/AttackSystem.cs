using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using Game.Custom.Components;

public class AttackSystem : EntityUpdateSystem
{
    private ComponentMapper<MeleeAttack> _meleeMapper;


    public AttackSystem()
        : base(Aspect.All(typeof(MeleeAttack)))
    {
    }

    public override void Initialize(IComponentMapperService mapperService)
    {
        _meleeMapper = mapperService.GetMapper<MeleeAttack>();
    }

    public override void Update(GameTime gameTime)
    {
        
    }
}
