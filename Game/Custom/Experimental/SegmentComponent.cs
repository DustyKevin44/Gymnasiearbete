using System;
using Game.Custom.Utilities;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;

namespace Game.Custom.Experimental;


public class SegmentComponent(Entity parentSegment, float length, float maxAngle)
{
    public Entity ParentSegment = parentSegment;
    public float Length = length;
    public float MaxAngle = maxAngle;
}


public class DirectionComponent(float direction)
{
    public float Direction = direction;
}


public class SegmentSystem() : EntityUpdateSystem(Aspect.All(typeof(SegmentComponent), typeof(Transform2)))
{
    private ComponentMapper<Transform2> _transformMapper;
    private ComponentMapper<SegmentComponent> _segmentComponentMapper;
    private ComponentMapper<DirectionComponent> _directionMapper;

    public override void Initialize(IComponentMapperService mapperService)
    {
        _transformMapper = mapperService.GetMapper<Transform2>();
        _segmentComponentMapper = mapperService.GetMapper<SegmentComponent>();
        _directionMapper = mapperService.GetMapper<DirectionComponent>();
    }

    public override void Update(GameTime gameTime)
    {
        foreach (var entity in ActiveEntities)
        {
            var segment = _segmentComponentMapper.Get(entity);
            var transform = _transformMapper.Get(entity);
            var direction = _directionMapper.Get(entity);

            if (segment.ParentSegment != null &&
                segment.ParentSegment.Has<Transform2>() &&
                segment.ParentSegment.Has<DirectionComponent>())
            {
                var parentTransform = segment.ParentSegment.Get<Transform2>();
                var parentDirection = segment.ParentSegment.Get<DirectionComponent>();

                Vector2 delta = transform.Position - parentTransform.Position;
                float angleToParent = (float)Math.Atan2(delta.Y, delta.X);

                float deltaAngle = MathHelper.WrapAngle(angleToParent - parentDirection.Direction);
                float clampedAngle = MathHelper.Clamp(deltaAngle, -segment.MaxAngle, segment.MaxAngle);

                float finalAngle = parentDirection.Direction + clampedAngle;
                direction.Direction = finalAngle;

                transform.Position = parentTransform.Position + Utils.FromAngle(finalAngle) * segment.Length;
            }
        }
    }
}
