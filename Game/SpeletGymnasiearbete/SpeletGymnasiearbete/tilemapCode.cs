using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using MonoGame.Extended.Content;
using SpeletGymnasiearbete.Classes;
using System.IO;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace SpeletGymnasiearbete;



public class TilemapCode
{
    private TiledMap _tiledMap;
    private TiledMapRenderer _tiledMapRenderer;

    public Dictionary<Vector2, int> LoadMap(string filepath)
    {
        Dictionary<Vector2, int> result = new();
        StreamReader reader = new(filepath);
        int y = 0;
        string line;
        while((line=reader.ReadLine()) != null)
        {
            string[] items = line.Split(",");

            for (int x = 0; x < items.Length; x++)
            {
                if(int.TryParse(items[x], out int value)){
                    if (value > -1)
                    {
                        result[new Vector2(x,y)] = value;
                    }
                }
            }
            y++;
        }
        return result;
    }
    // Load the TMX file as an XDocument
    public static string LayerData(string tmxFilePath, string layerName){
        XDocument tmxDocument = XDocument.Load(tmxFilePath);

        var layer = tmxDocument.Descendants("layer").FirstOrDefault(l => l.Attribute("name").Value == layerName);
                                 
        if (layer == null)
        {
            throw new Exception($"Layer '{layerName}' not found in the TMX file.");
        }
        var dataElement = layer.Element("data");

        if (dataElement != null && dataElement.Attribute("encoding").Value == "csv")
        {
            // Return the entire CSV data as a single string
            string csvData = dataElement.Value.Trim();
            return csvData;
        }
        return null;
    }
  
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