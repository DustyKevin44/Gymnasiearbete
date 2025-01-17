using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Custom.Tilemap;

public class Map
{
    public readonly Point Size = new(50, 50);
    public Tile[,] Tiles { get; }
    public Point TileSize { get; }

    public Vector2 MapToScreen(int x, int y) =>  new(x * TileSize.X, y * TileSize.Y);
    public (int x, int y) ScreenToMap(Vector2 pos) => ((int)pos.X / TileSize.X, (int)pos.Y / TileSize.Y);

    public Map(ContentManager content)
    {
        Tiles = new Tile[Size.X, Size.Y];
        var texture = content.Load<Texture2D>("tile");
        TileSize = new(16, 16);// new(texture.Width, texture.Height);

        for (int y = 0; y < Size.Y; y++)
        {
            for (int x = 0; x < Size.X; x++)
            {
                Tiles[x, y] = new(texture, MapToScreen(x, y), x, y);
            }
        }
    }

    public void Update()
    {
        for (int y = 0; y < Size.Y; y++)
        {
            for (int x = 0; x < Size.X; x++) Tiles[x, y].Update();
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        for (int y = 0; y < Size.Y; y++)
        {
            for (int x = 0; x < Size.X; x++) Tiles[x, y].Draw(spriteBatch);
        }
    }
}