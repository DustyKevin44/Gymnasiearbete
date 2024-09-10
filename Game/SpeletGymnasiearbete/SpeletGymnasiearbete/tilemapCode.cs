using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using MonoGame.Extended.Content;
using SpeletGymnasiearbete.Classes;

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
// Parsing methods
public class TileMapParser
{
    public TiledMap ParseTmx(string filePath)
    {
        // Load the TMX file content
        var tmxContent = File.ReadAllText(filePath);
        var xdoc = XDocument.Parse(tmxContent);

        // Extract map properties
        var mapElement = xdoc.Root;
        if (mapElement == null) throw new InvalidDataException("Invalid TMX file.");

        var mapName = mapElement.Attribute("name")?.Value ?? "UntitledMap";
        var width = int.Parse(mapElement.Attribute("width")?.Value ?? "0");
        var height = int.Parse(mapElement.Attribute("height")?.Value ?? "0");
        var tileWidth = int.Parse(mapElement.Attribute("tilewidth")?.Value ?? "0");
        var tileHeight = int.Parse(mapElement.Attribute("tileheight")?.Value ?? "0");

        // Example values for drawOrder and orientation (use appropriate values based on your TMX file)
        var drawOrder = TiledMapTileDrawOrder.RightDown; // Example
        var orientation = TiledMapOrientation.Orthogonal; // Example
        var backgroundColor = Color.Transparent; // Example

        // Create TiledMap
        var map = new TiledMap(
            mapName, 
            width, 
            height, 
            tileWidth, 
            tileHeight, 
            drawOrder, 
            orientation, 
            backgroundColor
        );

        // Extract and add layers
        var layers = new List<TiledLayer>();
        foreach (var layerElement in mapElement.Elements("layer"))
        {
            // Example: Create TiledLayer based on layerElement attributes and add to layers list
            // You'll need to parse actual layer data and add to the list
            var layerName = layerElement.Attribute("name")?.Value ?? "UnnamedLayer";
            var layerWidth = int.Parse(layerElement.Attribute("width")?.Value ?? "0");
            var layerHeight = int.Parse(layerElement.Attribute("height")?.Value ?? "0");

            // Create TiledLayer and add to the list
            var layer = new TiledLayer(layerName, layerWidth, layerHeight);
            layers.Add(layer);
        }

        // Add layers to map
        // This is a placeholder; you may need to adjust based on how you handle layers in your TiledMap implementation
        foreach (var layer in layers)
        {
            map.Layers.Add(layer); // Assuming TiledMap has an appropriate method to add layers
        }

        return map;
    }
}