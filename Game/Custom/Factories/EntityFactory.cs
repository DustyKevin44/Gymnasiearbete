using System.Collections.Generic;
using System.Linq;
using Game.Custom.Components;
using Game.Custom.Components.Systems;
using Game.Custom.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Input;

namespace Game.Custom.Factories;


public static class EntityFactory
{
    public static Entity CreatePlayerAt(Vector2 position)
    {
        void ZoomOut(GameTime gameTime)
        {
            Global.Camera?.ZoomOut(0.5f * gameTime.GetElapsedSeconds());
        }

        void ZoomIn(GameTime gameTime)
        {
            Global.Camera?.ZoomIn(0.5f * gameTime.GetElapsedSeconds());
        }

        var player = Global.World.CreateEntity();
        player.Attach(new Transform2(position));
        player.Attach(new VelocityComponent(Vector2.Zero));
        player.Attach(new SpriteComponent(Global.ContentLibrary.Sprites["player"]));
        player.Attach(new CollisionBox(new RectangleF(0, 0, 20, 20)));
        player.Attach(new PlayerComponent<StdActions>(
            "Player", new Dictionary<StdActions, Keybind> {
                { StdActions.MOVE_UP,    new Keybind(key: Keys.W) },
                { StdActions.MOVE_DOWN,  new Keybind(key: Keys.S) },
                { StdActions.MOVE_LEFT,  new Keybind(key: Keys.A) },
                { StdActions.MOVE_RIGHT, new Keybind(key: Keys.D) },
                { StdActions.DASH,       new Keybind(key: Keys.Space) },
                { StdActions.CUSTOM,     new CustomKeybind(ZoomIn, mouseButton: MouseButton.Right)},
                { StdActions.CUSTOM2,    new CustomKeybind(ZoomOut, mouseButton: MouseButton.Left)}
            })
        );

        Global.Players.Add(player);
        player.Get<CollisionBox>().Parent = player;
        return player;
    }

    public static Entity CreateSlimeAt(Vector2 position)
    {
        Color[] colors = [Color.Black, Color.White, Color.Aqua, Color.Green, Color.Yellow];
        var slimeCollision = new CollisionBox(new RectangleF(0, 0, 16, 16));

        var slime = Global.World.CreateEntity();
        slime.Attach(new Transform2(position));
        slime.Attach(new VelocityComponent(Vector2.Zero));
        slime.Attach(new Behavior(0, default, Global.Players.FirstOrDefault(defaultValue: null))); // <-- Maybe allow for multiple targets in the future
        slime.Attach(new AnimatedSprite(Global.ContentLibrary.Animations["slime"], "slimeAnimation") { Color = colors[Global.Random.Next(0, 5)] } );
        slime.Attach(new HealthComponent(100));
        slime.Attach(slimeCollision);
        
        slimeCollision.Parent = slime;
        return slime;
    }
}
