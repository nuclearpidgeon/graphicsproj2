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

    

    public class Ball : GameObject
    {
        
        public Ball(Project2Game game, Vector3 position, Vector3 scale, Vector3 orientation) : base(game) {
            this.SetScale(scale);
            this.SetOrientation(orientation);
            this.SetPosition(position);

            this.model = game.Content.Load<Model>("Models\\Knot");
            
            this.boundingSphere = this.model.CalculateBounds(this.worldMatrix);
            //Jitter.Collision.Octree = new Jitter.Collision.Octree(model.)
            this.physicsShape = new SphereShape(this.boundingSphere.Radius);
            this.physicsBody = new RigidBody(this.physicsShape);
            this.game.physics.AddBody(this.physicsBody);
        }

        public override void Update(GameTime gametime)
        {
            base.Update(gametime);
        }

        public override void Draw(GameTime gameTime)
        {
            // Apply the basic effect technique and draw the rotating cube
            //this.model.Draw(game.GraphicsDevice, this.worldMatrix, this.game.camera.view, this.game.camera.projection);

            base.Draw(gameTime);
        }
    }
}
