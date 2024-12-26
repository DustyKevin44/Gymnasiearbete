using MonoGame.Extended;
using MonoGame.Extended.Shapes;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Game.Custom.Graphics.Procedural;


public abstract class ABone(Vector2 position)
{
    public readonly List<Bone> Bones = [];

    public Vector2 LocalPosition = position;
    public float LocalRotation = 0f; // -PI -> PI

    public Vector2 GlobalPosition { get => LocalPosition; set => LocalPosition = value; }
    public float GlobalRotation { get => LocalRotation; set => LocalRotation = value; }

    public void Update(GameTime gameTime) {
        foreach (var bone in Bones) {
            bone.Update(gameTime);
        }
    }

    public void Draw(SpriteBatch spriteBatch) {
        foreach (var bone in Bones) {
            bone.Draw(spriteBatch);
        }
    }

    public void SolveIK(Vector2 target, Bone endEffector) {
        IKForward(target, endEffector);
        IKBackward();
    }
    
    public void IKBackward() {
        foreach (var bone in Bones) {
            var angle = (GlobalPosition - bone.GlobalPosition).ToAngle();
            bone.GlobalRotation = angle;
            bone.GlobalPosition = (this is Bone thisBone) ? thisBone.GlobalPosition : GlobalPosition;
            bone.IKBackward(); // Propegate
        }
    }

    public void IKForward(Vector2 target, Bone endEffector) {
        int limit = 1000;
        int i = 0;
        while (Vector2.DistanceSquared(target, endEffector.GlobalPosition) > 1 && ++i < limit) {
            ABone cur = endEffector;
            Vector2 t = target;
            while (cur is Bone bone) {
                if (bone.TipGlobalPosition == t) {
                    t = bone.GlobalPosition;
                    cur = bone.Parent;
                    continue;
                }
                var angle = (t - bone.GlobalPosition).ToAngle();
                bone.GlobalRotation = angle; // TODO: Apply constraints
                bone.TipGlobalPosition = t;
                t = bone.GlobalPosition;
                cur = bone.Parent;
            }
        }
    }
}

// Works as the root bone that all other bones in the "skeleton" are attached to, should be thought as the main body and the bones make up the limbs and appendages of the skeleton
public class Skeleton(Vector2 position) : ABone(position)
{
    private Polygon _polygon = new([Vector2.Zero, new Vector2(10, 10), Vector2.UnitY * -10, new Vector2(-10, 10), Vector2.Zero]);

    public Skeleton AttachBone(Bone bone) {
        Bones.Add(bone);
        bone.Parent = this;
        return this;
    }

    public new void Draw(SpriteBatch spriteBatch) {
        spriteBatch.DrawPolygon(GlobalPosition, _polygon.TransformedCopy(Vector2.Zero, GlobalRotation, Vector2.One), Color.White, 2f);
        base.Draw(spriteBatch);
    }
}


public class Bone(Vector2 position, float length, double minimumAngle, double maximumAngle) : ABone(position)
{
    public ABone Parent { get; set; }
    // Properties
    public new Vector2 GlobalPosition { get => LocalPosition + Parent.GlobalPosition; set => LocalPosition = value - Parent.GlobalPosition; }
    public new float GlobalRotation { get => LocalRotation + Parent.GlobalRotation; set => LocalRotation = value - Parent.GlobalRotation; }

    public Vector2 TipLocalPosition { get => LocalPosition + Length * new Vector2(MathF.Cos(GlobalRotation), MathF.Sin(GlobalRotation)); set => LocalPosition = value + LocalPosition - TipLocalPosition; }
    public Vector2 TipGlobalPosition { get => GlobalPosition + Length * new Vector2(MathF.Cos(GlobalRotation), MathF.Sin(GlobalRotation)); set => GlobalPosition = value + GlobalPosition - TipGlobalPosition; }

    public float Length { get; set; } = length;
    public float MinAngle { get; set; } = MathHelper.WrapAngle((float)minimumAngle) + MathF.PI;
    public float MaxAngle { get; set; } = MathHelper.WrapAngle((float)maximumAngle) + MathF.PI;

    public Bone AttachBone(Bone bone) {
        Bones.Add(bone);
        bone.Parent = this;
        return this;
    }

    public new void Update(GameTime gameTime) {
        LocalRotation = MathHelper.Clamp(MathHelper.WrapAngle(LocalRotation) + MathF.PI, MinAngle + 0.01f, MaxAngle - 0.01f);
    }

    public new void Draw(SpriteBatch spriteBatch) {
        spriteBatch.DrawLine(GlobalPosition, TipGlobalPosition, Color.White, 2f);
        Debug.DrawArc(spriteBatch, GlobalPosition, 20f, 10, MinAngle, MaxAngle, Color.Red);
        base.Draw(spriteBatch);
    }
}
