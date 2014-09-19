// Copyright (c) 2010-2013 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;

using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Input;

namespace Project2
{
    // Use this namespace here in case we need to use Direct3D11 namespace as well, as this
    // namespace will override the Direct3D11.
    using SharpDX.Toolkit.Graphics;

    public class Project2Game : Game
    {
        private GraphicsDeviceManager graphicsDeviceManager;
        private GameObject model;

        public Camera camera { private set; get; }

        private MouseManager mouseManager;
        public MouseState mouseState;

        private KeyboardManager keyboardManager;
        public KeyboardState keyboardState;

        private SpriteFont consoleFont;
        private SpriteBatch spriteBatch;

        public PhysicsSystem physics { private set; get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Project2Game" /> class.
        /// </summary>
        public Project2Game()
        {
            // Creates a graphics manager. This is mandatory.
            graphicsDeviceManager = new GraphicsDeviceManager(this);

            // Setup the relative directory to the executable directory
            // for loading contents with the ContentManager
            Content.RootDirectory = "Content";

            // Setup the keyboard manager
            keyboardManager = new KeyboardManager(this);

            // Setup the mouse manager
            mouseManager = new MouseManager(this);
        }

        protected override void LoadContent()
        {
            // Load the basic cube
            model = new Cube(this);

            // Load font for console
            consoleFont = ToDisposeContent(Content.Load<SpriteFont>("CourierNew10"));

            // Setup spritebatch
            spriteBatch = ToDisposeContent(new SpriteBatch(GraphicsDevice));


            base.LoadContent();
        }

        protected override void Initialize()
        {
            Window.Title = "Project 2";

            physics = new PhysicsSystem(this);
            this.GameSystems.Add(physics);

            // Create camera
            camera = new Camera(
                this,
                new Vector3(0, 15, -15),
                new Vector3(0, 0, 0)
            );




            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            // Get new keyboard info
            keyboardState = keyboardManager.GetState();

            // Get new mouse info
            mouseState = mouseManager.GetState();

            // Update camera
            camera.Update(gameTime);

            // Update the basic model
            model.Update(gameTime);

            // Quit on escape key
            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                this.Exit();
                this.Dispose();
            }

            // Handle base.Update
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Clears the screen with the Color.CornflowerBlue
            GraphicsDevice.Clear(new Color(0.1f));

            model.Draw(gameTime);

            spriteBatch.Begin();
            spriteBatch.DrawString(consoleFont, "Camera x location: " + camera.position.X, new Vector2(0f, 0f), Color.AliceBlue);
            spriteBatch.DrawString(consoleFont, "Camera y location: " + camera.position.Y, new Vector2(0f, 12f), Color.AliceBlue);
            spriteBatch.DrawString(consoleFont, "Camera z location: " + camera.position.Z, new Vector2(0f, 24f), Color.AliceBlue);
            spriteBatch.End();

            // Handle base.Draw
            base.Draw(gameTime);
        }
    }
}
