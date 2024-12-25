using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;

namespace Game.Custom.Graphics;


public static class Drawing
{
    public static void Arc(SpriteBatch spriteBatch, Vector2 center, float radius, float startAngle, float endAngle, int segments, Color color, float thickness = 2f)
    {
        float angleStep = (endAngle - startAngle) / segments;

        Vector2 prevPoint = center + new Vector2(
            radius * (float)Math.Cos(startAngle),
            radius * (float)Math.Sin(endAngle)
        );

        for (int i = 1; i <= segments; i++)
        {
            float angle = startAngle + i * angleStep;

            Vector2 currentPoint = center + new Vector2(
                radius * (float)Math.Cos(angle),
                radius * (float)Math.Sin(angle)
            );

            spriteBatch.DrawLine(prevPoint, currentPoint, color, thickness);

            prevPoint = currentPoint;
        }
    }
}