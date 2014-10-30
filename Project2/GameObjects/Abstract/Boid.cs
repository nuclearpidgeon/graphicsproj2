using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Project2.GameObjects.Abstract;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace Project2.GameObjects.Boids
{
    public abstract class Boid : ModelPhysicsObject
    {
        public Flock.BoidType boidType;
        public Flock flock;

        public Boid(Project2Game game, Flock flock, Model model, Vector3 position, Flock.BoidType boidType)
            : base(game, model, position)
        {
            this.PhysicsDescription.Mass = 0.25f;
            this.boidType = boidType;
            this.flock = flock;
            this.game.physics.World.CollisionSystem.CollisionDetected += HandleCollision;
        }


        /// <summary>
        /// This method handles explicit object collision logic. Is registered to physics engine CollisionDetected event handler.
        /// Fired on any detected collision, so must check if the collision applies to this object
        /// </summary>
        /// <param name="body1"></param>
        /// <param name="body2"></param>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="normal"></param>
        /// <param name="penetration"></param>
        virtual public void HandleCollision(RigidBody body1, RigidBody body2, JVector point1, JVector point2, JVector normal, float penetration)
        {
            // work out which, if any, of the collided bodies is this object, and name them semantically
            RigidBody other;
            var self = this.PhysicsDescription;
            if (body1 == self)
                other = body2;
            else if (body2 == self)
                other = body1;
            else return;

            if (other == this.flock.level.endGoal.PhysicsDescription) // we've collided with the end zone
            {
                // be careful of what you modify in this handler as it may be called during an Update()
                // attempting to modify any list (such as destroying game objects, etc) will cause an exception
                //this.Destroy(); // remove self (this causes an exception)

                // add to score
                this.game.incScore(10); // this doesn't
                self.ApplyImpulse(new JVector(0,1,0) * 7f, JVector.Zero); // this doesn't
            }
        }

    }

}
