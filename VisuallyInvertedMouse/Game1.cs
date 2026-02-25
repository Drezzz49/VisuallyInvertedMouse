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
        Vector2 middleOfScreen;
        bool isOverlayVisible = true;
        float timeSinceToggle = 0f;
        float timeSinceToggleClick = 0f;

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

            //find the opposite point of the mouse position to the center of the screen
            oppositePoiont = new Vector2(screenResolution.MaxX / 2, screenResolution.MaxY / 2) + (new Vector2(screenResolution.MaxX / 2, screenResolution.MaxY / 2) - MousePosition.Position);

            //normalize the opposite point so it's in the right direction but length of 1
            normalizedOppositePoint = Vector2.Normalize(oppositePoiont - new Vector2(screenResolution.MaxX / 2, screenResolution.MaxY / 2));

            //find the distance between the mouse and the center of the screen to set the radius of the circle
            circleRadius = (int)Vector2.Distance(MousePosition.Position, new Vector2(screenResolution.MaxX / 2, screenResolution.MaxY / 2));

            middleOfScreen = new Vector2(screenResolution.MaxX / 2, screenResolution.MaxY / 2);

            //float timeSinceToggleClick = 0f;
            //timeSinceToggleClick += (float)gameTime.ElapsedGameTime.TotalSeconds;
            //if (Keyboard.GetState().IsKeyDown(Keys.Z) && timeSinceToggle >= 0.3f)
            //{
            //    Vector2 mousePosition = new Vector2((int)MousePosition.X, (int)MousePosition.Y);

            //    MouseHelper.SetMousePosition((int)oppositePoiont.X, (int)oppositePoiont.Y);
            //    MouseHelper.SendLeftClick();
            //    MouseHelper.SetMousePosition((int)mousePosition.X, (int)mousePosition.Y);
            //}




            // Inside Update(GameTime gameTime)
            const int VK_Z = 0x5A; // Hex code for 'Z'
            timeSinceToggleClick += (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Check if Z is physically down across the whole OS
            if ((GetAsyncKeyState(VK_Z) & 0x8000) != 0 && timeSinceToggleClick >= 0.3f)
            {
                timeSinceToggleClick = 0f;

                // Capture positions locally so the background thread can use them
                int startX = (int)MousePosition.X;
                int startY = (int)MousePosition.Y;
                int targetX = (int)oppositePoiont.X;
                int targetY = (int)oppositePoiont.Y;

                // Run the click sequence in the background
                Task.Run(() =>
                {
                    MouseHelper.SetMousePosition(targetX, targetY);
                    System.Threading.Thread.Sleep(3);

                    // Manual Down-Wait-Up to avoid freezing
                    MouseHelper.SendLeftClick(0); // Just set position, no click yet

                    System.Threading.Thread.Sleep(10);
                    MouseHelper.SetMousePosition(startX, startY);
                    MouseHelper.SetMousePosition(startX, startY);
                    MouseHelper.SetMousePosition(startX, startY);
                    MouseHelper.SetMousePosition(startX, startY);
                });
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
                // TODO: Add your drawing code here
                spriteBatch.DrawString(font, "Close application with f8 and hide/un-hide overlay with f7", new Vector2(10, 20), Color.Yellow);
                spriteBatch.DrawString(font, $"Mouse Position: X:{MousePosition.X} Y:{MousePosition.Y}", new Vector2(10, 40), Color.LightSteelBlue); //Text
                spriteBatch.DrawString(font, $"Desired-Mouse Position: X:{oppositePoiont.X} Y:{oppositePoiont.Y}", new Vector2(10, 60), Color.LightSteelBlue); //Text
                spriteBatch.DrawString(font, $"Circle Radius:{circleRadius}", new Vector2(10, 80), Color.LightSteelBlue); //Text

                drawHelper.DrawCircle(new Vector2(screenResolution.MaxX / 2, screenResolution.MaxY / 2), circleRadius, Color.Red, 360); //tghe big circle
                drawHelper.DrawCircle(new Vector2(screenResolution.MaxX / 2, screenResolution.MaxY / 2), (int)MathF.Floor(Vector2.Distance(middleOfScreen + (normalizedOppositePoint * 50), middleOfScreen)), Color.Red, 360); //the middle circle

                //lines
                drawHelper.DrawLine(screenResolution.MaxX / 2, screenResolution.MaxY / 2, (int)MousePosition.X, (int)MousePosition.Y, Color.Green); //line
                drawHelper.DrawLine(screenResolution.MaxX / 2, screenResolution.MaxY / 2, (int)oppositePoiont.X, (int)oppositePoiont.Y, Color.Blue); //opposite line

                //points
                drawHelper.DrawPoint(new Vector2(screenResolution.MaxX / 2, screenResolution.MaxY / 2), 3, Color.LightCoral); //middle point
                drawHelper.DrawPoint(oppositePoiont, 3, Color.LightCoral); //opposite point
                drawHelper.DrawPoint(middleOfScreen + (normalizedOppositePoint*50), 3, Color.LightBlue);
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
    }
}
