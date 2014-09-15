using System.Linq;
using System.Text;

using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;

namespace Project2.Components
{

    public interface IInputComponent : Component
    {

        public IInputComponent(Project2Game game, GameObject owner) : base(game, owner){
        }

        virtual public void update() {
            // grab all kinds of input here from defined class helper methods
        }
    }
}
