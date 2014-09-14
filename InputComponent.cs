using System.Linq;
using System.Text;

using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;

namespace Project2
{
    public class InputComponent : Component
    {

        public InputComponent(Project2Game game, GameObject owner) : base(game, owner) {
            
        }

        virtual public void update() {
            // grab all kinds of input here from defined class helper methods
        }

        /// <summary>
        /// Gets a normalised vector of directional keyboard input based on arrow keys and WASD layout.
        /// Overide this method.
        /// </summary>
        /// <returns>Normalised vector where up/down keys are the y component of the vector and left/right is the x component. z component is 0.</returns>
        public virtual Vector3 get_input() {
            var state = this.game.keyboardState;
            var vec =  new Vector3(); // initialise to zero vector

            // Y component
            if (state.IsKeyDown(Keys.W) || state.IsKeyDown(Keys.Up)) {
                vec.Y += 1;
            }
            if (state.IsKeyDown(Keys.S) || state.IsKeyDown(Keys.Down))
            {
                vec.Y -= 1;
            }
            // X component
            if (state.IsKeyDown(Keys.A) || state.IsKeyDown(Keys.Left))
            {
                vec.X -= 1;
            }
            if (state.IsKeyDown(Keys.D) || state.IsKeyDown(Keys.Right))
            {
                vec.Y += 1;
            }

            return vec;       
        }
    }
}
