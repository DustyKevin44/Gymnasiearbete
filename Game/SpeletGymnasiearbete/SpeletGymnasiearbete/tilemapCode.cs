using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using MonoGame.Extended.Content;

namespace SpeletGymnasiearbete;

public class TilemapCode
{
    private TiledMap _tiledMap;
    private TiledMapRenderer _tiledMapRenderer;

    public void LoadContent(GraphicsDevice graphicsDevice, ContentManager content, string mapName)
    {
        // Load the TMX map file
        _tiledMap = content.Load<TiledMap>(mapName); // yourTileMap.tmx file without extension
        _tiledMapRenderer = new TiledMapRenderer(graphicsDevice, _tiledMap);
    }

    public void Update(GameTime gameTime)
    {
        _tiledMapRenderer?.Update(gameTime);
    }

    public void Draw(GameTime gameTime)
    {
        _tiledMapRenderer?.Draw();
    }
}
