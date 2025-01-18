using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Game.Custom.Graphics;

namespace Game.Custom.Tilemap;

public class Hero(Texture2D tex, Vector2 pos) : Sprite(tex, pos)
{
    public Vector2 DestinationPosition { get; protected set; } = pos;
    public bool MoveDone { get; protected set; } = true;
    protected float speed = 600;
    public List<Vector2> Path { get; private set; }
    private int _current;

    public void SetPath(List<Vector2> path)
    {
        if (path is null) return;
        if (path.Count < 1) return;

        Path = path;
        _current = 0;
        DestinationPosition = Path[_current];
        MoveDone = false;
    }

    private bool NearDestination()
    {
        if ((DestinationPosition - Position).Length() < 5)
        {
            Position = DestinationPosition;

            if (_current < Path.Count - 1)
            {
                _current++;
                DestinationPosition = Path[_current];
            }
            else
            {
                MoveDone = true;
            }
            return true;
        }
        return false;
    }

    public void Update(GameTime gameTime)
    {
        if (MoveDone) return;

        var direction = DestinationPosition - Position;
        if (direction != Vector2.Zero) direction.Normalize();

        var distance = gameTime.GetElapsedSeconds() * speed;
        int iterations = (int)Math.Ceiling(distance / 5);
        distance /= iterations;

        for (int i = 0; i < iterations; i++)
        {
            Position += direction * distance;
            if (NearDestination()) return;
        }
    }
}