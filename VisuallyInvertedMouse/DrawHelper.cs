using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace VisuallyInvertedMouse
{
    public class DrawHelper
    {
        private SpriteBatch spriteBatch;
        private GraphicsDevice graphicsDevice;
        private Texture2D pixel;

        // Constructor: needs a SpriteBatch and a 1x1 white pixel texture
        public DrawHelper(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            this.spriteBatch = spriteBatch;
            this.graphicsDevice = graphicsDevice;


            // Create a 1x1 white pixel if not provided
            pixel = new Texture2D(graphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
        }

        /// <summary>
        /// Draw a line using Bresenham's algorithm
        /// </summary>
        public void DrawLine(int x1, int y1, int x2, int y2, Color color)
        {
            int dx = Math.Abs(x2 - x1);
            int dy = Math.Abs(y2 - y1);
            int stepX = x1 < x2 ? 1 : -1;
            int stepY = y1 < y2 ? 1 : -1;
            int error = dx - dy;

            while (true)
            {
                spriteBatch.Draw(pixel, new Rectangle(x1, y1, 1, 1), color);

                if (x1 == x2 && y1 == y2) break;

                int e2 = 2 * error;
                if (e2 > -dy)
                {
                    error -= dy;
                    x1 += stepX;
                }
                if (e2 < dx)
                {
                    error += dx;
                    y1 += stepY;
                }
            }
        }

        /// <summary>
        /// Draw a circle using connected line segments
        /// </summary>
        public void DrawCircle(Vector2 center, int radius, Color color, int segments = 360)
        {
            float angleStep = (float)(2 * Math.PI / segments);

            for (int i = 0; i < segments; i++)
            {
                float angle1 = i * angleStep;
                float angle2 = (i + 1) * angleStep;

                float x1 = center.X + radius * (float)Math.Cos(angle1);
                float y1 = center.Y + radius * (float)Math.Sin(angle1);
                float x2 = center.X + radius * (float)Math.Cos(angle2);
                float y2 = center.Y + radius * (float)Math.Sin(angle2);

                DrawLine((int)x1, (int)y1, (int)x2, (int)y2, color);
            }
        }
    }
}