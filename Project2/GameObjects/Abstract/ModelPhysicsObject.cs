using System;
using System.Diagnostics;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace Project2.GameObjects.Abstract
{

    public abstract class ModelPhysicsObject : ModelGameObject, IPhysicsObject
    {
        public RigidBody PhysicsDescription { get; set; }

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
        }

        protected virtual RigidBody GeneratePhysicsDescription()
        {
            BoundingSphere bounds = model.CalculateBounds();
            Shape collisionShape = new SphereShape(bounds.Radius * 3);
            var rigidBody = new RigidBody(collisionShape)
            {
                Position = PhysicsSystem.toJVector(Position),
                IsStatic = false,
                EnableDebugDraw = true,
            };

            return rigidBody;
        }

        public void Destroy() {
            game.physics.RemoveBody(this.PhysicsDescription);
        }     

        public override void Update(GameTime gametime)
        {
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
