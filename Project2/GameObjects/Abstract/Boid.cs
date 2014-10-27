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
    public abstract class Boid : PhysicsObject
    {
        public Flock.BoidType boidType;
        public Flock flock;

        public Boid(Project2Game game, Flock flock, Model model, Vector3 position, Flock.BoidType boidType)
            : base(game, model, position, GeneratePhysicsDescription(position, model, false))
        {
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
            var self = this.physicsDescription.RigidBody;
            if (body1 == self)
                other = body2;
            else if (body2 == self)
                other = body1;
            else return;

            if (other == this.flock.level.endGoal.physicsDescription.RigidBody) // we've collided with the end zone
            {
                // be careful of what you modify in this handler as it may be called during an Update()
                // attempting to modify any list (such as destroying game objects, etc) will cause an exception
                //this.Destroy(); // remove self (this causes an exception)

                // add to score
                this.game.Score += 10; // this doesn't
                self.ApplyImpulse(new JVector(0,1,0) * 7f, JVector.Zero); // this doesn't
            }
        }

        private static PhysicsDescription GeneratePhysicsDescription(Vector3 position, Model model, Boolean isStatic)
        {
            var bounds = model.CalculateBounds();
            var collisionShape = new SphereShape(bounds.Radius);
            var rigidBody = new RigidBody(collisionShape)
            {
                Position = PhysicsSystem.toJVector(position),
                IsStatic = isStatic,
                EnableDebugDraw = true,
                Mass = 0.25f
            };

            var description = new PhysicsDescription()
            {
                IsStatic = isStatic,
                CollisionShape = collisionShape,
                Debug = false,
                RigidBody = rigidBody,
                Position = position
            };

            return description;
        }



        public override void Update(GameTime gametime)
        {
            //var pos = PhysicsSystem.toVector3(this.physicsBody.Position);
            //var orientation = PhysicsSystem.toMatrix(this.physicsBody.Orientation);
            ////System.Diagnostics.Debug.WriteLine(pos);
            ////System.Diagnostics.Debug.WriteLine(orientation);

            //// each call to SetX recalculates the world matrix. This is inefficient and should be fixed.
            //this.SetPosition(pos);
            //this.SetOrientation(orientation);
            //this.physicsDescription.RigidBody.ApplyImpulse(PhysicsSystem.toJVector(game.inputManager.SecondaryDirection() * 10f), PhysicsSystem.toJVector(Vector3.Zero));
            //this.physicsDescription.RigidBody.ApplyImpulse(PhysicsSystem.toJVector(game.inputManager.Acceleration() * 10f), PhysicsSystem.toJVector(Vector3.Zero));

            base.Update(gametime);
        }

        public override void Draw(GameTime gametime)
        {
            //basicEffect.CurrentTechnique.Passes[0].Apply();
            //basicEffect.World = this.worldMatrix;
            //basicEffect.View = game.camera.view;
            //basicEffect.Projection = game.camera.projection;

            //this.model.Draw(game.GraphicsDevice, this.worldMatrix, game.camera.view, game.camera.projection, basicEffect);
            base.Draw(gametime);
        }
    }

}
