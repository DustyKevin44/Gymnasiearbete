
namespace Game.Custom.Components;

public class HealthComponent(int maxHealth)
{
    private float _health = maxHealth;
    public float MaxHealth = maxHealth;

    public float Health { get => _health; set => _health = (value > MaxHealth) ? MaxHealth : value; }
    public bool IsAlive => _health > 0;
}

