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
    public class Ball : ModelPhysicsObject
    {
        public Ball(Project2Game game, Model model, Vector3 position)
            : base(game, model, position)
        {
            this.PhysicsDescription.Mass = 600f;
        }

        public override void Update(GameTime gametime)
        {
            this.PhysicsDescription.ApplyImpulse(PhysicsSystem.toJVector(game.inputManager.SecondaryDirection() * 400f), PhysicsSystem.toJVector(Vector3.Zero));
            this.PhysicsDescription.ApplyImpulse(PhysicsSystem.toJVector(game.inputManager.Acceleration() * 400f), PhysicsSystem.toJVector(Vector3.Zero));

            base.Update(gametime);
        }

    }
}
