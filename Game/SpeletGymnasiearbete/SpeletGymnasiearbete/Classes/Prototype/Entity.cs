using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable enable
namespace SpeletGymnasiearbete.Classes.Prototype;


public interface IGameObject {
    public void Update(GameTime gameTime);
    public void Draw(GameTime gameTime, ref SpriteBatch spriteBatch);
}

public class Node(List<Node>? children=null) : IGameObject {
    private List<Node> _children = children ?? [];
    public Node? Parent { get; private set; }

    public ref List<Node> GetChildren() {
        return ref _children;
    }

    public void AddChild(Node child) {
        child.Parent = this;
        _children.Add(child);
    }

    public void RemoveChild(Node child) {
        child.Parent = null;  // Change to global root
        _children.Remove(child);
    }

    public void Update(GameTime gameTime) {
        _children.ForEach(child => {
            child.Update(gameTime);
        });
    }

    public void Draw(GameTime gameTime, ref SpriteBatch spriteBatch) {
        foreach (Node child in _children) {
            child.Draw(gameTime, ref spriteBatch);
        }
    }
}


public class Entity(List<Node>? children=null) : Node(children) {
    public Vector2 Local_position;
    public Vector2 Global_position {
        get {
            Node? obj = Parent;
            Vector2 position = Local_position;
            while (obj != null) {
                if (obj is Entity entity) position += entity.Local_position;
                obj = obj.Parent;
            }
            return position;
        }
        set {
            Node? obj = Parent;
            Vector2 position = Vector2.Zero;
            while (obj != null) {
                if (obj is Entity entity) position += entity.Local_position;
                obj = obj.Parent;
            }
            Local_position = value - position;
        }
    }
}


public class Sprite(Texture2D texture, List<Node>? children=null) : Entity(children) {
    public Texture2D Texture = texture;
}


public abstract class Behaviour(Node owner, List<Node>? children=null) : Node(children) {
    private readonly Node _owner = owner;

    public abstract void Execute(GameTime gameTime, Node owner);
}


public abstract class Behaviour<T> : Node {
    private readonly T _owner;

    public Behaviour(T owner) {
        if (owner is not Node) {
            throw new ArgumentException("'owner' of Behaviour has to inherit from GameObject.");
        }
        _owner = owner;
    }

    public abstract void Execute(GameTime gameTime, T owner);
}


public class WanderBehaviour(float range, Entity owner) : Behaviour<Entity>(owner)
{
    private readonly float _range = range;
    private Vector2? _targetPosition;

    public override void Execute(GameTime gameTime, Entity owner) {
        if (_targetPosition is Vector2 target) {
            Vector2 delta = target - owner.Global_position;
        }

    }
}


public interface IState {
    public abstract void Update(GameTime gameTime, Node owner);
}


public class IdleState(Behaviour[] behaviours) : IState {
    private readonly Behaviour[] _behaviors = behaviours;

    public void Update(GameTime gameTime, Node owner) {
        foreach (Behaviour behaviour in _behaviors) {
            behaviour.Execute(gameTime, owner);
        }
    }
}


public class StateMachineProt(List<Node>? children=null) : Node(children) {
    private readonly List<IState> _state = [];

    public void AddState(IState state) {
        _state.Add(state);
    }

    public void RemoveState(IState state) {
        _state.Remove(state);
    }

    public new void Update(GameTime gameTime) {
        _state.ForEach(state => {
            if (Parent is not null) state.Update(gameTime, Parent);
        });
        base.Update(gameTime);
    }
}


public class Player(Entity entity) : IGameObject {
    public Entity Entity = entity;
    private readonly Dictionary<string, Action> _keyActionPairs = [];

    public void SetAction(string key, Action action) {
        _keyActionPairs.Remove(key);
        _keyActionPairs.Add(key, action);
    }

    public bool RemoveAction(string key) {
        return _keyActionPairs.Remove(key);
    }

    public bool RemoveAction(Action action) {
        if (!_keyActionPairs.ContainsValue(action)) return false;
        _keyActionPairs.Remove(_keyActionPairs.First(pair => pair.Value == action).Key);
        return true;
    }

    public void Draw(GameTime gameTime, ref SpriteBatch spriteBatch)
    {
        Entity.Draw(gameTime, ref spriteBatch);
    }

    public void Update(GameTime gameTime)
    {
        Entity.Update(gameTime);
    }
}


/*


Player {
    AnimatedSprite {
        IdleAnimation
        MoveAnimation
        RunAnimation
    }

    IdleBehaviour -> IdleAnim
    MoveBehaviour -> MoveAnim
    RunBehaviour -> RunAnim

    StateMachine {
        IdleState -> IdleBehaviour
        MoveState -> MoveBehaviour
        RunState -> RunBehaviour
    }
}


Entity player = new([
    "sprite": new AnimatedSprite([
        load("idle"),
        load("move"),
        load("run")
    ]),

    "idle": new Behaviour(parent => parent.children["sprite"].play("idle");),
    "move": new Behaviour(parent => {parent.children["sprite"].play("move"); parent.Position += bla bla bla; }),
    "run": new Behaviour(parent => {parent.children["sprite"].play("run"); parent.Position += bla bla bla * 2; }),

    "state": new StateMachine([
        "idle": new State("idle"),
        "move": new State("move"),
        "run": new State("run")
    ])
]);


*/
