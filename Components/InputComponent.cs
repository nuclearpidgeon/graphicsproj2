using System.Linq;
using System.Text;

using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;

namespace Project2.Components
{

    public class InputComponent : SharpDX.Component
        
    {

        public InputComponent(Project2Game game, GameObject owner) : base(){

        }

        virtual public void update() {
            // grab all kinds of input here from defined class helper methods
        }
    }
}
