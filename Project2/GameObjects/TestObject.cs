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

            this.model = game.Content.Load<Model>("Models\\Teapot");
            
            this.boundingSphere = this.model.CalculateBounds(this.worldMatrix);
            System.Diagnostics.Debug.WriteLine(this.boundingSphere.ToString());
            
            this.physicsShape = PhysicsSystem.BuildTriangleMeshShape(this.model);
            //this.physicsShape = new SphereShape(this.boundingSphere.Radius / 4);
            
            this.physicsBody = new RigidBody(this.physicsShape);
            this.physicsBody.Position = PhysicsSystem.toJVector(position);
            this.physicsBody.Orientation = PhysicsSystem.toJMatrix(this.orientationMatrix);
            this.game.physics.AddBody(this.physicsBody);


            this.basicEffect.EnableDefaultLighting();
            this.basicEffect.PreferPerPixelLighting = true;

            // this could add a fair performance penalty
            this.DebugDrawEnabled(true);
        }

        public override void Update(GameTime gametime)
        {
            var pos = PhysicsSystem.toVector3(this.physicsBody.Position);
            var orientation = PhysicsSystem.toMatrix(this.physicsBody.Orientation);
            //System.Diagnostics.Debug.WriteLine(pos);
            //System.Diagnostics.Debug.WriteLine(orientation);

            // each call to SetX recalculates the world matrix. This is inefficient and should be fixed.
            this.SetPosition(pos);
            this.SetOrientation(orientation);
            base.Update(gametime);
        }

        public override void Draw(GameTime gametime)
        {
            basicEffect.CurrentTechnique.Passes[0].Apply();
            basicEffect.World = this.worldMatrix;
            basicEffect.View = game.camera.view;
            basicEffect.Projection = game.camera.projection;

            this.model.Draw(game.GraphicsDevice, this.worldMatrix, game.camera.view, game.camera.projection, basicEffect);
            base.Draw(gametime);
        }
    }
}
