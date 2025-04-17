
namespace Game.Custom.Components;

public class HealthComponent(float health, float maxHealth)
{
    private float _health = health;
    public float MaxHealth = maxHealth;

    public float Health { get => _health; set => _health = (value > MaxHealth) ? MaxHealth : value; }
    public bool IsAlive => _health > 0;
}

