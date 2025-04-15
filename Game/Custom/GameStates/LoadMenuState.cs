using System;
using System.Collections.Generic;
using Game.Custom.Objects;
using Game.Custom.Saving;
using Game.Custom.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;
using static Game.Custom.Saving.SaveManager;

namespace Game.Custom.GameStates
{
    public class LoadMenuState : GameState
    {
        private readonly List<UIElement> _UI;
        private List<GameSave> _saves;  // Non-nullable GameSave objects
        private int _selectedIndex = 0;

        public LoadMenuState(Game game, GraphicsDevice graphicsDevice, ContentManager content)
            : base(game, graphicsDevice, content)
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

            _saves = Global.SaveManager.GetAllGameSaves();  // List of GameSave objects
            Global.SaveManager.PrintAllSavedData();
            // Create button for Save Slot 1
            var LoadSaveOneButton = new Button(buttonSprite, buttonFont)
            {
                Position = new Vector2(300, 200),
                Text = (_saves.Count > 0 && _saves[0].GameId != 0) ? $"Save: {_saves[0].SaveName}" : "Save slot empty"
            };
            LoadSaveOneButton.Click += (s, e) => TryLoadSave(0, (e as ButtonEventArgs).gameTime);

            // Create button for Save Slot 2
            var LoadSaveTwoButton = new Button(buttonSprite, buttonFont)
            {
                Position = new Vector2(300, 250),
                Text = (_saves.Count > 1 && _saves[1].GameId != 0) ? $"Save: {_saves[1].SaveName}" : "Save slot empty"
            };
            LoadSaveTwoButton.Click += (s, e) => TryLoadSave(1, (e as ButtonEventArgs).gameTime);

            // Create button for Save Slot 3
            var LoadSaveThreeButton = new Button(buttonSprite, buttonFont)
            {
                Position = new Vector2(300, 300),
                Text = (_saves.Count > 2 && _saves[2].GameId != 0) ? $"Save: {_saves[2].SaveName}" : "Save slot empty"
            };
            LoadSaveThreeButton.Click += (s, e) => TryLoadSave(2, (e as ButtonEventArgs).gameTime);

            // Button to create a new save for each slot
            var CreateSaveButton = new Button(buttonSprite, buttonFont)
            {
                Position = new Vector2(700, 200),
                Text = "New Save"
            };
            CreateSaveButton.Click += (s, e) => CreateNewSave();


            // Back Button
            var BackButton = new Button(buttonSprite, buttonFont)
            {
                Position = new Vector2(300, 400),
                Text = "Go back"
            };
            BackButton.Click += BackButton_Click;

            _UI = [
                BackButton,
                LoadSaveOneButton,
                LoadSaveTwoButton,
                LoadSaveThreeButton,
                CreateSaveButton
            ];
        }

        private void TryLoadSave(int index, GameTime gameTime)
        {
            if (index < _saves.Count && _saves[index].GameId != 0)
            {
                int gameId = _saves[index].GameId;
                Console.WriteLine($"Loading GameId: {gameId}");
                var gameState = Global.SaveManager.StartFromSave(_game, _graphicsDevice, _content, gameId);
                _game.ChangeState(gameState);
                Global.TimeGameStarted = gameTime.TotalGameTime.Seconds;
            }
            else
            {
                Console.WriteLine("Save slot empty");
            }
        }

        private void CreateNewSave()
        {
            const int MaxSaves = 3;

            if (_saves.Count >= MaxSaves)
            {
                Console.WriteLine("No remaining save slots.");
                return;
            }

            string saveName = $"Save Slot {_saves.Count + 1}";
            int gameId = Global.SaveManager.CreateNewSave(saveName);

            _saves = Global.SaveManager.GetAllGameSaves();
            Global.SaveManager.AddEntity(gameId, new(0, 0), "Player", 100);

            Console.WriteLine($"Created new save in slot {_saves.Count} with GameId: {gameId}");
            UpdateButtonText(_saves.Count);
        }

        private void UpdateButtonText(int slot)
        {
            // Update the button text after creating a new save
            switch (slot)
            {
                case 1:
                    var LoadSaveOneButton = _UI[1] as Button;
                    LoadSaveOneButton.Text = $"Save: {_saves[0].SaveName}";
                    break;
                case 2:
                    var LoadSaveTwoButton = _UI[2] as Button;
                    LoadSaveTwoButton.Text = $"Save: {_saves[1].SaveName}";
                    break;
                case 3:
                    var LoadSaveThreeButton = _UI[3] as Button;
                    LoadSaveThreeButton.Text = $"Save: {_saves[2].SaveName}";
                    break;
            }
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
}
