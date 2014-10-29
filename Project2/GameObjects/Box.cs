using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jitter.LinearMath;
using Project2.GameObjects.Abstract;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

using Jitter;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;

namespace Project2.GameObjects
{
    public class Box : ModelPhysicsObject
    {
        public Box(Project2Game game, Model model, Vector3 position, Vector3 size, Boolean isStatic)
            : base(game, model, position, Vector3.Zero, size)
        {
            this.PhysicsDescription.Mass = 600f;
        }

        public Box(Project2Game game, Model model, Vector3 position, Vector3 size, Vector3 orientation, Boolean isStatic)
            : base(game, model, position, orientation, size)
        {
        }


    }
}
