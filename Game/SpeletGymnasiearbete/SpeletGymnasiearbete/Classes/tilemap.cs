using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using static SpeletGymnasiearbete.Utils;
namespace SpeletGymnasiearbete.Classes;

public class TileMap(int layers, Vector2 scale, Point face_size, Point tile_size)
{
    public readonly TileLayer[] Layers = new TileLayer[layers];
    public Texture2D Tileset { private set; get; }

    public Vector2 Scale = scale;
    public Point Face_size = face_size;
    public Point Tile_size = tile_size;
    protected Vector2 _faceOver2 = face_size.ToVector2() / 2f;

    public void LoadTileset(string path) { Tileset = Globals.ContentManager.Load<Texture2D>(path); }
    public void LoadLayer(string csv_filepath, int layer, Point chunk_size) { Layers[layer] = TileLayer.FromSVC(csv_filepath, chunk_size); }

    public Point WorldToIso(Vector2 World) {
        return new(
            (int)System.Math.Round(World.X / (Face_size.X * Scale.X) + World.Y / (Face_size.Y * Scale.Y - 1)),
            (int)System.Math.Round(World.Y / (Face_size.Y * Scale.Y) - World.X / (Face_size.X * Scale.X))
        );
    }
    
    public Vector2 IsoToWorld(Point Iso) {
        return new(
            (Iso.X - Iso.Y) * _faceOver2.X * Scale.X,
            (Iso.X + Iso.Y) * _faceOver2.Y * Scale.Y
        );
    }
}

public class TileLayer
{
    private readonly List<List<IChunk>> _chunks = [[]];

    public void Draw(Point start, Point end, SpriteBatch spriteBatch, TileMap tileMap)
    {
        for (int x=start.X; x <= end.X; x++)
        {
            if (x < 0 || x >= _chunks.Count) continue;
            for (int y=start.Y; y <= end.Y; y++)
            {
                if (y < 0 || y >= _chunks[x].Count) continue;
                if (_chunks[x][y] is null) continue;
                Vector2 position = tileMap.IsoToWorld(new(x*2, y*2));
                _chunks[x][y].Draw(spriteBatch, tileMap, new Vector2(position.X, position.Y));
            }
        }
    }

    public IChunk GetChunk(Point position)
    {
        if (_chunks.Count > position.X && position.X >= 0 &&
            _chunks[position.X].Count > position.Y && position.Y >= 0)
        {
            return _chunks[position.X][position.Y];
        }
        return null;
    }

    public void SetChunk(IChunk chunk, Point position)
    {
        if (position.X < 0) {
            for (int x=position.X; x<=0; x++) _chunks.Add([]);
            position.X = 0;
        } else {
            for (int x=0; x <= position.X - _chunks.Count; x++) _chunks.Add([]);
        }
        if (position.Y < 0) {
            for (int y=position.Y; y<=0; y++) _chunks[position.X].Add(null);
            position.Y = 0;
        } else {
            for (int y=0; y <= position.Y - _chunks[position.X].Count; y++) _chunks[position.X].Add(null);
        }
        _chunks[position.X][position.Y] = chunk;
    }

    public static TileLayer FromSVC(string filepath, Point chunk_size)
    {
        if (chunk_size.X < 1 || chunk_size.Y < 1) throw new System.Exception("Can not initialize TileLayer with a chunk size less than (1, 1).");
        TileLayer layer = new();

        System.IO.StreamReader reader = new(filepath);
        int y = 0;
        while(reader.ReadLine() is string line)
        {
            string[] items = line.Split(",");
            for (int x = 0; x < items.Length; x++)
            {
                Point chunk_pos = new(x / chunk_size.X, y / chunk_size.Y);
                Point tile_pos = new(x % chunk_size.X, y % chunk_size.Y);

                if (chunk_pos.X >= layer._chunks.Count)
                    layer._chunks.Add([]);
                if (chunk_pos.Y >= layer._chunks[chunk_pos.X].Count)
                    layer._chunks[chunk_pos.X].Add(new ChunkPlane(chunk_size));
                    
                layer._chunks[chunk_pos.X][chunk_pos.Y].SetTile(tile_pos, int.TryParse(items[x], out int value) ? value : 1);
            }
            y++;
        }
        reader.Close();
        return layer;
    }
}

public interface IChunk
{
    public void SetTile(Point position, int tile);
    public int GetTile(Point position);
    public void Draw(SpriteBatch spriteBatch, TileMap tileMap, Vector2 offset);
}

public class ChunkPlane : IChunk
{
    private readonly int[][] _tiles;

    public ChunkPlane(Point Size)
    {
        _tiles = new int[Size.X][];
        for(int x=0; x<Size.X; x++)
        {
            _tiles[x] = new int[Size.Y];
            for(int y=0; y<Size.Y; y++)
            {
                _tiles[x][y] = -1;
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch, TileMap tileMap, Vector2 offset)
    {
        for(int x=0; x<_tiles.Length; x++)
        {
            for(int y=0; y<_tiles[x].Length; y++)
            {
                if (_tiles[x][y] == -1) continue;
                spriteBatch.Draw(tileMap.Tileset, tileMap.IsoToWorld(new Point(x, y)) + offset - Globals.Active_Camera.Position, new Rectangle(_tiles[x][y] * tileMap.Tile_size.X, 0, tileMap.Tile_size.X, tileMap.Tile_size.Y), Color.White, 0f, Vector2.Zero, tileMap.Scale, SpriteEffects.None, 0f);
            }
        }
    }

    public void SetTile(Point position, int tile) { _tiles[position.X][position.Y] = tile; }
    public int GetTile(Point position) { return _tiles[position.X][position.Y]; }
}

public class ChunkMap : IChunk
{
    private readonly Dictionary<Point, int> _tiles = [];

    public void Draw(SpriteBatch spriteBatch, TileMap tileMap, Vector2 offset)
    {
        foreach(KeyValuePair<Point, int> pair in _tiles)
        {
            spriteBatch.Draw(tileMap.Tileset, tileMap.IsoToWorld(pair.Key) + offset - Globals.Active_Camera.Position, new Rectangle(pair.Value * tileMap.Tile_size.X, 0, tileMap.Tile_size.X, tileMap.Tile_size.Y), Color.White, 0f, Vector2.Zero, tileMap.Scale, SpriteEffects.None, 0f);
        }
    }

    public void SetTile(Point position, int tile) { _tiles[position] = tile; }
    public int GetTile(Point position) { return _tiles[position]; }
}
