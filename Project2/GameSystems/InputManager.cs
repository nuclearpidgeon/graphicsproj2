﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project2.GameSystems;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Input;

using Windows.Devices.Sensors;
using Windows.UI.Xaml;
using Windows.UI.Input;
using Windows.UI.Core;

namespace Project2
{
    /// <summary>
    /// This class handles all user input for the game, implemented as a system.
    /// </summary>
    public class InputManager : GameSystem
    {
        KeyboardManager keyboardManager;
        MouseManager mouseManager;
        Vector2 mouseDelta;

        public GestureRecognizer gestureRecognizer;

        PointerManager pointerManager;
        PointerState pointerState;


        KeyboardState keyboardState;
        MouseState mouseState, prevMouseState;


        private Boolean accelerometerEnabled;
        private Boolean useMouseDelta;

        public Boolean mouseClick, mouseHeld;

        Accelerometer accelerometer;
        AccelerometerReading accelerometerReading;

        public CoreWindow window;
        
        KeyMapping keyMapping { get; set; }

        /// <summary>
        /// Initialise the input system. Note that accelerometer input defaults to off.
        /// </summary>
        /// <param name="game"></param>
        public InputManager(Project2Game game) : base(game)
        {

            // initialisation
            useMouseDelta = false;
            accelerometerEnabled = false;
            mouseDelta = new Vector2();

            keyboardManager = new KeyboardManager(game);
            mouseManager = new MouseManager(game);
            pointerManager = new PointerManager(game);
            keyMapping = new KeyMapping();

            // get the accelerometer. Returns null if no accelerometer found
            accelerometer = Accelerometer.GetDefault();
            window = Window.Current.CoreWindow;

            // Set up the gesture recognizer.  In this game, it only responds to TranslateX, TranslateY and Tap events
            gestureRecognizer = new Windows.UI.Input.GestureRecognizer();
            gestureRecognizer.GestureSettings = GestureSettings.ManipulationTranslateX | 
                GestureSettings.ManipulationTranslateY | GestureSettings.Tap;
            
            // Register event handlers for pointer events
            window.PointerPressed += OnPointerPressed;
            window.PointerMoved += OnPointerMoved;
            window.PointerReleased += OnPointerReleased;
            
            // automatically enable accelerometer if we have one
            this.AccelerometerEnabled(true);
            this.MouseDeltaEnabled(true);
            
        }

        #region touch input event handlers
        // Call the gesture recognizer when a pointer event occurs
        void OnPointerPressed(CoreWindow sender, PointerEventArgs args)
        {
            gestureRecognizer.ProcessDownEvent(args.CurrentPoint);
        }

        void OnPointerMoved(CoreWindow sender, PointerEventArgs args)
        {
            gestureRecognizer.ProcessMoveEvents(args.GetIntermediatePoints());
        }

        void OnPointerReleased(CoreWindow sender, PointerEventArgs args)
        {
            gestureRecognizer.ProcessUpEvent(args.CurrentPoint);
        }
        #endregion

        /// <summary>
        /// Used to set the enable state of the accelerometer.
        /// When the accelerometer is disabled, the acceleration data returned is a zero vector.
        /// </summary>
        /// <param name="t"></param>
        public void AccelerometerEnabled(Boolean t) {
            if (accelerometer != null) {
                accelerometerEnabled = t;
            } else {
                accelerometerEnabled = false;
            }  
        }
        
        /// <summary>
        /// Update the state of all desired input devices.
        /// </summary>
        /// <param name="gameTime"></param>
        override public void Update(GameTime gameTime)
        {   
            // update state
            keyboardManager.Update(gameTime);
            keyboardState = keyboardManager.GetState();
            mouseState = mouseManager.GetState();
            pointerState = pointerManager.GetState();

              
            //mouseClick = mouseState.LeftButton.Pressed && !prevMouseState.LeftButton.Pressed;
            //mouseHeld = mouseState.LeftButton.Pressed && prevMouseState.LeftButton.Pressed;

            if (accelerometer != null) {
                accelerometerReading = accelerometer.GetCurrentReading();
                
            }

            // get mouse delta and reset mouse to centre of window
            if (useMouseDelta && mouseManager.Enabled) {
                mouseDelta = new Vector2(0.5f - mouseState.X, 0.5f - mouseState.Y);
                mouseManager.SetPosition(new Vector2(0.5f, 0.5f));
            }

            // record previous mouse state
            prevMouseState = mouseState;

 	        base.Update(gameTime);
        }

        /// <summary>
        /// Get the raw pointer state;
        /// </summary>
        /// <returns></returns>
        public PointerState PointerState()
        {
            return this.pointerState;
        }

        /// <summary>
        /// Get the raw mouse state;
        /// </summary>
        /// <returns></returns>
        public MouseState MouseState() {
            return this.mouseState;
        }

        public Boolean SingleClick()
        {
            return mouseClick;
        }

        /// <summary>
        /// Get the raw mouse position
        /// </summary>
        /// <returns></returns>
        public Vector2 MousePosition()
        {
            return new Vector2(mouseState.X, mouseState.Y);
        }

        /// <summary>
        /// Get a non-normalised vector representing the direction of mouse movement since the last frame;
        /// </summary>
        /// <returns></returns>
        public Vector2 MouseDelta() {
            return this.mouseDelta;
        }

