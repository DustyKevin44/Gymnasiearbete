using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static SpeletGymnasiearbete.Utils;

namespace SpeletGymnasiearbete.Classes.Tilemap;


public class Tile(Point position)
{
    public readonly Point Position = position;
}

public interface ITileLayer
{
    public void Draw(Vector2 offset);
}


public class TileLayer(Point face_size, Point tile_size, Point group_size, Point position, Texture2D tile_set) : Tile(position), ITileLayer
{
    private Point _face_size = face_size;
    private Point _tile_size = tile_size;
    
    public Vector2 Face_size { get => _face_size.ToVector2() * scale; }
    public Vector2 Tile_size { get => _tile_size.ToVector2() * scale; }
    private Vector2 FaceOver2 { get => _face_size.ToVector2() * scale / 2f; }
    
    public readonly Point map_size = group_size;
    public readonly Texture2D tile_set = tile_set;

    
    readonly Tile[,] _tiles = new Tile[group_size.X, group_size.Y];

    public float rotation = 0f;
    public float depth = 0f;
    public Vector2 scale = Vector2.One;
    public SpriteEffects effects = SpriteEffects.None;

    public void Draw(Vector2 offset)
    {
        for (int y = 0; y < map_size.Y; y++)
        {
            for (int x = 0; x < map_size.X; x++)
            {
                Vector2 position = offset + IsoToWorld(new Vector2(x, y));
                
                if (_tiles[x,y] is ITileLayer tile) tile.Draw(position);
                else
                    Globals.SpriteBatch.Draw(tile_set, position - Globals.Active_Camera.Position, new Rectangle((_tiles[x,y].Position.ToVector2() * Tile_size).ToPoint(), Tile_size.ToPoint()), Color.White, rotation, tile_size.ToVector2() / 2f, scale, effects, depth);
            }
        }
    }

    public Tile GetTile(Point position) => _tiles[position.X, position.Y];
    public void SetTile(Tile tile, Point position) => _tiles[position.X, position.Y] = tile;

    public Point WorldToIso(Vector2 World) {
        return new(
            (int)System.Math.Round(World.X / Face_size.X + World.Y / Face_size.Y - 1),
            (int)System.Math.Round(World.Y / Face_size.Y - World.X / Face_size.X)
        );
    }

    public Vector2 IsoToWorld(Vector2 Iso) {
        return new(
            (Iso.X - Iso.Y) * FaceOver2.X,
            (Iso.X + Iso.Y) * FaceOver2.Y
        );
    }
}
