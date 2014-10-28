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
        public PhysicsDescription PhysicsDescription { get; set; }
        protected Boolean IsStatic
        {
            get
            {
                return PhysicsDescription.RigidBody.IsStatic;
            }
            set
            {
                PhysicsDescription.RigidBody.IsStatic = value;
            }
        }
        protected Boolean DebugDrawStatus 
        {
            get 
            {
                return PhysicsDescription.RigidBody.EnableDebugDraw;
            }
            set
            {
                PhysicsDescription.RigidBody.EnableDebugDraw = value;
            }
        }

        #region extra constructors
        protected ModelPhysicsObject(Project2Game game, PhysicsDescription physicsDescription)
            : this(game, null, physicsDescription.Position, physicsDescription)
        {

        }

        protected ModelPhysicsObject(Project2Game game, Model model, Vector3 position, PhysicsDescription physicsDescription)
            : this(game, model, position, Vector3.Zero, Vector3.One, physicsDescription)
        {
        }
        #endregion

        protected ModelPhysicsObject(Project2Game game, Model model, Vector3 position, Vector3 orientation, Vector3 size, PhysicsDescription physicsDescription)
            : base(game, model, position, orientation, size)
        {
            this.PhysicsDescription = physicsDescription;
            this.Position = physicsDescription.Position;

            game.physics.AddBody(physicsDescription.RigidBody);
        }

        public void Destroy() {
            game.physics.RemoveBody(this.PhysicsDescription);
        }     

        public override void Update(GameTime gametime)
        {
            // Update the GameObject position and orientation post physics simulation

            Vector3 pos = PhysicsSystem.toVector3(this.PhysicsDescription.RigidBody.Position);
            Matrix orientation = PhysicsSystem.toMatrix(this.PhysicsDescription.RigidBody.Orientation);

            // NB: each call to SetX recalculates the world matrix. This is inefficient and should be fixed.
            this.Position = pos;
            this.OrientationMatrix = orientation;
            base.Update(gametime);
        }

        public override void Draw(SharpDX.Toolkit.GameTime gametime)
        {
            if (PhysicsDescription.RigidBody.EnableDebugDraw && PhysicsDescription.RigidBody != null)
            {
                PhysicsDescription.RigidBody.DebugDraw(game.debugDrawer);
            }
            base.Draw(gametime);
        }
    }
}
