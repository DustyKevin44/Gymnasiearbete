using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Graphics;

namespace Game;


public sealed class Global
{
    private static Global instance = null;
    private static readonly object padlock = new();

    public static Global Instance { get { lock (padlock) { instance ??= new Global(); return instance; } } }

    public static void Initialize(Game game, Random random, CollisionComponent collisionComponent, ContentManager contentManager, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
    {
        Instance._game = game;
        Instance._random = random;
        Instance._collisionComponent = collisionComponent;
        Instance._contentManager = contentManager;
        Instance._graphicsDevice = graphicsDevice;
        Instance._spriteBatch = spriteBatch;
    }

    private World _world;
    private Game _game;
    private OrthographicCamera _camera;
    private Random _random;
    private CollisionComponent _collisionComponent;
    private ContentManager _contentManager;
    private ContentLibrary _contentLibrary = new();
    private GraphicsDevice _graphicsDevice; 
    private SpriteBatch _spriteBatch;
    private readonly List<Entity> _players = [];

    public static List<Entity> Players { get => Instance._players; }
    public static World World { get => Instance._world; set => Instance._world = value; }
    public static Game Game { get => Instance._game; set => Instance._game = value; }
    public static OrthographicCamera Camera { get => Instance._camera; set => Instance._camera = value; }
    public static Random Random { get => Instance._random; set => Instance._random = value; }
    public static CollisionComponent CollisionSystem { get => Instance._collisionComponent; set => Instance._collisionComponent = value; }
    public static ContentManager ContentManager { get => Instance._contentManager; set => Instance._contentManager = value; }
    public static ContentLibrary ContentLibrary { get => Instance._contentLibrary; set => Instance._contentLibrary = value; }
    public static GraphicsDevice GraphicsDevice { get => Instance._graphicsDevice; set => Instance._graphicsDevice = value; }
    public static SpriteBatch SpriteBatch { get => Instance._spriteBatch; set => Instance._spriteBatch = value; }

    public static void SetWorld(World world)
    {
        Instance._world = world;
    }
}


public class ContentLibrary
{
    public Dictionary<string, Texture2D> Sprites = [];
    public Dictionary<string, SpriteSheet> Animations = [];
    public Dictionary<string, Texture2DAtlas> Atlas = [];

    public void SaveSprite(Texture2D texture, string Name)
    {
        Sprites[Name] = texture;
    }

    public void SaveAnimation(SpriteSheet spriteSheet, string Name)
    {
        Animations[Name] = spriteSheet;
    }

    public void SaveAtlas(Texture2DAtlas atlas, string Name)
    {
        Atlas[Name] = atlas;
    }
}

