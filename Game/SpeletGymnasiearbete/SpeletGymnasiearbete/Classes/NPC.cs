using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpeletGymnasiearbete.Classes


{
    public class randomMovingEntity
    {
        protected float speed;            // Speed of the entity
        protected float moveInterval;     // Time interval to change direction
        protected float timer;            // Tracks time elapsed
        public Vector2 position;
        private Random random = new Random();
        public Vector2 direction;
        public AnimatedSprite sprite;     // AnimatedSprite for the entity
        public float timeInterval;

        public randomMovingEntity(Texture2D texture, Vector2 initialPosition)
        {
            position = initialPosition;
            speed = 1f;  // Default speed
            timeInterval = 1;
            moveInterval = GetRandomInterval(timeInterval);
            timer = 0f;
            ChooseRandomDirection();

            // Initialize the sprite (AnimatedSprite is used here)
            sprite = new AnimatedSprite(texture, position, 4, 200f, true); // 4 frames, 200ms per frame
        }

        public void Update(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (timer >= moveInterval)
            {
                ChooseRandomDirection();
                timer = 0f;
                moveInterval = GetRandomInterval(timeInterval);
            }

            // Update the position based on the direction and speed
            position += direction * speed;

            // Update the sprite (which handles animation)
            sprite.Position = position;  // Update the sprite's position
            sprite.Update(gameTime);      // Call the sprite's update method
        }

        protected void ChooseRandomDirection()
        {
            // Pick a random angle and convert it to a direction vector
            float angle = (float)(random.NextDouble() * MathHelper.TwoPi);
            direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        protected float GetRandomInterval(float timeInterval)
        {
            return 0.01f + (float)(random.NextDouble() * timeInterval); // Random interval between 1 and 3 seconds
        }

        public virtual void Draw()
        {
            sprite.Draw();  // Call the Draw method on the sprite
        }
    }

public class Slime : randomMovingEntity
{
    public Slime(Texture2D texture, Vector2 initialPosition) : base(texture, initialPosition)
    {
        speed = 1f;  // Set the slime's speed (slower than default)
        timeInterval = 0.5f;
        // You can also add any specific behavior for the slime if needed
    }

    // Override Update if you want to add additional slime-specific logic
    public new void Update(GameTime gameTime)
    {
        base.Update(gameTime);  // Call base class Update (handles movement, random direction, etc.)
        // Slime-specific behavior can be added here (e.g., reaction to nearby entities)
    }

  
}
}
