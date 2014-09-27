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
using System.Collections.Generic;

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
        private List<GameObject> gameObjects;

        public Camera camera { private set; get; }

        private MouseManager mouseManager;
        public MouseState mouseState;

        private SpriteFont consoleFont;
        private SpriteBatch spriteBatch;

        public PhysicsSystem physics { private set; get; }
        public DebugDrawer debugDrawer;
        public InputManager inputManager { private set; get; }
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

            gameObjects = new List<GameObject>();
        }

        protected override void LoadContent()
        {

            gameObjects.Add(new Cube(this, new Vector3(10f, 1f, 10f), Vector3.Zero, false));
            //gameObjects.Add(new Cube(this, new Vector3(1, 1f, 1), new Vector3(0.5f, 2f, 0f), true));
            gameObjects.Add(new Cube(this, new Vector3(1, 1f, 1), new Vector3(0f, 10f, 0f), true));
            gameObjects.Add(new Cube(this, new Vector3(1, 1f, 1), new Vector3(0.3f, 11f, 0f), true));
            gameObjects.Add(new Ball(this, new Vector3(0f, 10f, 0f), Vector3.One, Vector3.Zero));
            //gameObjects.Add(new Cube(this, new Vector3(1, 1f, 1), new Vector3(0f, 12f, 0.2f), true));
            //gameObjects.Add(new Cube(this, new Vector3(1, 1f, 1), new Vector3(3f, 1f, 0.2f), true));
            //Model model2 = Content.Load<Model>("torus.fbx");

            // Load font for console
            //consoleFont = ToDisposeContent(Content.Load<SpriteFont>("CourierNew10"));

            // Setup spritebatch
            spriteBatch = ToDisposeContent(new SpriteBatch(GraphicsDevice));


            base.LoadContent();
        }

        protected override void Initialize()
        {
            Window.Title = "Project 2";
            graphicsDeviceManager.PreferredBackBufferWidth = Window.ClientBounds.Width;
            graphicsDeviceManager.PreferredBackBufferHeight = Window.ClientBounds.Height;
            graphicsDeviceManager.ApplyChanges();
            // Create camera
            camera = new Camera(
                this,
                new Vector3(0, 15, -15),
                new Vector3(0, 0, 0)
            );

            // Create some GameSystems
            inputManager = new InputManager(this);
            physics = new PhysicsSystem(this);
            debugDrawer = new DebugDrawer(this);

            // enable their Update() or Draw() routines to be automagically called by Game.Update() / Game.Draw() as they implement IUpdateable or IDrawable
            inputManager.Enabled = true;
            physics.Enabled = true;

            debugDrawer.Enabled = true;
            debugDrawer.Visible = true;

            this.GameSystems.Add(debugDrawer);
            this.GameSystems.Add(physics);
            this.GameSystems.Add(inputManager);


            base.Initialize();
        }


        protected override void Update(GameTime gameTime)
        {


            // Get new mouse info
            mouseState = inputManager.MouseState();

            // Update camera
            camera.Update(gameTime);

            // Update the basic model
            for (int i = 0; i < gameObjects.Count; i++)
            {
                gameObjects[i].Update(gameTime);
            }

            // Quit on escape key
            if (inputManager.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }


            // Handle base.Update
            base.Update(gameTime);
        }

        /// <summary>
        /// Use this method body to do stuff while the game is exiting.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected override void OnExiting(object sender, EventArgs args)
        {

            System.Diagnostics.Debug.WriteLine("Shutting down...");
            App.Current.Exit();
            base.OnExiting(sender, args);
        }


        protected override void Draw(GameTime gameTime)
        {
            // Clears the screen with the Color.CornflowerBlue
            GraphicsDevice.Clear(new Color(0.1f));

            for (int i = 0; i < gameObjects.Count; i++)
            {
                gameObjects[i].Draw(gameTime);
            }

            // Handle base.Draw
            base.Draw(gameTime);

            // SpriteBatch must be the last thing drawn, not super sure why yet.
            //spriteBatch.Begin();
            //spriteBatch.DrawString(consoleFont, "Camera x location: " + camera.position.X, new Vector2(0f, 0f), Color.AliceBlue);
            //spriteBatch.DrawString(consoleFont, "Camera y location: " + camera.position.Y, new Vector2(0f, 12f), Color.AliceBlue);
            //spriteBatch.DrawString(consoleFont, "Camera z location: " + camera.position.Z, new Vector2(0f, 24f), Color.AliceBlue);
            //spriteBatch.End();

        }
    }
}