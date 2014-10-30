using System;
using System.Collections.Generic;
using System.Diagnostics;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Project2.GameObjects.Interface;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace Project2.GameObjects.Abstract
{

    public abstract class ModelPhysicsObject : ModelGameObject, IPhysicsObject
    {
        public RigidBody PhysicsDescription { get; set; }
        private Boolean ToDestroy;

        #region extra constructors
        /// <summary>
        /// Create a ModelPhysicsObject with default size and orientation.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="model"></param>
        /// <param name="physicsDescription"></param>
        /// <param name="position"></param>
        protected ModelPhysicsObject(Project2Game game, Model model, Vector3 position)
            : this(game, model, position, Vector3.Zero, Vector3.One)
        {
        }
        #endregion

        protected ModelPhysicsObject(Project2Game game, Model model, Vector3 position, Vector3 orientation, Vector3 size)
            : base(game, model, position, orientation, size)
        {
            this.PhysicsDescription = GeneratePhysicsDescription();
            this.Position = PhysicsSystem.toVector3(PhysicsDescription.Position);

            game.physics.AddBody(PhysicsDescription);
            ToDestroy = false;
        }

        protected virtual RigidBody GeneratePhysicsDescription()
        {
            BoundingSphere bounds = model.CalculateBounds(WorldMatrix);
            Shape collisionShape = new SphereShape(bounds.Radius);
            var rigidBody = new RigidBody(collisionShape)
            {
                Position = PhysicsSystem.toJVector(Position),
                Orientation = PhysicsSystem.toJMatrix(OrientationMatrix),
                IsStatic = false,
                EnableDebugDraw = true,
            };

            return rigidBody;
        }

        /// <summary>
        /// Implements object destroying in a manner that can be called asynchronously
        /// The first call to this function doesn't actually destroy the object, but sets a flag
        /// to destroy it. Calling this function again will remove the object.
        /// A suggested use case it to call this function if ToDestroy is inside Update()
        /// </summary>
        virtual public void Destroy(Boolean Async = false) {

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
            }
        }     

        public override void Update(GameTime gametime)
        {
            if (ToDestroy) // handle out-of-sync called (i.e. event handled) object destruction in next game engine step
            {
                Destroy();
            }

            // Update the GameObject position and orientation post physics simulation
            // NB: each call to SetX recalculates the world matrix. This is inefficient and should be fixed.
            this.Position = PhysicsSystem.toVector3(this.PhysicsDescription.Position);
            this.OrientationMatrix = PhysicsSystem.toMatrix(this.PhysicsDescription.Orientation); ;
            base.Update(gametime);
        }

        public override void Draw(SharpDX.Toolkit.GameTime gametime)
        {
            if (PhysicsDescription.EnableDebugDraw && PhysicsDescription != null)
            {
                PhysicsDescription.DebugDraw(game.debugDrawer);
            }           
            base.Draw(gametime);
        }

    }
}
