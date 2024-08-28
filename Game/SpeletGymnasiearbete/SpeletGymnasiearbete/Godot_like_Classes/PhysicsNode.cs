using System.Collections.Generic;
using Microsoft.Xna.Framework;
using static SpeletGymnasiearbete.Node;
namespace SpeletGymnasiearbete;
#nullable enable

class PhysicsNode : Node2D
{
    private Vector2 _acceleraion;
    private Vector2 _velocity;
    private List<PhysicsNode> _collision_group = new();
    private System.Func<PhysicsNode, bool>? _CollidesWith;
    public System.Func<bool>? PhysicsCollisionBehaviour;

    public PhysicsNode (
        Vector2? position = null,
        float rotation = 0f,
        Vector2? scale = null,
        System.Func<PhysicsNode, bool>? CollisionLogic = null,
        System.Func<bool>? CollisionBehaviour = null,
        List<Node>? children = null
    ) : base(position, rotation, scale, children)
    {
        _CollidesWith = CollisionLogic;
        PhysicsCollisionBehaviour = CollisionBehaviour;
    }

    public new virtual void Update(GameTime gameTime)
    {
        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _velocity += _acceleraion * delta;
        Position += _velocity * delta;
        if (_CollidesWith is  null) { return; }

        foreach(PhysicsNode node in _collision_group)
        {
            if (_CollidesWith(node)) { continue; }
            if (PhysicsCollisionBehaviour is null)
            {
                Position -= _velocity * delta;
                _velocity *= -1;
                break;
            } else {
                PhysicsCollisionBehaviour();
                break;
            }
        }
    }

    public void AddForce(Vector2 force) { _acceleraion += force; }
}