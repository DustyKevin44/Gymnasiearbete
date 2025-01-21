namespace Game.Custom.ObjectComponents
{
    public class HealthComponent
    {
        private int _maxHealth { get; set; }
        private int health;
        public int Health { get => health; set => health = (value > _maxHealth) ? _maxHealth : value; }

        public bool IsAlive()
        {
            return Health > 0;
        }
    }
} 
