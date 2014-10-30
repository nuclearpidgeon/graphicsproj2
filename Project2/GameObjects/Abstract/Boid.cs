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
        private Boolean ToDestroy;
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
                this.Destroy(true); // remove self
                
                // add to score
                //self.ApplyImpulse(new JVector(0,1,0) * 7f, JVector.Zero); // this doesn't
            }
        }

        public override void Update(GameTime gametime)
        {
            if (ToDestroy)
            {
                Destroy();
            }
            base.Update(gametime);
        }

        /// <summary>
        /// Implements object destroying in a manner that can be called asynchronously
        /// The first call to this function doesn't actually destroy the object, but sets a flag
        /// to destroy it. Calling this function again will remove the object.
        /// A suggested use case it to call this function if ToDestroy is inside Update()
        /// </summary>
        override public void Destroy(Boolean Async = false)
        {

            // the first call to this function
            if (ToDestroy == false)
            {
                ToDestroy = true;
                return;
            }

            if (ToDestroy && !Async)
            {
                // remove from physics system
                game.physics.RemoveBody(this.PhysicsDescription);
                // remove from graph
                this.Parent.RemoveChild(this);
                this.game.Score += 10; // add to score on removal
            }
        } 

    }

}
