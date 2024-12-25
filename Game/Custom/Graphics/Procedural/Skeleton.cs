using MonoGame.Extended;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Game.Custom.Graphics.Procedural;


public abstract class ABone
{
    public List<Bone> Bones { get; private set; }
    public Transform2 LocalTransform { get; set; }

    public ABone(Transform2 transform, List<Bone> bones = null) {
        LocalTransform = transform;
        Bones = bones ?? [];
        foreach (var bone in Bones) { bone.Parent = this; }
    }

    public void Update() {
        foreach (var bone in Bones) {
            bone.Update();
        }
    }

    public void Draw(SpriteBatch spriteBatch) {
        foreach (var bone in Bones) {
            bone.Draw(spriteBatch);
        }
    }
}

// Works as the root bone that all other bones in the "skeleton" are attached to, should be thought as the main body and the bones make up the limbs and appendages of the skeleton
public class Skeleton(Transform2 transform, List<Bone> bones = null) : ABone(transform, bones);


public class Bone : ABone
{
    public List<IConstraint> Constraints { get; private set; }
    public ABone Parent { get; set; }

    public Bone(Transform2 transform, List<IConstraint> constraints = null, List<Bone> bones = null) : base(transform, bones) {
        Constraints = constraints ?? [];
        foreach (var constraint in Constraints) {
            constraint.Owner = this;
        }
    }

    public new void Update() {
        foreach (var constraint in Constraints) {
            constraint.Update(Parent);
        }
        base.Update();
    }

    public new void Draw(SpriteBatch spriteBatch) {
        foreach (var constraint in Constraints) {
            constraint.Draw(Parent, spriteBatch);
        }
        base.Draw(spriteBatch);
    }
}


public interface IConstraint {
    public Bone Owner { get; set; }
    public void Update(ABone target);
    public void Draw(ABone target, SpriteBatch spriteBatch);
}


public abstract class BaseConstraint {
    public Bone Owner { get; set; }
}


public class DistanceConstraint(float length) : BaseConstraint, IConstraint
{
    public float Length { get; set; } = length;

    public void Draw(ABone target, SpriteBatch spriteBatch) {
        spriteBatch.DrawLine(Owner.LocalTransform.Position, target.LocalTransform.Position, Color.Red, 2f);
    }

    public void Update(ABone target) {
        var dir = Owner.LocalTransform.Position - target.LocalTransform.Position;
        dir.Normalize();

        Owner.LocalTransform.Position = target.LocalTransform.Position + dir * Length;
    }
}


public class AngleConstraint(double minAngle, double maxAngle) : BaseConstraint, IConstraint
{
    public float MinAngle { get; set; } = (float)minAngle;
    public float MaxAngle { get; set; } = (float)maxAngle;

    public void Draw(ABone target, SpriteBatch spriteBatch) {
        Drawing.Arc(spriteBatch, Owner.LocalTransform.Position, 20f, MinAngle, MaxAngle, 32, Color.Red);
    }

    public void Update(ABone target) {}
}

