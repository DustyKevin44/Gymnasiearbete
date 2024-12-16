using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable enable
namespace SpeletGymnasiearbete.Classes;

public class GameObject {
    private List<GameObject> _children = [];
    protected GameObject? parent;

    public ref List<GameObject> GetChildren() {
        return ref _children;
    }

    public void AddChild(GameObject child) {
        child.parent = this;
        _children.Add(child);
    }

    public void RemoveChild(GameObject child) {
        child.parent = null;  // Change to global root
        _children.Remove(child);
    }

    public void Update(GameTime gameTime) {
        _children.ForEach(child => {
            child.Update(gameTime);
        });
    }

    public void Draw(GameTime gameTime, ref SpriteBatch spriteBatch) {
        foreach (GameObject child in _children) {
            child.Draw(gameTime, ref spriteBatch);
        }
    }
}


public class Entity : GameObject {
    public Vector2 Local_position;
    public Vector2 Global_position {
        get {
            if (parent is Entity entityParent) return entityParent.Global_position + Local_position;
            else return Local_position;
        }
        set {
            if (parent is Entity entityParent) Local_position = value - entityParent.Global_position;
            else Local_position = value;
        }
    }
}


public class Sprite2D(Texture2D texture) : Entity {
    public Texture2D Texture = texture;
}


// Components


public abstract class AbstractComponent : GameObject;


public class ScriptComponent(Action<GameTime, GameObject> script) : AbstractComponent {
    private readonly Action<GameTime, GameObject> _script = script;

    public new void Update(GameTime gameTime) {
        if (parent is not null) _script.Invoke(gameTime, parent);
    }
}


// Behaviour


/*

Entity player = new(Vector2.Zero);
player.AddChild(new Sprite(Vector2.Zero, texture));

AnimatedBehaviour bobbingBehaviour = new(BobbingAnimation);
player.AddChild(bobbingBehaviour);

FollowPositionBehaviour follow_mouse = new(() => Mouse.get_position() + global.camera.position; );
player.AddChild(followMouseBhaviour);

StateMachine statemachine = new();
statemachine.AddState(new IdleState(bobbingBehaviour));
statemachine.AddState(new FollowState(followMouseBehaviour));
player.AddChild(statemachine);


*/

public abstract class Behaviour(GameObject owner) : GameObject {
    private readonly GameObject _owner = owner;

    public abstract void Execute(GameTime gameTime, GameObject owner);
}

public abstract class Behaviour<T> : GameObject {
    private readonly T _owner;

    public Behaviour(T owner) {
        if (owner is not GameObject) {
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


// State

public interface IState {
    public abstract void Update(GameTime gameTime, GameObject owner);
}


public class IdleState(Behaviour[] behaviours) : IState {
    private readonly Behaviour[] _behaviors = behaviours;

    public void Update(GameTime gameTime, GameObject owner) {
        foreach (Behaviour behaviour in _behaviors) {
            behaviour.Execute(gameTime, owner);
        }
    }
}


public class StateMachineProt : AbstractComponent {
    private readonly List<IState> _state = [];

    public void AddState(IState state) {
        _state.Add(state);
    }

    public void RemoveState(IState state) {
        _state.Remove(state);
    }

    public new void Update(GameTime gameTime) {
        _state.ForEach(state => {
            if (parent is not null) state.Update(gameTime, parent);
        });
        base.Update(gameTime);
    }
}
