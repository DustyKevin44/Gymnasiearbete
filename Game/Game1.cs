using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using Game.Custom.GameStates;
using MonoGameGum.GueDeriving;
using RenderingLibrary;
using System.Linq;
using GumRuntime;
using System;
using Gum.Wireframe;
using MonoGameGum.Forms.Controls;
using MonoGameGum.Forms;
using System.Collections.Generic;

namespace Game;


public class Game : Microsoft.Xna.Framework.Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private OrthographicCamera _camera;
    // Game states
    private GameState _currentState;
    private GameState _nextState;

    // GUI
    public GraphicalUiElement Root;

    public void ChangeState(GameState state) { _nextState = state; }

    public Game()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _camera = new OrthographicCamera(GraphicsDevice);  // Initialize the camera
        _currentState = new MenuState(this, GraphicsDevice, Content);
        Point WindowSize = new(1200, 900);
        _graphics.PreferredBackBufferWidth = WindowSize.X;
        _graphics.PreferredBackBufferHeight = WindowSize.Y;
        _graphics.ApplyChanges();

        // Gum GUI
        var project = MonoGameGum.GumService.Default.Initialize(this, "GumProject/GumProject.gumx");
        var screen = project.Screens.Find(item => item.Name == "TitleScreen"); // Loads the title screen
        Root = screen.ToGraphicalUiElement(SystemManagers.Default, true);

        var button = Root.GetFrameworkElementByName<Button>("ButtonStandardInstance");
        button.Click += (_, _) => Root = project.Screens.Find(item => item.Name == "Main").ToGraphicalUiElement(SystemManagers.Default, true);

        base.Initialize();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            Exit();

        // State logic
        if (_nextState != null)
        {
            _currentState = _nextState;
            _nextState = null;
        }

        _currentState.Update(gameTime);
        _currentState.PostUpdate(gameTime);
        
        MonoGameGum.GumService.Default.Update(this, gameTime, Root);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        _currentState.Draw(gameTime, _spriteBatch);  // Draw state
        
        MonoGameGum.GumService.Default.Draw();
        base.Draw(gameTime);
    }
}
