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

using Windows.Devices.Sensors;
using Windows.UI.Input;
using Windows.UI.Core;
using Project2.Levels;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Input;

using Project2.GameSystems;

namespace Project2
{
    // Use this namespace here in case we need to use Direct3D11 namespace as well, as this
    // namespace will override the Direct3D11.
    using SharpDX.Toolkit.Graphics;
    using Project2.GameObjects.Events;

    public enum LevelSelection
    {
        Level1,
        Level2
    }

    public class Project2Game : Game
    {
        
        private GraphicsDeviceManager graphicsDeviceManager;
        private LevelSelection levelSelection;
        public Level level;
        private Double Score;
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

        private bool paused = false;

        public event EventHandler PauseRequest;
        public event EventHandler ScoreUpdated;

        /// <summary>
        /// Initializes a new instance of the <see cref="Project2Game" /> class.
        /// </summary>
        public Project2Game(LevelSelection levelSelection)
        {
            // Creates a graphics manager. This is mandatory.
            graphicsDeviceManager = new GraphicsDeviceManager(this);

            // Setup the relative directory to the executable directory
            // for loading contents with the ContentManager
            Content.RootDirectory = "Content";

            this.levelSelection = levelSelection;
            models = new Dictionary<string, Model>();
            Score = 0;
            this.IsFixedTimeStep = !PersistentStateManager.dynamicTimestep; // note the NOT

        }

        protected override void LoadContent()
        {
            foreach (var modelName in new List<String> { "Teapot", "box", "Sphere", "monkey", "bigmonkey" })
            {
                try
                {
                    var model = Content.Load<Model>("Models\\" + modelName);
                    BasicEffect.EnableDefaultLighting(model, true);
                    models.Add(modelName, model);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e);
                    //throw;
                }
            }
            //var heightmap = Content.Load<Texture2D>("Terrain\\heightmap.jpg");

            switch (levelSelection)
            {
                case LevelSelection.Level1:
                    level = new TestLevel(this);
                    break;
                case LevelSelection.Level2:
                    level = new TerrainLevel(this);
                    break;
                default:
                    level = new TestLevel(this);
                    break;
            }

            // Load font for console
            //consoleFont = ToDisposeContent(Content.Load<SpriteFont>("CourierNew10"));

            // Setup spritebatch for console
            //spriteBatch = ToDisposeContent(new SpriteBatch(GraphicsDevice));

            camera.position = level.getCameraStartPosition();
            camera.offset = level.getCameraOffset();
            camera.SetFollowObject(this.level.player);

            base.LoadContent();
        }

