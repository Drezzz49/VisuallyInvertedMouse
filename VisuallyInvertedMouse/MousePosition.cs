using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace VisuallyInvertedMouse
{
    public class MousePosition
    {
        /// <summary>
        /// Gets the current mouse position as a Vector2
        /// </summary>
        public static Vector2 Position
        {
            get
            {
                MouseState state = Mouse.GetState();
                return new Vector2(state.X, state.Y);
            }
        }

        /// <summary>
        /// Gets the X coordinate of the mouse
        /// </summary>
        public static int X
        {
            get
            {
                return Mouse.GetState().X;
            }
        }

        /// <summary>
        /// Gets the Y coordinate of the mouse
        /// </summary>
        public static int Y
        {
            get
            {
                return Mouse.GetState().Y;
            }
        }

        /// <summary>
        /// Checks if a mouse button is pressed
        /// </summary>
        public static bool IsLeftButtonDown()
        {
            return Mouse.GetState().LeftButton == ButtonState.Pressed;
        }

        public static bool IsRightButtonDown()
        {
            return Mouse.GetState().RightButton == ButtonState.Pressed;
        }
    }
}