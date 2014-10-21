using System;
using System.Diagnostics;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace Project2.GameObjects.Abstract
{
    public struct PhysicsDescription
    {
        public RigidBody RigidBody;
        public Shape CollisionShape;
        public Boolean Debug;
        public Boolean IsStatic;
        public Vector3 Position;
    }

    public abstract class PhysicsObject : GameObject
    {
        public Project2Game Game { get; set; }

        public PhysicsDescription physicsDescription;

        protected Boolean IsStaticBody;
        protected Boolean DebugDrawStatus;


        protected PhysicsObject(Project2Game game, PhysicsDescription physicsDescription)
            : this(game, null, physicsDescription.Position, physicsDescription)
        {

        }

        protected PhysicsObject(Project2Game game, Model model, Vector3 position, Vector3 orientation, Vector3 size, PhysicsDescription physicsDescription)
            : base(game, model, position, orientation, size)
        {
            this.physicsDescription = physicsDescription;
            this.Position = physicsDescription.Position;

            game.physics.AddBody(physicsDescription.RigidBody);
        }

        protected PhysicsObject(Project2Game game, Model model, Vector3 position, PhysicsDescription physicsDescription)
            : this(game,model,position, Vector3.Zero, Vector3.One,physicsDescription)
        {
        }

        public void Destroy() {
            game.physics.RemoveBody(this.physicsDescription);
        }
     

        public override void Update(GameTime gametime)
        {
            var pos = PhysicsSystem.toVector3(this.physicsDescription.RigidBody.Position);
            Matrix orientation = PhysicsSystem.toMatrix(this.physicsDescription.RigidBody.Orientation);
            
            //System.Diagnostics.Debug.WriteLine(pos);
            //System.Diagnostics.Debug.WriteLine(orientation);

            // each call to SetX recalculates the world matrix. This is inefficient and should be fixed.
            this.SetPosition(pos);
            this.SetOrientation(orientation);
            base.Update(gametime);
        }

        public override void Draw(SharpDX.Toolkit.GameTime gametime)
        {
            if (physicsDescription.RigidBody.EnableDebugDraw && physicsDescription.RigidBody != null)
            {
                physicsDescription.RigidBody.DebugDraw(game.debugDrawer);
            }
            base.Draw(gametime);
        }

        public void DebugDrawEnabled(Boolean t)
        {
            physicsDescription.RigidBody.EnableDebugDraw = t;
            this.DebugDrawStatus = t;
        }
    }
}
