using Game.Custom.Components;
using Game.Custom.Components.Systems;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using System;
using System.Collections.Generic;

public class TileCollisionSystem : EntityUpdateSystem
{
    private ComponentMapper<Transform2> _transformMapper;
    private ComponentMapper<VelocityComponent> _velocityMapper;
    private ComponentMapper<CollisionBox> _colliderMapper;
    private HashSet<Point> _solidTiles;
    private int tileSize = 32; // Adjust based on your tile size

    public TileCollisionSystem(HashSet<Point> solidTiles)
        : base(Aspect.All(typeof(Transform2), typeof(VelocityComponent), typeof(CollisionBox)))
    {
        _solidTiles = solidTiles ?? throw new ArgumentNullException(nameof(solidTiles));
    }

    public override void Initialize(IComponentMapperService mapperService)
    {
        _transformMapper = mapperService.GetMapper<Transform2>();
        _velocityMapper = mapperService.GetMapper<VelocityComponent>();
        _colliderMapper = mapperService.GetMapper<CollisionBox>();
    }

    public override void Update(GameTime gameTime)
    {
        foreach (var entity in ActiveEntities)
        {
            var transform = _transformMapper.Get(entity);
            var velocity = _velocityMapper.Get(entity);
            var collider = _colliderMapper.Get(entity);

            Vector2 newPosition = transform.Position + velocity.Velocity * gameTime.GetElapsedSeconds();
            RectangleF newBounds = new RectangleF(newPosition, collider.Shape.BoundingRectangle.Size);

            string collisionInfo = GetNearbySolidTiles(transform.Position);
            //Console.WriteLine($"Collision! Solid tiles detected at: {collisionInfo}");

            if (!IsCollidingWithTile(newBounds))
            {
                transform.Position = newPosition; // Apply movement if no collision
            }
            else
            {
                velocity.Velocity = Vector2.Zero; // Stop movement if collision
                Console.WriteLine($"Collision! Solid tiles detected at: {collisionInfo}");
            }
        }
    }

    private bool IsCollidingWithTile(RectangleF bounds)
    {
        int leftTile = (int)(bounds.Left / tileSize);
        int rightTile = (int)(bounds.Right / tileSize);
        int topTile = (int)(bounds.Top / tileSize);
        int bottomTile = (int)(bounds.Bottom / tileSize);
        
        for (int x = leftTile; x <= rightTile; x++)
        {
            for (int y = topTile; y <= bottomTile; y++)
            {
                if (_solidTiles.Contains(new Point(x, y)))
                {
                    return true; // Collision detected
                }
            }
        }
        return false;
    }

    private string GetNearbySolidTiles(Vector2 position)
    {
        int centerTileX = (int)(position.X / tileSize);
        int centerTileY = (int)(position.Y / tileSize);

        string result = "";

        for (int offsetX = -1; offsetX <= 1; offsetX++)
        {
            for (int offsetY = -1; offsetY <= 1; offsetY++)
            {
                int tileX = centerTileX + offsetX;
                int tileY = centerTileY + offsetY;

                if (_solidTiles.Contains(new Point(tileX, tileY)))
                {
                    string direction = GetDirectionFromOffset(offsetX, offsetY);
                    if (!string.IsNullOrEmpty(direction))
                    {
                        if (result.Length > 0) result += ", ";
                        result += direction;
                    }
                }
            }
        }

        return string.IsNullOrEmpty(result) ? "No solid tiles nearby" : result;
    }

    private string GetDirectionFromOffset(int offsetX, int offsetY)
    {
        if (offsetX == 0 && offsetY == -1) return "North";
        if (offsetX == 0 && offsetY == 1) return "South";
        if (offsetX == -1 && offsetY == 0) return "West";
        if (offsetX == 1 && offsetY == 0) return "East";
        if (offsetX == -1 && offsetY == -1) return "North-West";
        if (offsetX == 1 && offsetY == -1) return "North-East";
        if (offsetX == -1 && offsetY == 1) return "South-West";
        if (offsetX == 1 && offsetY == 1) return "South-East";

        return "";
    }
}
