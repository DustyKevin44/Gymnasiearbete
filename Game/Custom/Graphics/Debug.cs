using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;

namespace Game.Custom.Graphics;


public static class Debug
{
    public static void DrawArc(SpriteBatch spriteBatch, Vector2 center, float radius, int segments, float start, float end, Color color, float thickness = 2f)
    {
        float angleStep = (end - start) / segments;

        Vector2 previousPoint = center + new Vector2(
            radius * (float)Math.Cos(start),
            radius * (float)Math.Sin(start)
        );

        for (int i = 1; i <= segments; i++)
        {
            float currentAngle = start + angleStep * i;

            Vector2 currentPoint = center + new Vector2(
                radius * (float)Math.Cos(currentAngle),
                radius * (float)Math.Sin(currentAngle)
            );

            spriteBatch.DrawLine(previousPoint, currentPoint, color, thickness);

            previousPoint = currentPoint;
        }
    }
}
