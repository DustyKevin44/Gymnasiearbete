
namespace Game.Custom.Components;

public class HealthComponent(int maxHealth)
{
    private int _health = maxHealth;
    public int MaxHealth = maxHealth;

    public int Health { get => _health; set => _health = (value > MaxHealth) ? MaxHealth : value; }
    public bool IsAlive => _health > 0;
}

