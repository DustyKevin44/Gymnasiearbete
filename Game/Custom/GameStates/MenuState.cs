using Game.Custom.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
using MonoGame.Extended.Graphics;
using Game.Custom.Saving;

namespace Game.Custom.GameStates;

public class MenuState : GameState
{
    private readonly List<UIElement> _UI;

    public MenuState(Game game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
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

        var continueButton = new Button(buttonSprite, buttonFont)
        {
            Position = new Vector2(300, 200),
            Text = "Continue"
        };

        continueButton.Click += ContinueButton_Click;

        var loadGameButton = new Button(buttonSprite, buttonFont)
        {
            Position = new Vector2(300, 250),
            Text = "Load"
        };

        loadGameButton.Click += LoadGameButton_Click;

        var quitGameButton = new Button(buttonSprite, buttonFont)
        {
            Position = new Vector2(300, 300),
            Text = "Quit"
        };

        quitGameButton.Click += QuitGameButton_Click;

        _UI = [
            continueButton,
            loadGameButton,
            quitGameButton,
        ];
    }

    private void ContinueButton_Click(object sender, EventArgs e)
    {
        var save = Global.SaveManager.GetLastPlayedSave();
        if (save.HasValue)
        {
            var gameState = Global.SaveManager.StartFromSave(_game, _graphicsDevice, _content, save.Value.GameId);
            _game.ChangeState(gameState);
        }
        else
        {
            Console.WriteLine("No saves");
        }
    }

    private void LoadGameButton_Click(object sender, EventArgs e)
    {
        _game.ChangeState(new LoadMenuState(_game, _graphicsDevice, _content));
    }

    private void QuitGameButton_Click(object sender, EventArgs e)
    {
        _game.Exit();
        Console.WriteLine("Exit game");
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
