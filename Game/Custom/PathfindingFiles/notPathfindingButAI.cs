using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended; // For Transform2
// using MonoGame.Extended.Tiled; // If you need to work with Tiled maps

namespace Game.Custom.PathfindingFiles
{
    /// <summary>
    /// A very simple enemy that “sees” the player and moves toward them when in line-of-sight.
    /// If the enemy is blocked by an obstacle it will try to slide left/right.
    /// </summary>
    public class Enemy
    {
        // Using MonoGame.Extended’s Transform2 for position, rotation, and scale.
        public Transform2 Transform { get; set; }
        
        // Set a default speed (pixels per second)
        public float Speed { get; set; } = 100f;
        
        // For simplicity we assume the enemy is 32x32 pixels.
        public int Width { get; set; } = 32;
        public int Height { get; set; } = 32;
        
        // State for avoiding obstacles
        private bool _isAvoiding = false;
        private int _avoidDirection = 0; // -1 for left, 1 for right
        
        public Enemy(Vector2 startPosition)
        {
            Transform = new Transform2(startPosition);
        }
        
        // Returns the enemy's current bounding box.
        public Rectangle BoundingBox => new Rectangle(
            (int)Transform.Position.X, 
            (int)Transform.Position.Y, 
            Width, 
            Height);
        
        /// <summary>
        /// Update the enemy’s behavior.
        /// </summary>
        /// <param name="gameTime">Game time</param>
        /// <param name="player">The player reference</param>
        /// <param name="obstacles">A list of obstacles (as Rectangles) that block vision and movement</param>
        public void Update(GameTime gameTime, Player player, List<Rectangle> obstacles)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            // Check if we have an unobstructed view of the player
            if (CanSeePlayer(player, obstacles))
            {
                // If we can see the player, reset avoidance and move directly toward them.
                _isAvoiding = false;
                
                // Compute a normalized direction vector toward the player.
                Vector2 direction = player.Transform.Position - Transform.Position;
                if (direction != Vector2.Zero)
                    direction.Normalize();
                
                Transform.Position += direction * Speed * dt;
            }
            else
            {
                // If we cannot see the player, check for collisions.
                bool colliding = obstacles.Any(obs => obs.Intersects(BoundingBox));
                
                if (colliding)
                {
                    // If we’re colliding with something, choose an avoidance direction if not already set.
                    if (!_isAvoiding)
                    {
                        // Here you might choose a direction based on additional logic.
                        _avoidDirection = 1; // or -1 for left
                        _isAvoiding = true;
                    }
                    // Move horizontally to try to get around the obstacle.
                    Transform.Position += new Vector2(_avoidDirection * Speed * dt, 0);
                }
                else
                {
                    // Not colliding but still no clear view.
                    // As a fallback, adjust horizontally to try to align with the player.
                    if (Math.Abs(player.Transform.Position.X - Transform.Position.X) > 1)
                    {
                        _avoidDirection = player.Transform.Position.X > Transform.Position.X ? 1 : -1;
                        Transform.Position += new Vector2(_avoidDirection * Speed * dt, 0);
                    }
                }
            }
        }
        
        /// <summary>
        /// Checks if there is a clear line-of-sight between the enemy and the player.
        /// </summary>
        /// <param name="player">The player</param>
        /// <param name="obstacles">The list of obstacles</param>
        /// <returns>True if the enemy “sees” the player; false otherwise</returns>
        private bool CanSeePlayer(Player player, List<Rectangle> obstacles)
        {
            // Get the center of the enemy and the player.
            Vector2 enemyCenter = new Vector2(Transform.Position.X + Width / 2f, Transform.Position.Y + Height / 2f);
            Vector2 playerCenter = new Vector2(
                player.Transform.Position.X + player.Width / 2f, 
                player.Transform.Position.Y + player.Height / 2f);
            
            // Check if the line from enemyCenter to playerCenter intersects any obstacles.
            foreach (var obs in obstacles)
            {
                if (LineIntersectsRectangle(enemyCenter, playerCenter, obs))
                    return false;
            }
            return true;
        }
        
        /// <summary>
        /// Checks if a line segment (p1 to p2) intersects a rectangle.
        /// </summary>
        private bool LineIntersectsRectangle(Vector2 p1, Vector2 p2, Rectangle rect)
        {
            // Define the four corners of the rectangle.
            Vector2 topLeft = new Vector2(rect.Left, rect.Top);
            Vector2 topRight = new Vector2(rect.Right, rect.Top);
            Vector2 bottomLeft = new Vector2(rect.Left, rect.Bottom);
            Vector2 bottomRight = new Vector2(rect.Right, rect.Bottom);
            
            // Check each edge of the rectangle for intersection with our line.
            if (LinesIntersect(p1, p2, topLeft, topRight)) return true;
            if (LinesIntersect(p1, p2, topRight, bottomRight)) return true;
            if (LinesIntersect(p1, p2, bottomRight, bottomLeft)) return true;
            if (LinesIntersect(p1, p2, bottomLeft, topLeft)) return true;
            
            return false;
        }
        
        /// <summary>
        /// Determines if two line segments (p1-p2 and p3-p4) intersect.
        /// </summary>
        private bool LinesIntersect(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
        {
            float denominator = ((p4.Y - p3.Y) * (p2.X - p1.X)) - ((p4.X - p3.X) * (p2.Y - p1.Y));
            if (denominator == 0)
                return false; // The lines are parallel.
            
            float ua = (((p4.X - p3.X) * (p1.Y - p3.Y)) - ((p4.Y - p3.Y) * (p1.X - p3.X))) / denominator;
            float ub = (((p2.X - p1.X) * (p1.Y - p3.Y)) - ((p2.Y - p1.Y) * (p1.X - p3.X))) / denominator;
            
            // If ua and ub are between 0 and 1, the line segments intersect.
            return (ua >= 0 && ua <= 1 && ub >= 0 && ub <= 1);
        }
    }

    /// <summary>
    /// A simple player class.
    /// </summary>
    public class Player
    {
        public Transform2 Transform { get; set; }
        public int Width { get; set; } = 32;
        public int Height { get; set; } = 32;
        
        public Player(Vector2 startPosition)
        {
            Transform = new Transform2(startPosition);
        }
        
        public Rectangle BoundingBox => new Rectangle(
            (int)Transform.Position.X, 
            (int)Transform.Position.Y, 
            Width, 
            Height);
    }
}
