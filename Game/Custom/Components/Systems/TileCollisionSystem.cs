using Game.Custom.Components;
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
    private ComponentMapper<ColliderComponent> _colliderMapper;
    private HashSet<Point> _solidTiles;
    private int tileSize = 32; // Adjust based on your tile size

    public TileCollisionSystem(HashSet<Point> solidTiles)
        : base(Aspect.All(typeof(Transform2), typeof(VelocityComponent), typeof(ColliderComponent)))
    {
        _solidTiles = solidTiles ?? throw new ArgumentNullException(nameof(solidTiles));
    }

    public override void Initialize(IComponentMapperService mapperService)
    {
        _transformMapper = mapperService.GetMapper<Transform2>();
        _velocityMapper = mapperService.GetMapper<VelocityComponent>();
        _colliderMapper = mapperService.GetMapper<ColliderComponent>();
    }

    public override void Update(GameTime gameTime)
    {
        foreach (var entity in ActiveEntities)
        {
            var transform = _transformMapper.Get(entity);
            var velocity = _velocityMapper.Get(entity);
            var collider = _colliderMapper.Get(entity);

            Vector2 newPosition = transform.Position + velocity.Velocity * gameTime.GetElapsedSeconds();
            RectangleF newBounds = new RectangleF(newPosition, collider.Bounds.Size);

            if (!IsCollidingWithTile(newBounds))
            {
                transform.Position = newPosition; // Apply movement if no collision
            }
            else
            {
                velocity.Velocity = Vector2.Zero; // Stop movement if collision
            }
        }
    }

    private bool IsCollidingWithTile(RectangleF bounds)
    {
        int leftTile = (int)(bounds.Left / tileSize);
        int rightTile = (int)(bounds.Right / tileSize);
        int topTile = (int)(bounds.Top / tileSize);
        int bottomTile = (int)(bounds.Bottom / tileSize);
        //Console.WriteLine("Tries" + leftTile + rightTile + topTile + bottomTile);
        Console.WriteLine($"Checking collision from ({leftTile}, {topTile}) to ({rightTile}, {bottomTile})");
        for (int x = leftTile; x <= rightTile; x++)
        {
            for (int y = topTile; y <= bottomTile; y++)
            {
                Console.WriteLine($"Checking Tile: {x}, {y}");
                Console.WriteLine("Solid Tiles:");
                foreach (var tile in _solidTiles)
                {
                    Console.WriteLine($"Tile: {tile.X}, {tile.Y}");
                }
                if (_solidTiles.Contains(new Point(x, y)))
                {
                    Console.WriteLine("Collision detected!");
                    return true; // Collision detected
                }
            }
        }
        return false;
    }
}
