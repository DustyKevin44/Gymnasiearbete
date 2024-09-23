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
            (int)System.Math.Round(World.X / Face_size.X * Scale.X + World.Y / Face_size.Y * Scale.Y - 1),
            (int)System.Math.Round(World.Y / Face_size.Y * Scale.Y - World.X / Face_size.X * Scale.X)
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
    private Point min = Point.Zero;
    private List<List<IChunk>> _chunks = [[]];

    public void Draw(Point start, Point end, SpriteBatch spriteBatch, TileMap tileMap, int layer_id)
    {
        start.X = (start.X < min.X) ? min.X : start.X;
        start.Y = (start.Y < min.Y) ? min.Y : start.Y;
        for(;start.X<end.X;start.X++)
        {
            if (start.X >= _chunks.Count) continue;
            for(;start.Y<end.Y;start.Y++)
            {
                if (start.Y >= _chunks[start.X].Count) break;
                if (_chunks[start.X][start.Y] is null) continue;
                _chunks[start.X][start.Y].Draw(spriteBatch, tileMap, new Vector2(0, layer_id * tileMap.Face_size.Y * tileMap.Scale.Y));
            }
        }
    }

    public IChunk GetChunk(Point position)
    {
        position -= min;
        if (_chunks.Count > position.X && position.X >= 0 &&
            _chunks[position.X].Count > position.Y && position.Y >= 0)
        {
            return _chunks[position.X][position.Y];
        }
        return null;
    }

    public IChunk SetChunk(IChunk chunk, Point position)
    {
        // Append Lists and Chunks until _chunks[position.X][position.Y] = null
        if (position.X < min.X)
        {
            _chunks.InsertRange(0, Enumerable.Repeat<List<IChunk>>([], min.X - position.X + 1));
            min.X = position.X;
        }
        else if (position.X >= _chunks.Count)
            _chunks.AddRange(Enumerable.Repeat<List<IChunk>>([], position.X - min.X + 1));
        
        if (position.Y < min.Y)
        {
            _chunks[position.X].InsertRange(0, Enumerable.Repeat<IChunk>(null, min.Y - position.Y + 1));
            min.Y = position.Y;
        }
        else if (position.Y >= _chunks[position.X].Count)
            _chunks[position.X].AddRange(Enumerable.Repeat<IChunk>(null, position.Y - min.Y + 1));
        // Set Chunk and return the replaced chunk, null if no chunk replaced
        IChunk replaced = _chunks[position.X][position.Y];
        _chunks[position.X][position.Y] = chunk;
        return replaced;
    }

    public static TileLayer FromSVC(string filepath, Point chunk_size)
    {
        TileLayer layer = new();

        System.IO.StreamReader reader = new(filepath);
        int y = 0;
        while(reader.ReadLine() is string line)
        {
            string[] items = line.Split(",");
            for (int x = 0; x < items.Length; x++)
            {
                if (int.TryParse(items[x], out int value))
                {
                    if (x / chunk_size.X >= layer._chunks.Count)
                        layer.SetChunk(new ChunkPlane(chunk_size), new(x / chunk_size.X, y / chunk_size.Y));
                    else if (y / chunk_size.Y >= layer._chunks[x / chunk_size.X].Count)
                        layer.SetChunk(new ChunkPlane(chunk_size), new(x / chunk_size.X, y / chunk_size.Y));
                    if (layer._chunks[x/chunk_size.X][y/chunk_size.Y] is null) layer._chunks[x/chunk_size.X][y/chunk_size.Y] = new ChunkPlane(chunk_size);
                    layer._chunks[x/chunk_size.X][y/chunk_size.Y].SetTile(new(x % chunk_size.X, y % chunk_size.Y), value);
                }
            }
            y++;
        }
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


/*
TileMap
    * TileLayers
    * Tileset
    * Tile Logic

TileLayer
    * TileChunks

TileChunk (Fixed size)
    * Tiles
        - Map: Vector -> T
        or Array: [T, T, ...], [...], ...

*/
