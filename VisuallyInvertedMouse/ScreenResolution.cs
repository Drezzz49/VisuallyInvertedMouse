using Microsoft.Xna.Framework.Graphics;

namespace VisuallyInvertedMouse
{
    public class ScreenResolution
    {
        private GraphicsDevice graphicsDevice;

        public ScreenResolution(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
        }

        /// <summary>
        /// Returns the width of the current display in pixels
        /// </summary>
        public int MaxX
        {
            get
            {
                return GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            }
        }

        /// <summary>
        /// Returns the height of the current display in pixels
        /// </summary>
        public int MaxY
        {
            get
            {
                return GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            }
        }

        /// <summary>
        /// Returns the current resolution as a Vector2
        /// </summary>
        public Microsoft.Xna.Framework.Vector2 Resolution
        {
            get
            {
                return new Microsoft.Xna.Framework.Vector2(MaxX, MaxY);
            }
        }
    }
}