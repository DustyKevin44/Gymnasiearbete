using System;
using Game.Custom.Saving;
using Game.Custom.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using MonoGame.Extended.Graphics;

namespace Game.Custom.GameStates;


public class LoadMenuState : GameState
{
    private readonly List<UIElement> _UI;
    private SaveManager _saveManager = new();
    private List<string> _saves;
    private int _selectedIndex = 0;


    public LoadMenuState(Game game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
    {
        Texture2D buttonSheet = content.Load<Texture2D>("ButtonSheet");
        Texture2DAtlas atlas = Texture2DAtlas.Create("Atlas/button", buttonSheet, 64, 8);
        SpriteSheet buttonSpriteSheet = new("SpriteSheet/button", atlas);
        buttonSpriteSheet.DefineAnimation("idle", builder =>
            {
                builder.IsLooping(true)
                    .AddFrame(0, TimeSpan.FromSeconds(0.4))
                    .AddFrame(1, TimeSpan.FromSeconds(0.4))
                    .AddFrame(2, TimeSpan.FromSeconds(0.4));
            });
        SpriteFont buttonFont = _content.Load<SpriteFont>("Fonts/Font");
        SpriteBatch _spriteBatch = new SpriteBatch(graphicsDevice);
        AnimatedSprite buttonSprite = new AnimatedSprite(buttonSpriteSheet, "idle");

        _saves = _saveManager.GetSaves(); // Get all saves

        var LoadSaveOneButton = new Button(buttonSprite, buttonFont)
        {
            Position = new Vector2(300, 200),
            Text = "Save 1"
        };

        LoadSaveOneButton.Click += LoadSaveOneButton_Click;

        var LoadSaveTwoButton = new Button(buttonSprite, buttonFont)
        {
            Position = new Vector2(300, 250),
            Text = "Save 2"
        };

        LoadSaveTwoButton.Click += LoadSaveTwoButton_Click;

        var LoadSaveThreeButton = new Button(buttonSprite, buttonFont)
        {
            Position = new Vector2(300, 300),
            Text = "Save 3"
        };

        LoadSaveThreeButton.Click += LoadSaveThreeButton_Click;

        var BackButton = new Button(buttonSprite, buttonFont)
        {
            Position = new Vector2(300, 400),
            Text = "Go back"
        };

        BackButton.Click += BackButton_Click;

        _UI = [
            BackButton,
            LoadSaveThreeButton,
            LoadSaveTwoButton,
            LoadSaveOneButton,
        ];
    }

    private void LoadSaveThreeButton_Click(object sender, EventArgs e)
    {
        Console.WriteLine("Load game 3");
    }

    private void LoadSaveTwoButton_Click(object sender, EventArgs e)
    {
        Console.WriteLine("Load game 2");
    }

    private void LoadSaveOneButton_Click(object sender, EventArgs e)
    {
        Console.WriteLine("Load game 1");
    }

    private void BackButton_Click(object sender, EventArgs e)
    {
        _game.ChangeState(new MenuState(_game, _graphicsDevice, _content));
    }


    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);

        foreach (UIElement element in _UI)
        {
            element.Draw(gameTime, spriteBatch);
        }

        spriteBatch.End();
    }

    public override void LoadContent() { }

    public override void PostUpdate(GameTime gameTime) { }

    public override void Update(GameTime gameTime)
    {
        foreach (UIElement element in _UI)
        {
            element.Update(gameTime);
        }
    }
}