        protected override void Initialize()
        {
            Window.Title = "Project 2";

            // Listen for the virtual graphics device so we can initialise the 
            // graphicsDeviceManagers' rendering variables
            graphicsDeviceManager.DeviceCreated += OnDeviceCreated;
            
            // Create automatic ball-following camera
            camera = new ThirdPersonCamera(this, Vector3.Zero, Vector3.Zero);
            /// NOTE that camera position gets overidden in LoadContent()
            //// Create keyboard/mouse-controlled camera
            //camera = new ControllableCamera(this, new Vector3(0f, 30f, 0f), new Vector3(0f, 1f, 1f) * 35);

            // Create some GameSystems
            inputManager = new InputManager(this);
            physics = new PhysicsSystem(this);
            debugDrawer = new DebugDrawer(this);

            // enable their Update() or Draw() routines to be automagically called by Game.Update() / Game.Draw() as they implement IUpdateable or IDrawable
            inputManager.Enabled = true;
            physics.Enabled = true;

            debugDrawer.Enabled = false;
            debugDrawer.Visible = false;

            this.GameSystems.Add(debugDrawer);
            this.GameSystems.Add(physics);
            this.GameSystems.Add(inputManager);

            // Initialise event handling.
            inputManager.gestureRecognizer.Tapped += Tapped;
            inputManager.gestureRecognizer.ManipulationStarted += OnManipulationStarted;
            inputManager.gestureRecognizer.ManipulationUpdated += OnManipulationUpdated;
            inputManager.gestureRecognizer.ManipulationCompleted += OnManipulationCompleted;
            inputManager.gestureRecognizer.Holding += OnHolding;

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


        private void TestPause()
        {
            // Pause on spacekey
            if (inputManager.PauseRequest()) togglePaused();
        }

        public void incScore(int x)
        {
            Score += x;
            if (ScoreUpdated != null) ScoreUpdated(this, new ScoreUpdatedEvent(Score));    
        }

        protected override void Update(GameTime gameTime)
        {

            TestPause();
            if (paused) {
                inputManager.Update(gameTime);
                return;
            }

            // Get new mouse info
            mouseState = inputManager.MouseState();

            // Update camera
            camera.Update(gameTime);


            // Update the level
            level.Update(gameTime);

            // Reset on escape key
            // if (inputManager.IsKeyDown(Keys.Escape)) restartGame();

            // Handle base.Update
            base.Update(gameTime);
        }

        public void restartGame()
        {
            level.ResetPlayer();
        }

        public void togglePaused()
        {
            paused = !paused;
            // Dispatch an event to pause
            EventHandler handler = PauseRequest;
            if (handler != null) handler(this, null);            
            
        }

        public bool isPaused()
        {
            return paused;
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

        #region touch input event handlers for this context
        public void Tapped(GestureRecognizer sender, TappedEventArgs args)
        {
            physics.Tapped(sender, args);
        }

        public void OnManipulationStarted(GestureRecognizer sender, ManipulationStartedEventArgs args)
        {
            physics.OnManipulationStarted(sender, args);
        }

        public void OnManipulationUpdated(GestureRecognizer sender, ManipulationUpdatedEventArgs args)
        {
            physics.OnManipulationUpdated(sender, args);
        }

        public void OnManipulationCompleted(GestureRecognizer sender, ManipulationCompletedEventArgs args)
        {
            physics.OnManipulationCompleted(sender, args);
        }

        public void OnHolding(GestureRecognizer sender, HoldingEventArgs args)
        {
            //physics.OnHolding(sender, args);
        }
        #endregion

        protected override void Draw(GameTime gameTime)
        {
            // Clears the screen with the Color.CornflowerBlue
            GraphicsDevice.Clear(new Color(0.5f));


            level.Draw(gameTime);
            // Handle base.Draw
            base.Draw(gameTime);
            // SpriteBatch must be the last thing drawn, not super sure why yet.
            if (PersistentStateManager.debugRender && consoleFont != null)
            {
            //spriteBatch.Begin();
            //    spriteBatch.DrawString(consoleFont, "Camera x location: " + camera.position.X, new Vector2(0f, 0f), Color.AliceBlue);
            //    spriteBatch.DrawString(consoleFont, "Camera y location: " + camera.position.Y, new Vector2(0f, 12f), Color.AliceBlue);
            //    spriteBatch.DrawString(consoleFont, "Camera z location: " + camera.position.Z, new Vector2(0f, 24f), Color.AliceBlue);
            //    //spriteBatch.DrawString(consoleFont, "Player game x location: " + this.level.player.Position.X, new Vector2(0f, 0f), Color.AliceBlue);
            //    //spriteBatch.DrawString(consoleFont, "Player game y location: " + this.level.player.Position.Y, new Vector2(0f, 12f), Color.AliceBlue);
            //    //spriteBatch.DrawString(consoleFont, "Player game z location: " + this.level.player.Position.Z, new Vector2(0f, 24f), Color.AliceBlue);
            //    //spriteBatch.DrawString(consoleFont, "Player phys x location: " + this.level.player.PhysicsDescription.Position.X, new Vector2(0f, 36f), Color.AliceBlue);
            //    //spriteBatch.DrawString(consoleFont, "Player phys y location: " + this.level.player.PhysicsDescription.Position.Y, new Vector2(0f, 48f), Color.AliceBlue);
            //    //spriteBatch.DrawString(consoleFont, "Player phys z location: " + this.level.player.PhysicsDescription.Position.Z, new Vector2(0f, 60f), Color.AliceBlue);
            //spriteBatch.End();
            }


        }
    }
}