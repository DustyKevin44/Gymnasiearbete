using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;

namespace Game.Custom.Components.Systems;


public class ColliderSystem : EntityUpdateSystem
{
    private ComponentMapper<HurtBox> _hurtBoxMapper;
    private ComponentMapper<HitBox> _hitBoxMapper;
    private ComponentMapper<CollisionBox> _collisionBoxMapper;
    private ComponentMapper<Transform2> _transformMapper;

    public ColliderSystem() : base(Aspect.All(typeof(Transform2)).One(typeof(HurtBox), typeof(HitBox), typeof(CollisionBox))) { }

    public override void Initialize(IComponentMapperService mapperService)
    {
        _hitBoxMapper = mapperService.GetMapper<HitBox>();
        _hurtBoxMapper = mapperService.GetMapper<HurtBox>();
        _collisionBoxMapper = mapperService.GetMapper<CollisionBox>();
        _transformMapper = mapperService.GetMapper<Transform2>();
    }

    public override void Update(GameTime gameTime)
    {
        foreach (int entity in ActiveEntities)
        {
            var entityTransform = _transformMapper.Get(entity);

            if (_hitBoxMapper.Has(entity))
            {
                var hitBox = _hitBoxMapper.Get(entity);

                // Offset local position to global position
                hitBox.Shape.Position += entityTransform.Position;

                foreach (int other in ActiveEntities)
                {
                    if (!_hurtBoxMapper.Has(other))
                        continue;

                    var otherTransform = _transformMapper.Get(other);
                    var hurtBox = _hurtBoxMapper.Get(other);

                    // Offset to global for other entity
                    hurtBox.Shape.Position += otherTransform.Position;

                    if (hitBox.Shape.Intersects(hurtBox.Shape))
                    {
                        // Idk
                    }

                    // Offset back to local for other entity
                    hurtBox.Shape.Position -= otherTransform.Position;
                }
                
                // Offset back to local position
                hitBox.Shape.Position -= entityTransform.Position;
            }
            if (_collisionBoxMapper.Has(entity))
            {
                var entityCollisionBox = _collisionBoxMapper.Get(entity);
                
                // Offset local collision shape by entity position
                entityCollisionBox.Shape.Position += entityTransform.Position;

                foreach (int other in ActiveEntities)
                {
                    if (!_collisionBoxMapper.Has(other))
                        continue;

                    var otherTransform = _transformMapper.Get(other);
                    var otherCollisionBox = _collisionBoxMapper.Get(other);

                    if (entityCollisionBox.IsStatic && otherCollisionBox.IsStatic)
                        continue;

                    // Offset local collision shape by other entity position
                    otherCollisionBox.Shape.Position += otherTransform.Position;

                    if (entityCollisionBox.Shape.Intersects(otherCollisionBox.Shape))
                    {
                        var entityRect = entityCollisionBox.Shape.BoundingRectangle;
                        var otherRect = otherCollisionBox.Shape.BoundingRectangle;

                        var entityCenter = new Vector2(entityRect.Center.X, entityRect.Center.Y);
                        var otherCenter = new Vector2(otherRect.Center.X, otherRect.Center.Y);

                        var separationVector = entityCenter - otherCenter;

                        if (separationVector.LengthSquared() > 0)
                        {
                            separationVector.Normalize();

                            float overlapX = Math.Min(entityRect.Right, otherRect.Right) - Math.Max(entityRect.Left, otherRect.Left);
                            float overlapY = Math.Min(entityRect.Bottom, otherRect.Bottom) - Math.Max(entityRect.Top, otherRect.Top);
                            float penetrationDepth = Math.Min(overlapX, overlapY) + 0.1f; // Add small buffer to prevent re-collision

                            if (entityCollisionBox.IsStatic)
                            {
                                otherTransform.Position -= separationVector * penetrationDepth;
                                otherCollisionBox.Shape.Position = otherTransform.Position; // Update collision box immediately
                            }
                            else if (otherCollisionBox.IsStatic)
                            {
                                entityTransform.Position += separationVector * penetrationDepth;
                                entityCollisionBox.Shape.Position = entityTransform.Position; // Update collision box immediately
                            }
                            else
                            {
                                entityTransform.Position += separationVector * (penetrationDepth / 2);
                                otherTransform.Position -= separationVector * (penetrationDepth / 2);

                                entityCollisionBox.Shape.Position = entityTransform.Position; // Sync collision box
                                otherCollisionBox.Shape.Position = otherTransform.Position; // Sync collision box
                            }
                        }
                    }

                    // Reset offset
                    otherCollisionBox.Shape.Position -= otherTransform.Position;
                }
                // Reset offset
                entityCollisionBox.Shape.Position -= entityTransform.Position;
            }
        }
    }
}