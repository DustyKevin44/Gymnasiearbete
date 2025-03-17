using System.Linq;
using Game.Custom.Input;
using Game.Custom.Static;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;

namespace Game.Custom.Components.Systems;

public enum StdActions {
    MOVE_LEFT,
    MOVE_RIGHT,
    MOVE_UP,
    MOVE_DOWN,
    DASH,
    CUSTOM,
    CUSTOM2,
    MENU,
    MainAttack
}

public class PlayerSystem : EntityUpdateSystem
{
    private ComponentMapper<PlayerComponent<StdActions>> _playerMapper;
    private ComponentMapper<VelocityComponent> _velocityMapper;
    private ComponentMapper<Equipment> _equipmentMapper;
    private ComponentMapper<Transform2> _transformMapper;

    public PlayerSystem() : base(Aspect.All(typeof(PlayerComponent<StdActions>), typeof(VelocityComponent))) { }

    public override void Initialize(IComponentMapperService mapperService)
    {
        _playerMapper = mapperService.GetMapper<PlayerComponent<StdActions>>();
        _velocityMapper = mapperService.GetMapper<VelocityComponent>();
        _equipmentMapper = mapperService.GetMapper<Equipment>();
        _transformMapper = mapperService.GetMapper<Transform2>();
    }

    public override void Update(GameTime gameTime)
    {
        int host = ActiveEntities.FirstOrDefault(-1);
        if (host != -1 && _transformMapper.Has(host))
        {
            Global.Camera.LookAt(_transformMapper.Get(host).Position);
        }

        foreach (int entity in ActiveEntities)
        {
            var player = _playerMapper.Get(entity);
            var velocity = _velocityMapper.Get(entity);

            player.DashTimer.Update(gameTime);

            // Player logic

            if (!player.IsInControl)
            {
                // Temporary fix to prevent friction in Movement System
                velocity.Velocity = player.Direction * 400f;
                continue;
            }

            Vector2 direction = InputManager.GetDirection(
                player.GetKey(StdActions.MOVE_UP),
                player.GetKey(StdActions.MOVE_DOWN),
                player.GetKey(StdActions.MOVE_LEFT),
                player.GetKey(StdActions.MOVE_RIGHT)
            );
            if (direction != Vector2.Zero)
                direction.Normalize();

            player.Direction = direction;

            velocity.Velocity += direction * 1000f * gameTime.GetElapsedSeconds();

            if (velocity.Velocity.X < 0.01 && velocity.Velocity.X > -0.01)
                velocity.Velocity.X = 0f;

            if (velocity.Velocity.Y < 0.01 && velocity.Velocity.Y > -0.01)
                velocity.Velocity.Y = 0f;

            if (player.IsActionJustPressed(StdActions.DASH) && velocity.Velocity.LengthSquared() > 0.01f)
            {
                player.IsInControl = false;
                player.DashTimer.Restart();
            }

            if (_equipmentMapper.Has(entity))
            {
                var equipment = _equipmentMapper.Get(entity);
                if (player.IsActionJustPressed(StdActions.MainAttack) && equipment.TryGet("hand", out Entity Weapon))
                {
                    if (Weapon.Has<MeleeAttack>())
                    {
                        var melee = Weapon.Get<MeleeAttack>();
                        if (melee.IsOffCooldown(gameTime))
                            Melee.Attack(Weapon);
                    }

                    if (Weapon.Has<RangedAttack>())
                    {
                        var ranged = Weapon.Get<RangedAttack>();
                        if (ranged.IsOffCooldown(gameTime))
                            Ranged.Attack(ranged);
                    }
                }
            }

            foreach ((StdActions action, CustomKeybind keybind) in player.Keybinds.Where(kv => kv.Value is CustomKeybind customKeybind && player.IsActionPressed(kv.Key)).Select(kv => (kv.Key, (CustomKeybind)kv.Value)))
                keybind.Action.Invoke(gameTime);
        }
    }
}