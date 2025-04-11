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
        private SaveManager _saveManager = new();
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

            _saves = _saveManager.GetAllGameSaves();  // List of GameSave objects
            _saveManager.PrintAllSavedData();
            // Create button for Save Slot 1
            var LoadSaveOneButton = new Button(buttonSprite, buttonFont)
            {
                Position = new Vector2(300, 200),
                Text = (_saves.Count > 0 && _saves[0].GameId != 0) ? $"Save: {_saves[0].SaveName}" : "Save slot empty"
            };
            LoadSaveOneButton.Click += (s, e) => TryLoadSave(0);

            // Create button for Save Slot 2
            var LoadSaveTwoButton = new Button(buttonSprite, buttonFont)
            {
                Position = new Vector2(300, 250),
                Text = (_saves.Count > 1 && _saves[1].GameId != 0) ? $"Save: {_saves[1].SaveName}" : "Save slot empty"
            };
            LoadSaveTwoButton.Click += (s, e) => TryLoadSave(1);

            // Create button for Save Slot 3
            var LoadSaveThreeButton = new Button(buttonSprite, buttonFont)
            {
                Position = new Vector2(300, 300),
                Text = (_saves.Count > 2 && _saves[2].GameId != 0) ? $"Save: {_saves[2].SaveName}" : "Save slot empty"
            };
            LoadSaveThreeButton.Click += (s, e) => TryLoadSave(2);

            // Button to create a new save for each slot
            var CreateSaveOneButton = new Button(buttonSprite, buttonFont)
            {
                Position = new Vector2(700, 200),
                Text = "Create Save 1"
            };
            CreateSaveOneButton.Click += (s, e) => CreateNewSave(0);

            var CreateSaveTwoButton = new Button(buttonSprite, buttonFont)
            {
                Position = new Vector2(700, 250),
                Text = "Create Save 2"
            };
            CreateSaveTwoButton.Click += (s, e) => CreateNewSave(1);

            var CreateSaveThreeButton = new Button(buttonSprite, buttonFont)
            {
                Position = new Vector2(700, 300),
                Text = "Create Save 3"
            };
            CreateSaveThreeButton.Click += (s, e) => CreateNewSave(2);

            // Back Button
            var BackButton = new Button(buttonSprite, buttonFont)
            {
                Position = new Vector2(300, 400),
                Text = "Go back"
            };
            BackButton.Click += BackButton_Click;

            _UI = new List<UIElement>
            {
                BackButton,
                LoadSaveThreeButton,
                LoadSaveTwoButton,
                LoadSaveOneButton,
                CreateSaveThreeButton,
                CreateSaveTwoButton,
                CreateSaveOneButton
            };
        }

        private void TryLoadSave(int index)
        {
            if (index < _saves.Count && _saves[index].GameId != 0)
            {
                int gameId = _saves[index].GameId;
                Console.WriteLine($"Loading GameId: {gameId}");
                _saveManager.StartStartFromSave(_game, _graphicsDevice, _content, gameId);
                // Optionally: Transition to the game state after loading
                // _game.ChangeState(new GameWorldState(...));
            }
            else
            {
                Console.WriteLine("Save slot empty");
            }
        }

        private void CreateNewSave(int slot)
        {
            // Check if the save slot is empty before creating a new save
            if (_saves.Count <= slot || _saves[slot].GameId == 0)
            {
                string saveName = $"Save Slot {slot + 1}";
                int gameId = _saveManager.CreateNewSave(saveName);
                _saves = _saveManager.GetAllGameSaves(); // Refresh the list of saves after creating a new save
                Console.WriteLine($"Created new save in slot {slot + 1} with GameId: {gameId}");
                _saveManager.AddEntity(slot, new(0,0), "Player", 100);
                // Optionally, update the button text to reflect the new save name
                UpdateButtonText(slot);
            }
            else
            {
                Console.WriteLine("This slot already has a save.");
            }
        }

        private void UpdateButtonText(int slot)
        {
            // Update the button text after creating a new save
            switch (slot)
            {
                case 0:
                    var LoadSaveOneButton = _UI[3] as Button;
                    LoadSaveOneButton.Text = $"Save: {_saves[0].SaveName}";
                    break;
                case 1:
                    var LoadSaveTwoButton = _UI[4] as Button;
                    LoadSaveTwoButton.Text = $"Save: {_saves[1].SaveName}";
                    break;
                case 2:
                    var LoadSaveThreeButton = _UI[5] as Button;
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
