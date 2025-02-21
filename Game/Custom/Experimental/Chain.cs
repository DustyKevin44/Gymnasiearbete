using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System;
using MonoGame.Extended;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Custom.Experimental
{
    public class Joint(Vector2 position, float length, float rotation, float minRotation = -MathF.PI, float maxRotation = MathF.PI)
    {
        public Vector2 Position = position;
        public float Length = length;
        public float Rotation = rotation;
        public float MinRotation = minRotation;
        public float MaxRotation = maxRotation;
    }

    public class Chain
    {
        public readonly List<Joint> Joints;
        public Vector2 Anchor;
        public Vector2 Target;
        public int Iterations = 8;

        public Chain(Vector2 anchor, Vector2 target, List<Joint> joints)
        {
            if (joints.Count == 0)
                throw new Exception("Cannot create a chain with 0 joints.");
            
            Joints = joints;
            Anchor = anchor;
            Target = target;
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < Iterations; i++)
            {
                FabrikF();
                FabrikB();
            }
        }

        public void FabrikF()
        {
            Joint endJoint = Joints[^1];
            endJoint.Position = Target;

            for (int i = Joints.Count - 2; i >= 0; i--)
            {
                Joint beta = Joints[i + 1];
                Joint alpha = Joints[i];
                
                var delta = alpha.Position - beta.Position;
                Vector2 direction = delta == Vector2.Zero ? Vector2.UnitX : delta.NormalizedCopy();
                
                float targetRotation = MathF.Atan2(direction.Y, direction.X);
                float childRotation = (i == Joints.Count - 2) ? targetRotation : Joints[i + 1].Rotation;
                
                float deltaRotation = targetRotation - childRotation;
                deltaRotation = MathHelper.WrapAngle(deltaRotation);
                
                float constrainedRotation = MathHelper.Clamp(deltaRotation, beta.MinRotation, beta.MaxRotation) + childRotation;
                beta.Rotation = constrainedRotation;
                
                alpha.Position = beta.Position + new Vector2(MathF.Cos(constrainedRotation), MathF.Sin(constrainedRotation)) * beta.Length;
            }
        }

        public void FabrikB()
        {
            Joints[0].Position = Anchor;
            
            for (int i = 0; i < Joints.Count - 1; i++)
            {
                Joint alpha = Joints[i];
                Joint beta = Joints[i + 1];

                var delta = beta.Position - alpha.Position;
                Vector2 direction = delta == Vector2.Zero ? Vector2.UnitX : delta.NormalizedCopy();
                
                float targetRotation = MathF.Atan2(direction.Y, direction.X);
                float parentRotation = (i == 0) ? 0f : Joints[i - 1].Rotation;
                
                float deltaRotation = targetRotation - parentRotation;
                deltaRotation = MathHelper.WrapAngle(deltaRotation);
                
                float constrainedRotation = MathHelper.Clamp(deltaRotation, alpha.MinRotation, alpha.MaxRotation) + parentRotation;
                alpha.Rotation = constrainedRotation;
                
                beta.Position = alpha.Position + new Vector2(MathF.Cos(constrainedRotation), MathF.Sin(constrainedRotation)) * alpha.Length;
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var joint in Joints.SkipLast(1))
            {
                var endPosition = joint.Position + new Vector2(MathF.Cos(joint.Rotation), MathF.Sin(joint.Rotation)) * joint.Length;
                spriteBatch.DrawLine(joint.Position, endPosition, Color.Black, 2f);
                spriteBatch.DrawCircle(joint.Position, 5f, 32, Color.Red, 2f);
            }
            spriteBatch.DrawCircle(Joints.Last().Position, 5f, 32, Color.Red, 2f);
        }
    }
}
