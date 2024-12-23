using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Custom.Graphics;

static class Utils {
    public static void DrawPixelPerfectLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Texture2D texture, int size, Color color)
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

        while (true)
        {
            // Draw the current pixel
            spriteBatch.Draw(texture, new Rectangle(x0 * size, y0 * size, size, size), color);

            // Check if we've reached the endpoint
            if (x0 == x1 && y0 == y1)
                break;

            // Calculate the next pixel
            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }
    }
}