        /// <summary>
        /// Enable capturing of mouse cursor and mouse delta data.
        /// </summary>
        /// <param name="t"></param>
        public void MouseDeltaEnabled(Boolean t) {
            this.useMouseDelta = t;
        }

        /// <summary>
        /// Check if Key is pressed.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Boolean IsKeyDown(Keys key) {
            return keyboardState.IsKeyDown(key);
        }

        /// <summary>
        /// Get the acceleration data from sensor input as of last update.
        /// </summary>
        /// <returns></returns>
        public Vector3 Acceleration() {
            var v = new Vector3();
            if (accelerometerEnabled && accelerometerReading != null) {
                v = new Vector3(
                    (float)accelerometerReading.AccelerationX * -1, //this is due to RH matrix stuff
                    0f,
                    (float)accelerometerReading.AccelerationY
                    )
                    * (float)PersistentStateManager.accelSensitivity; // apply accelerometer sensitivity scaling

            }
            //Debug.WriteLine(v);
            return v;
        }

        /// <summary>
        /// Helper method to return primary directional input vector
        /// </summary>
        /// <returns></returns>
        public Vector3 PrimaryDirection()
        {
            var v = new Vector3();
            if (keyboardState.IsKeyDown(keyMapping.up_Primary_key)) {
                v.Y += 1;
            }
            if (keyboardState.IsKeyDown(keyMapping.down_Primary_key))
            {
                v.Y -= 1;
            }
            if (keyboardState.IsKeyDown(keyMapping.left_Primary_key))
            {
                v.X += 1;
            }
            if (keyboardState.IsKeyDown(keyMapping.right_Primary_key))
            {
                v.X -= 1;
            }

            // do accelerometer stuff here if enabled
            if (accelerometerEnabled) {
            }

            return v;
        }

        /// <summary>
        /// Helper method to return secondary directional input vector
        /// </summary>
        /// <returns></returns>
        public Vector3 SecondaryDirection()
        {
            var v = new Vector3();

            if (keyboardState.IsKeyDown(keyMapping.up_Secondary_key))
            {
                v.Z += 1;
            }
            if (keyboardState.IsKeyDown(keyMapping.down_Secondary_key))
            {
                v.Z -= 1;
            }
            if (keyboardState.IsKeyDown(keyMapping.left_Secondary_key))
            {
                v.X += 1;
            }
            if (keyboardState.IsKeyDown(keyMapping.right_Secondary_key))
            {
                v.X -= 1;
            }

            return v;
        }

        /// <summary>
        /// Helper method to determine if there is an input that should be interpreted as a "jump" command.
        /// </summary>
        /// <returns></returns>
        public Boolean Jump()
        {
            var T = new Boolean();
            T = false;

            if (keyboardState.IsKeyDown(keyMapping.jump_key)) {
                T = true;
            }

            if (accelerometerEnabled) {
                // look for impulse event of user jolting the tablet PC upward
                // accelerometer.Shaken
            }
            return T;
        }

        /// <summary>
        /// Helper method to determine if there is an input that should be interpreted as a "sprint" command.
        /// </summary>
        /// <returns></returns>
        public Boolean Sprint()
        {
            var T = new Boolean();
            T = false;

            if (keyboardState.IsKeyDown(keyMapping.sprint_key))
            {
                T = true;
            }

            if (accelerometerEnabled)
            {
                //accelerometer.Shaken
                // I'm not sure what a "sprint" action might be like on the accelerometer, maybe an impulse in the direction of movement?
            }
            return T;
        }

        /// <summary>
        /// Helper method to determine the player has requested a new pause or unpause event
        /// </summary>
        /// <returns>If the user has just pressed the pause button</returns>
        public bool PauseRequest()
        {
            bool result = false;

            if (keyboardState.IsKeyDown(keyMapping.pause_key))
            {
                if (!keyMapping.pause_pressed) result = keyMapping.pause_pressed = true;
            }
            else keyMapping.pause_pressed = false;


            return result;
        }
    }


    /// <summary>
    /// Class to map keys to their desired input effect. Allows for key remapping.
    /// </summary>
    class KeyMapping
    {
        #region default keymap
        public Keys up_Primary_key { get; set; }
        public Keys down_Primary_key { get; set; }
        public Keys left_Primary_key { get; set; }
        public Keys right_Primary_key { get; set; }

        public Keys up_Secondary_key { get; set; }
        public Keys down_Secondary_key { get; set; }
        public Keys left_Secondary_key { get; set; }
        public Keys right_Secondary_key { get; set; }

        public Keys sprint_key { get; set; }
        public Keys jump_key { get; set; }

        public Keys pause_key { get; set; }
        public bool pause_pressed { get; set; }
        #endregion

        public KeyMapping()
        {
            // Initialise default keys
            DefaultKeymapState();
        }

        /// <summary>
        /// Return key mapping to default state.
        /// </summary>
        private void DefaultKeymapState()
        {
            up_Primary_key = Keys.W;
            down_Primary_key = Keys.S;
            left_Primary_key = Keys.A;
            right_Primary_key = Keys.D;

            up_Secondary_key = Keys.Up;
            down_Secondary_key = Keys.Down;
            left_Secondary_key = Keys.Left;
            right_Secondary_key = Keys.Right;

            sprint_key = Keys.Shift; 
            jump_key = Keys.Space;
            pause_key = Keys.Escape;
        }
    }
}

