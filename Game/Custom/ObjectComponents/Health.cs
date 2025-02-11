namespace Game.Custom.ObjectComponents
{
    public class HealthComponent
    {
        private int _health;
        public int MaxHealth;

        public int Health { get => _health; set => _health = (value > MaxHealth) ? MaxHealth : value; }
        public bool IsAlive => _health > 0;
    }
} 
