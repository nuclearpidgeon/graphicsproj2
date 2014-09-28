using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

using Jitter;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;

namespace Project2.GameObjects
{
    class TestObject : GameObject
    {
        public TestObject(Project2Game game, Vector3 position, Vector3 scale, Vector3 orientation) : base(game) {
            this.SetScale(scale);
            this.SetOrientation(orientation);
            this.SetPosition(position);

            this.model = game.Content.Load<Model>("Models\\Knot");
            
            this.boundingSphere = this.model.CalculateBounds(this.worldMatrix);
            //Jitter.Collision.Octree = new Jitter.Collision.Octree(model.)
            this.physicsShape = new SphereShape(this.boundingSphere.Radius);
            this.physicsBody = new RigidBody(this.physicsShape);
            this.physicsBody.Position = PhysicsSystem.toJVector(position);
            this.physicsBody.Orientation = PhysicsSystem.toJMatrix(this.orientationMatrix);
            this.game.physics.AddBody(this.physicsBody);
            this.basicEffect.LightingEnabled = true;
            //this.basicEffect.EnableDefaultLighting(this.model);
            this.DebugDrawEnabled(true);
        }   

        public override void Draw(GameTime gametime)
        {
            this.model.Draw(game.GraphicsDevice, Matrix.Identity, game.camera.view, game.camera.projection);
            this.physicsBody.DebugDraw(game.debugDrawer);
            base.Draw(gametime);
        }
    }
}
