using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Input;

namespace Project2
{
    /// <summary>
    /// This class handles all user input for the game, implemented as a system.
    /// </summary>
    class InputManager : GameSystem
    {
        KeyboardManager keyboardManager;
        MouseManager mouseManager;
        PointerManager pointerManager;

        KeyboardState keyboardState;
        MouseState mouseState;
        PointerState pointerState;

        KeyMapping keyMapping { get; set; }

        public Boolean accelerometerEnabled { get; set; }


        /// <summary>
        /// Initialise the input system. Note that accelerometer input defaults to off.
        /// </summary>
        /// <param name="game"></param>
        public InputManager(Project2Game game) : base(game)
        {
            keyboardManager = new KeyboardManager(game);
            mouseManager = new MouseManager(game);
            pointerManager = new PointerManager(game);
            keyMapping = new KeyMapping();
            accelerometerEnabled = false;
        }

        public override void Update(GameTime gameTime)
        {            
            // accept input
            keyboardManager.Update(gameTime);
            mouseManager.Update(gameTime);
            pointerManager.Update(gameTime);

            // update state
            keyboardState = keyboardManager.GetState();
            mouseState = mouseManager.GetState();
            pointerState = pointerManager.GetState();

 	        base.Update(gameTime);
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
            if (keyboardState.IsKeyDown(keyMapping.right_Primary_key))
            {
                v.X += 1;
            }
            if (keyboardState.IsKeyDown(keyMapping.left_Primary_key))
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
                v.Y += 1;
            }
            if (keyboardState.IsKeyDown(keyMapping.down_Secondary_key))
            {
                v.Y -= 1;
            }
            if (keyboardState.IsKeyDown(keyMapping.right_Secondary_key))
            {
                v.X += 1;
            }
            if (keyboardState.IsKeyDown(keyMapping.left_Secondary_key))
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
                // I'm not sure what a "sprint" action might be like on the accelerometer, maybe an impulse in the direction of movement?
            }
            return T;
        }
    }


    /// <summary>
    /// Class to map keys to their desired input effect. Allows for key remapping.
    /// </summary>
    class KeyMapping {

        public Keys up_Primary_key {public get; set; }
        public Keys down_Primary_key { get; set; }
        public Keys left_Primary_key { get; set; }
        public Keys right_Primary_key { get; set; }

        public Keys up_Secondary_key { get; set; }
        public Keys down_Secondary_key { get; set; }
        public Keys left_Secondary_key { get; set; }
        public Keys right_Secondary_key { get; set; }

        public Keys sprint_key { get; set; }
        public Keys jump_key { get; set; }

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
        }
    }
}

