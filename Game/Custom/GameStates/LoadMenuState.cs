using Game.Custom.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
using MonoGame.Extended.Graphics;
using Game.Custom;
namespace Game.Custom.GameStates;

public class LoadMenuState : GameState
{
    private readonly List<UIElement> UI;

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
        #region Save variables
        private SaveManager saveManager = new SaveManager;
        private List<string> saves;
        private int selectedIndex = 0;
        saves = saveManager.GetSaves(); // Get all saves
        #endregion
        for 
        

        var LoadSaveOneGameButton = new Button(buttonSprite, buttonFont)
        {
            Position = new Vector2(300, 300),
            Text = "Quit"
        };

        LoadSaveOneGameButton.Click += LoadSaveOneGameButton_Click;

        var LoadSaveTwoGameButton = new Button(buttonSprite, buttonFont)
        {
            Position = new Vector2(300, 300),
            Text = "Quit"
        };

        LoadSaveTwoGameButton.Click += LoadSaveTwoGameButton_Click;

        var LoadSaveThreeGameButton = new Button(buttonSprite, buttonFont)
        {
            Position = new Vector2(300, 300),
            Text = "Quit"
        };

        LoadSaveThreeGameButton.Click += LoadSaveThreeGameButton_Click;

        UI = [
            LoadSaveThreeGameButton,
            LoadSaveTwoGameButton,
            LoadSaveOneGameButton,
        ];
    }

    private void LoadSaveTwoGameButton_Click(object sender, EventArgs e)
    {
        Console.WriteLine("Load game");
    }

    private void LoadSaveOneGameButton_Click(object sender, EventArgs e)
    {
        Console.WriteLine("Load game");
    }

    private void LoadSaveThreeGameButton_Click(object sender, EventArgs e)
    {
        _game.Exit();
        Console.WriteLine("Exit game");
    }


    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);

        foreach (UIElement element in UI)
        {
            element.Draw(gameTime, spriteBatch);
        }

        spriteBatch.End();
    }

    public override void LoadContent() { }

    public override void PostUpdate(GameTime gameTime) { }

    public override void Update(GameTime gameTime)
    {
        foreach (UIElement element in UI)
        {
            element.Update(gameTime);
        }
    }
}
