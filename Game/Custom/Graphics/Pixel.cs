using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Custom.Graphics;

static class Pixel {
    public static Vector2 SnapToPixelGrid(Vector2 position, int pixelSize) {
        float snappedX = (float)Math.Floor(position.X / pixelSize) * pixelSize;
        float snappedY = (float)Math.Floor(position.Y / pixelSize) * pixelSize;

        return new Vector2(snappedX, snappedY);
    }

    public static void DrawPerfectLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Texture2D texture, int size, Color color)
    {
        start /= size;
        end /= size;

        int x0 = (int)start.X;
        int y0 = (int)start.Y;
        int x1 = (int)end.X;
        int y1 = (int)end.Y;

        int dx = Math.Abs(x1 - x0);
        int dy = Math.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        int maxSteps = dx + dy + 1000; // Safety limit
        int steps = 0;
        while (true)
        {
            spriteBatch.Draw(texture, new Rectangle(x0 * size, y0 * size, size, size), color);

            if (x0 == x1 && y0 == y1)
                break;

            int e2 = 2 * err;
            if (e2 > -dy) { err -= dy; x0 += sx; }
            if (e2 < dx) { err += dx; y0 += sy; }

            if (++steps > maxSteps) 
            {
                throw new InvalidOperationException("Exceeded maximum steps in DrawPixelPerfectLine.");
            }
        }
    }
}