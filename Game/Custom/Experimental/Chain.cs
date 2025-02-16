using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System;
using MonoGame.Extended;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Custom.Experimental;


public class Joint(Vector2 position, float lenght, float rotation, float minRotation=-MathF.PI, float maxRotation=MathF.PI)
{
    public Vector2 Position = position;
    public float Length = lenght;
    public float Rotation = rotation;
    public float MinRotation = minRotation;
    public float MaxRotation = maxRotation;
}

public class Chain(List<Joint> joints)
{
    public readonly List<Joint> Joints = (joints.Count > 0) ? joints : throw new Exception("Can not create a chain with 0 joints.");
    
    public Joint Root => Joints.First();

    public void Update(GameTime gameTime)
    {
        for (int i = 0; i < Joints.Count - 1; i++)
        {
            Joint alpha = Joints[i];
            Joint beta = Joints[i + 1];

            if (Vector2.DistanceSquared(beta.Position, alpha.Position) == alpha.Length * alpha.Length)
                continue;

            // Calculate new position for Joint
            var delta = beta.Position - alpha.Position;
            
            Vector2 direction;
            if (delta == Vector2.Zero)
                new Random().NextUnitVector(out direction);
            else direction = delta.NormalizedCopy();

            beta.Position = alpha.Position + direction * alpha.Length;

            // Get relative rotation contraints
            var relMaxRotation = ((i == 0) ? 0f : Joints[i - 1].Rotation) + alpha.MaxRotation;
            var relMinRotation = ((i == 0) ? 0f : Joints[i - 1].Rotation) + alpha.MinRotation;

            // Get true rotation
            alpha.Rotation = (float)Math.Atan2(direction.Y, direction.X);
            var newRotation = MathHelper.Clamp(alpha.Rotation, relMinRotation, relMaxRotation);

            if (MathF.Abs(GetDeltaAngle(alpha.Rotation, newRotation)) > 0.01)
            {
                newRotation = (float)(alpha.Rotation + MathF.Sign(newRotation - alpha.Rotation) * 0.01);
            }

            // Apply rotation contraints to position
            beta.Position.RotateAround(alpha.Position, newRotation - alpha.Rotation);
            alpha.Rotation = newRotation;
        }
    }

    public static float NormalizeAngle(float angle)
    {
        while (angle > MathF.PI) angle -= 2 * MathF.PI;
        while (angle < -MathF.PI) angle += 2 * MathF.PI;
        return angle;
    }

    public static float GetDeltaAngle(float from, float to)
    {
        float delta = NormalizeAngle(to) - NormalizeAngle(from);
        if (delta > MathF.PI) delta -= 2 * MathF.PI;
        if (delta < -MathF.PI) delta += 2 * MathF.PI;
        return delta;
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        foreach (var joint in Joints)
        {
            var endPosition = joint.Position + new Vector2(MathF.Cos(joint.Rotation), MathF.Sin(joint.Rotation)) * joint.Length;
            spriteBatch.DrawLine(joint.Position, endPosition, Color.Black, 2f);
            spriteBatch.DrawCircle(joint.Position, 5f, 32, Color.Red, 2f);
        }
    }
}
