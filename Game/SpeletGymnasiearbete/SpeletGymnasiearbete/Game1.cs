using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace SpeletGymnasiearbete;


enum SPRITE {
    background,
    player,

    _COUNT
}


public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _sprite_batch;
    private Node _root;
    private Sprite2D[] _sprite_group = new Sprite2D[(int)SPRITE._COUNT];

    private float _player_speed = 500f;
    private Node2D _player;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    public Texture2D CreateTexture(int width, int height, Color color)
    {
        Texture2D texture = new Texture2D(GraphicsDevice, width, height);
        Color[] data = new Color[width*height];
        for(int pixel=0; pixel<data.Count(); pixel++) { data[pixel] = color; }
        texture.SetData(data);
        return texture;
    }

    
    protected override void Initialize()
    {
        /* --- node tree --- */

        _root = new Node([
            _player = new Node2D(
                new Vector2(0, 0),
                children:
                [
                    _sprite_group[(int)SPRITE.player] = new Sprite2D(Vector2.Zero)
                ]
            ),
            _sprite_group[(int)SPRITE.background] = new Sprite2D(Vector2.Zero)
        ]);

        /* --- Scripts --- */

        _player._process = (GameTime deltaTime) => {
            InputNode.Update();
            _player.Position += InputNode.DirectionNormalized * _player_speed * (float)deltaTime.ElapsedGameTime.TotalSeconds;
            System.Console.WriteLine(_player.Position);
            return true;
        };

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _sprite_batch = new SpriteBatch(GraphicsDevice);

        _sprite_group[(int)SPRITE.background].Texture = CreateTexture(
            GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
            GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height,
            Color.SteelBlue
        );

        _sprite_group[(int)SPRITE.player].Texture = Content.Load<Texture2D>("Player-1");
    }

    protected override void Update(GameTime deltaTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) { Exit(); }
        _root.Update(deltaTime);
        _root.Update_children(deltaTime);
        base.Update(deltaTime);
    }

    protected override void Draw(GameTime deltaTime)
    {
        _sprite_batch.Begin();
        foreach (Sprite2D sprite in _sprite_group)
        {
            _sprite_batch.Draw(sprite.Texture, sprite.Position, Color.White);
        }
        _sprite_batch.End();
        base.Draw(deltaTime);
    }
}





/*
public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _playerTexture;    
    private Vector2 _player_position;


    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
       
        // TODO: use this.Content to load your game content here
        _playerTexture = Content.Load<Texture2D>("Player-1");
    }

    protected override void Update(GameTime gameTime)
    {

        // TODO: Add your update logic here
        Vector2 mouse_position = Mouse.GetState().Position.ToVector2();
        _player_position = mouse_position - _playerTexture.Bounds.Size.ToVector2() / 2;
        
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here
        _spriteBatch.Begin();
        _spriteBatch.Draw(_playerTexture, _player_position, Color.White);
        _spriteBatch.End();
        base.Draw(gameTime);
    }
}
*/