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
    public class Brick : ModelPhysicsObject
    {
        /// <summary>
        /// Create a new Brick with default orientation
        /// </summary>
        /// <param name="game"></param>
        /// <param name="model"></param>
        /// <param name="position"></param>
        /// <param name="size"></param>
        /// <param name="isStatic"></param>
        public Brick(Project2Game game, Model model, Vector3 position, Vector3 size, Boolean isStatic)
            : base(game, model, position, Vector3.Zero, size)
        {
            this.PhysicsDescription.Mass = 20f;
        }

        protected override RigidBody GeneratePhysicsDescription()
        {
            Shape collisionShape = new BoxShape(Scale.X, Scale.Y, Scale.Z);
            var rigidBody = new RigidBody(collisionShape)
            {
                Position = PhysicsSystem.toJVector(Position),
                Orientation = PhysicsSystem.toJMatrix(OrientationMatrix),
                IsStatic = false,
                EnableDebugDraw = true,
            };

            return rigidBody;
        }
    }
}
