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
        public Box(Project2Game game, Model model, Vector3 position, Vector3 size)
            : this(game, model, position, size, Vector3.Zero)
        {
            
        }

        public Box(Project2Game game, Model model, Vector3 position, Vector3 size, Vector3 orientation)
            : base(game, model, position, orientation, size)
        {
            
        }

        protected override RigidBody GeneratePhysicsDescription()
        {
            Shape collisionShape = new BoxShape(Scale.X, Scale.Y, Scale.Z);
            var rigidBody = new RigidBody(collisionShape)
            {
                Position = PhysicsSystem.toJVector(Position),
                Orientation = PhysicsSystem.toJMatrix(OrientationMatrix),
                IsStatic = true,
                EnableDebugDraw = true,
            };

            return rigidBody;
        }
    }
}
