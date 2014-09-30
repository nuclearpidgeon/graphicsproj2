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
using Project2.GameObjects;
using Project2.GameObjects.Abstract;
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
        public Dictionary<String, Model> models; 

        public ThirdPersonCamera camera { private set; get; }

        private MouseManager mouseManager;
        public MouseState mouseState;

        private SpriteFont consoleFont;
        private SpriteBatch spriteBatch;

        public PhysicsSystem physics { private set; get; }
        public DebugDrawer debugDrawer;
        public InputManager inputManager { private set; get; }

        private Ball playerBall;
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
            models = new Dictionary<string, Model>();
            
        }

        protected override void LoadContent()
        {
            foreach (var modelName in new List<String> { "Teapot", "box", "Sphere" })
            {
                try
                {
                    models.Add(modelName, Content.Load<Model>("Models\\" + modelName));
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e);
                    //throw;
                }
            }
            var heightmap = Content.Load<Texture2D>("Terrain\\heightmap.jpg");


            playerBall = new GameObjects.Ball(this, models["Sphere"], new Vector3(19f, 3f, 14f), false);

            //gameObjects.Add(new GameObjects.TestObject(this, models["Teapot"], new Vector3(14f, 3f, 14f), false));
            gameObjects.Add(playerBall);
            gameObjects.Add(new Project2.GameObjects.Terrain(this, Vector3.Zero, 7, 2, 15));
            //gameObjects.Add(new Terrain(this, new Vector3(0f, 255f, 0f), heightmap, 5.0));

            // Load font for console
            //consoleFont = ToDisposeContent(Content.Load<SpriteFont>("CourierNew10"));

            // Setup spritebatch
            spriteBatch = ToDisposeContent(new SpriteBatch(GraphicsDevice));

            camera.SetFollowObject(playerBall);

            base.LoadContent();
        }

        protected override void Initialize()
        {
            Window.Title = "Project 2";
            graphicsDeviceManager.PreferredBackBufferWidth = Window.ClientBounds.Width;
            graphicsDeviceManager.PreferredBackBufferHeight = Window.ClientBounds.Height;
            graphicsDeviceManager.ApplyChanges();
            // Create camera
            //camera = new Camera(
            //    this,
            //    new Vector3(0, 15, -15),
            //    new Vector3(0, 0, 0)
            //);
            camera = new ThirdPersonCamera(this, new Vector3(0f, 30f, 0f), new Vector3(0f, 1f, 1f) * 35);


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
                gameObjects.Remove(playerBall);
                playerBall = null;
                playerBall = new GameObjects.Ball(this, models["Sphere"], new Vector3(19f, 3f, 14f), false);
                this.camera.SetFollowObject(playerBall);
                gameObjects.Add(playerBall);
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
            GraphicsDevice.Clear(new Color(0.5f));

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