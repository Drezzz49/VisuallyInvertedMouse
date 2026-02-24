using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Runtime.InteropServices;

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

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            this.InactiveSleepTime = new TimeSpan(0);


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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();



            //find the distance between the mouse and the center of the screen to set the radius of the circle
            circleRadius = (int)Vector2.Distance(MousePosition.Position, new Vector2(screenResolution.MaxX / 2, screenResolution.MaxY / 2));

            //find the opposite point of the mouse position to the center of the screen
            oppositePoiont = new Vector2(screenResolution.MaxX / 2, screenResolution.MaxY / 2) + (new Vector2(screenResolution.MaxX / 2, screenResolution.MaxY / 2) - MousePosition.Position);


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            

            // TODO: Add your drawing code here
            spriteBatch.DrawString(font, $"Mouse Position: X:{MousePosition.X} Y:{MousePosition.Y}", new Vector2(10, 20), Color.Red); //Text
            spriteBatch.DrawString(font, $"Desired-Mouse Position: X:{oppositePoiont.X} Y:{oppositePoiont.Y}", new Vector2(10, 40), Color.Red); //Text
            spriteBatch.DrawString(font, $"Circle Radius:{circleRadius}", new Vector2(10, 60), Color.Red); //Text

            drawHelper.DrawCircle(new Vector2(screenResolution.MaxX/2,screenResolution.MaxY/2), circleRadius, Color.Red, 360); //circle

            //lines
            drawHelper.DrawLine(screenResolution.MaxX / 2, screenResolution.MaxY / 2, (int)MousePosition.X, (int)MousePosition.Y, Color.Green); //line
            drawHelper.DrawLine(screenResolution.MaxX / 2, screenResolution.MaxY / 2, (int)oppositePoiont.X, (int)oppositePoiont.Y, Color.Blue); //opposite line

            //points
            drawHelper.DrawPoint(new Vector2(screenResolution.MaxX / 2, screenResolution.MaxY / 2), 3 ,Color.LightCoral); //middle point
            drawHelper.DrawPoint(oppositePoiont, 3 ,Color.LightCoral); //opposite point
            drawHelper.DrawPoint(new Vector2((int)MousePosition.X, (int)MousePosition.Y), 3 ,Color.LightCoral); //mouse point


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
