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

using Project2.GameSystems;
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
        private BasicLevel level;
        public Dictionary<String, Model> models; 

        public ThirdPersonCamera camera { private set; get; }
        //public ControllableCamera camera { private set; get; }

        private MouseManager mouseManager;
        public MouseState mouseState;

        private SpriteFont consoleFont;
        private SpriteBatch spriteBatch;

        public PhysicsSystem physics { private set; get; }
        public DebugDrawer debugDrawer;
        public InputManager inputManager { private set; get; }

        public Monkey playerBall;
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
            foreach (var modelName in new List<String> { "Teapot", "box", "Sphere", "monkey", "bigmonkey" })
            {
                try
                {
                    var model = Content.Load<Model>("Models\\" + modelName);
                    BasicEffect.EnableDefaultLighting(model, false);
                    models.Add(modelName, model);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e);
                    //throw;
                }
            }
            //var heightmap = Content.Load<Texture2D>("Terrain\\heightmap.jpg");

            level = new BasicLevel(this);

            playerBall = new GameObjects.Monkey(this, models["bigmonkey"], level.getStartPosition(), false);

            //gameObjects.Add(new GameObjects.TestObject(this, models["Teapot"], new Vector3(14f, 3f, 14f), false));
            gameObjects.Add(playerBall);
            //gameObjects.Add(new Project2.GameObjects.Terrain(this, new Vector3(-50f), 7, 2, 15));
            foreach (var levelPiece in level.levelPieces)
            {
                gameObjects.AddRange(levelPiece.gameObjects);
            }

            int size = 3;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    gameObjects.Add(
                        new Project2.GameObjects.Boids.Boid(
                            this, 
                            models["Sphere"],
                            level.getStartPosition() + new Vector3((float)((size / 2.0 - i) * 4), 10f, (float)(size / 2.0 - j) * 4),
                            false
                        )
                    );
                }
            }
            //gameObjects.Add(new Project2.GameObjects.Monkey(this, Vector3.Zero, 7, 2, 15));
            //gameObjects.Add(new Terrain(this, new Vector3(0f, 255f, 0f), heightmap, 5.0));

            // Load font for console
            consoleFont = ToDisposeContent(Content.Load<SpriteFont>("CourierNew10"));

            // Setup spritebatch for console
            spriteBatch = ToDisposeContent(new SpriteBatch(GraphicsDevice));

            camera.SetFollowObject(playerBall);

            base.LoadContent();
        }

        protected override void Initialize()
        {
            Window.Title = "Project 2";

            // Listen for the virtual graphics device so we can initialise the 
            // graphicsDeviceManagers' rendering variables
            graphicsDeviceManager.DeviceCreated += OnDeviceCreated;

            // Create automatic ball-following camera
            camera = new ThirdPersonCamera(this, new Vector3(0f, 20f, 0f), new Vector3(0f, 1f, 2f) * 25);
            //// Create keyboard/mouse-controlled camera
            //camera = new ControllableCamera(this, new Vector3(0f, 30f, 0f), new Vector3(0f, 1f, 1f) * 35);

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

        /// <summary>
        /// When the virtual graphics device is created, we need to grab its viewport dimensions to
        /// pass into the graphicsManager's PreferredBackBuffer Width and Height variables. This data is not
        /// available until the device has been initialised.
        /// </summary>
        /// <param name="sender">The object which dispatched the event.</param>
        /// <param name="e">Additional data stored in the event</param>
        void OnDeviceCreated(object sender, EventArgs e)
        {
            graphicsDeviceManager.PreferredBackBufferWidth = (int)GraphicsDevice.Viewport.Width;
            graphicsDeviceManager.PreferredBackBufferHeight = (int)GraphicsDevice.Viewport.Height;
            graphicsDeviceManager.ApplyChanges();

        }

        public void RemoveGameObject(GameObject o)
        {
            this.gameObjects.Remove(o);
        }

        protected override void Update(GameTime gameTime)
        {


            // Get new mouse info
            mouseState = inputManager.MouseState();

            // Update camera
            camera.Update(gameTime);

            // Update the game objects
            for (int i = 0; i < gameObjects.Count; i++)
            {
                gameObjects[i].Update(gameTime);
            }

            // Reset on escape key
            if (inputManager.IsKeyDown(Keys.Escape))
            {
                // this is janky
                playerBall.Destroy();
                playerBall = new GameObjects.Monkey(this, models["bigmonkey"], level.getStartPosition(), false);
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
            spriteBatch.Begin();
            //spriteBatch.DrawString(consoleFont, "Camera x location: " + camera.position.X, new Vector2(0f, 0f), Color.AliceBlue);
            //spriteBatch.DrawString(consoleFont, "Camera y location: " + camera.position.Y, new Vector2(0f, 12f), Color.AliceBlue);
            //spriteBatch.DrawString(consoleFont, "Camera z location: " + camera.position.Z, new Vector2(0f, 24f), Color.AliceBlue);
            spriteBatch.DrawString(consoleFont, "Player game x location: " + this.playerBall.Position.X, new Vector2(0f, 0f), Color.AliceBlue);
            spriteBatch.DrawString(consoleFont, "Player game y location: " + this.playerBall.Position.Y, new Vector2(0f, 12f), Color.AliceBlue);
            spriteBatch.DrawString(consoleFont, "Player game z location: " + this.playerBall.Position.Z, new Vector2(0f, 24f), Color.AliceBlue);
            spriteBatch.DrawString(consoleFont, "Player phys x location: " + this.playerBall.physicsDescription.RigidBody.Position.X, new Vector2(0f, 36f), Color.AliceBlue);
            spriteBatch.DrawString(consoleFont, "Player phys y location: " + this.playerBall.physicsDescription.RigidBody.Position.Y, new Vector2(0f, 48f), Color.AliceBlue);
            spriteBatch.DrawString(consoleFont, "Player phys z location: " + this.playerBall.physicsDescription.RigidBody.Position.Z, new Vector2(0f, 60f), Color.AliceBlue);

            //spriteBatch.DrawString(consoleFont, "Rigid bodies: " + physics.World.RigidBodies.Count, new Vector2(0f, 36f), Color.AliceBlue);
            //spriteBatch.DrawString(consoleFont, "Physics: " + physics.World.DebugTimes[0], new Vector2(0f, 48f), Color.AliceBlue);
            //spriteBatch.DrawString(consoleFont, "FPS: " + 1.0 /this.gameTime.ElapsedGameTime.TotalSeconds, new Vector2(0f, 60f), Color.AliceBlue);

            spriteBatch.End();

        }
    }
}