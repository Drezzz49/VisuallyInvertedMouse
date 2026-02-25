using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace VisuallyInvertedMouse
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        SpriteFont font;
        private DrawHelper drawHelper;
        private ScreenResolution screenResolution;

        int circleRadius = 50;
        Vector2 oppositePoiont;
        Vector2 normalizedOppositePoint;
        Vector2 midPoint;
        bool isOverlayVisible = true;
        float timeSinceToggle = 0f;
        float timeSinceToggleClick = 0f;
        bool isZPressed = false;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            this.InactiveSleepTime = new TimeSpan(0);
            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 240.0); // 240 FPS


            graphics.PreferredBackBufferHeight = 1080;
            graphics.PreferredBackBufferWidth = 1920;
            graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            TransparentWindow();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            drawHelper = new DrawHelper(spriteBatch, GraphicsDevice);
            screenResolution = new ScreenResolution(GraphicsDevice);
            font = Content.Load<SpriteFont>("font");

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.F8))
                Exit();

            timeSinceToggle += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Keyboard.GetState().IsKeyDown(Keys.F7) && timeSinceToggle >= 0.3f) //0.3s delay
            {
                isOverlayVisible = !isOverlayVisible;
                timeSinceToggle = 0;
            }

            midPoint = new Vector2(screenResolution.MaxX / 2, screenResolution.MaxY / 2 - 50);
            //find the opposite point of the mouse position to the center of the screen
            oppositePoiont = midPoint + (midPoint - MousePosition.Position);

            //normalize the opposite point so it's in the right direction but length of 1
            normalizedOppositePoint = Vector2.Normalize(oppositePoiont - midPoint);
            normalizedOppositePoint = midPoint + (normalizedOppositePoint * 150);

            //find the distance between the mouse and the center of the screen to set the radius of the circle
            circleRadius = (int)Vector2.Distance(MousePosition.Position, midPoint);


            const int VK_Z = 0x5A;
            bool isCurrentlyDown = (GetAsyncKeyState(VK_Z) & 0x8000) != 0;
            timeSinceToggleClick += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (isCurrentlyDown)
            {
                isZPressed = true;
                if (timeSinceToggleClick >= 0.1f)
                {
                    timeSinceToggleClick = 0f;

                    // Använd Win32 direkt för att hämta positionen (snabbare än Cursor.Position)
                    GetCursorPos(out NativePoint lpPoint);

                    // Sekvensen sker nu utan paus
                    MouseHelper.SetMousePosition((int)normalizedOppositePoint.X, (int)normalizedOppositePoint.Y);
                    MouseHelper.SetMousePosition((int)normalizedOppositePoint.X, (int)normalizedOppositePoint.Y);
                    MouseHelper.SetMousePosition((int)normalizedOppositePoint.X, (int)normalizedOppositePoint.Y);
                    MouseHelper.mouse_event(0x02, 0, 0, 0, 0); // Down
                    MouseHelper.mouse_event(0x02, 0, 0, 0, 0); // Down
                    MouseHelper.mouse_event(0x02, 0, 0, 0, 0); // Down
                    Thread.Sleep(5);
                    MouseHelper.mouse_event(0x04, 0, 0, 0, 0); // Up
                    MouseHelper.mouse_event(0x04, 0, 0, 0, 0); // Up
                    MouseHelper.mouse_event(0x04, 0, 0, 0, 0); // Up
                    MouseHelper.SetMousePosition(lpPoint.X, lpPoint.Y);
                    MouseHelper.SetMousePosition(lpPoint.X, lpPoint.Y);
                    MouseHelper.SetMousePosition(lpPoint.X, lpPoint.Y);
                    MouseHelper.SetMousePosition(lpPoint.X, lpPoint.Y);
                }
            }
            else if (isZPressed)
            {
                isZPressed = false;

                GetCursorPos(out NativePoint lpPoint);

                // Flytta till mitten, tryck 3, flytta tillbaka
                MouseHelper.SetMousePosition((int)midPoint.X, (int)midPoint.Y);

                // Skicka tangent '3'
                keybd_event(VK_3, 0, 0, 0); // Down
                Thread.Sleep(5);
                keybd_event(VK_3, 0, KEYEVENTF_KEYUP, 0); // Up

                MouseHelper.SetMousePosition(lpPoint.X, lpPoint.Y);
            }


            base.Update(gameTime);
        }

        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int vKey);

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();


            if (isOverlayVisible)
            {
                spriteBatch.DrawString(font, "Close application with f8 and hide/un-hide overlay with f7", new Vector2(200, 140), Color.Yellow);
                spriteBatch.DrawString(font, $"Mouse Position: X:{MousePosition.X} Y:{MousePosition.Y}", new Vector2(200, 160), Color.LightSteelBlue); //Text
                spriteBatch.DrawString(font, $"Desired-Mouse Position: X:{oppositePoiont.X} Y:{oppositePoiont.Y}", new Vector2(200, 180), Color.LightSteelBlue); //Text
                spriteBatch.DrawString(font, $"Circle Radius:{circleRadius}", new Vector2(200, 200), Color.LightSteelBlue); //Text

                //circles
                drawHelper.DrawCircle(midPoint, circleRadius, Color.Red, 360); //tghe big circle
                drawHelper.DrawCircle(midPoint, (int)MathF.Floor(Vector2.Distance(normalizedOppositePoint, midPoint)), Color.Red, 360); //the middle circle

                //lines
                drawHelper.DrawLine((int)midPoint.X, (int)midPoint.Y, (int)MousePosition.X, (int)MousePosition.Y, Color.Green); //line
                drawHelper.DrawLine((int)midPoint.X, (int)midPoint.Y, (int)oppositePoiont.X, (int)oppositePoiont.Y, Color.Blue); //opposite line

                //points
                drawHelper.DrawPoint(midPoint, 3, Color.LightCoral); //middle point
                drawHelper.DrawPoint(oppositePoiont, 3, Color.LightCoral); //opposite point
                drawHelper.DrawPoint(normalizedOppositePoint, 3, Color.LightBlue);
                drawHelper.DrawPoint(new Vector2((int)MousePosition.X, (int)MousePosition.Y), 3, Color.LightCoral); //mouse point
            }
           


            base.Draw(gameTime);
            spriteBatch.End();
        }



        private void TransparentWindow()
        {
            this.Window.IsBorderless = true;
            this.Window.Position = new Point(0, 0);



            IntPtr hWnd = this.Window.Handle;

            // Get current window style and add the "Layered" attribute
            int style = GetWindowLong(hWnd, GWL_EXSTYLE);
            SetWindowLong(hWnd, GWL_EXSTYLE, style | WS_EX_LAYERED | WS_EX_TRANSPARENT);
            SetLayeredWindowAttributes(hWnd, 0, 0, LWA_COLORKEY);

            // 2. Force it to be Always on Top
            // SWP_NOMOVE | SWP_NOSIZE tells Windows: "Keep the current size and position, just move it to the TopMost layer"
            SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
        }

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        // Constants for Always on Top
        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        const uint SWP_NOSIZE = 0x0001;
        const uint SWP_NOMOVE = 0x0002;

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern bool SetLayeredWindowAttributes(IntPtr hWnd, uint crKey, byte bAlpha, uint dwFlags);

        const int GWL_EXSTYLE = -20;
        const int WS_EX_LAYERED = 0x80000;
        const int LWA_COLORKEY = 0x1; 
        const int WS_EX_TRANSPARENT = 0x20; // The "click-through" flag

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        const int KEYEVENTF_KEYUP = 0x0002;
        const byte VK_3 = 0x33; // Virtual Key Code för siffran 3

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out NativePoint lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        public struct NativePoint
        {
            public int X;
            public int Y;
        }
    }
}
