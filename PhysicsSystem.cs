using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jitter;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Jitter.Dynamics.Constraints;
using SharpDX;
using SharpDX.Toolkit;

using  Matrix = SharpDX.Matrix;
using  Vector3 = SharpDX.Vector3;
using  Vector4 = SharpDX.Vector4;


namespace Project2
{
    public class PhysicsSystem : GameSystem
    {

        public World World;
        Jitter.Collision.CollisionSystem collisionSystem = new Jitter.Collision.CollisionSystemSAP();
       

        public PhysicsSystem(Game game) : base(game) {

            World = new World(collisionSystem);// whole_new_world.wav
            World.Gravity = new JVector(0, -10, 0); // -10 m.s^-2

            
            //game.GameSystems.Add(this);
        }

        public void addTestBox(Vector3 position, float mass)
        {


            // these can actually be reused between objects to save memory
            Shape boxShape = new Jitter.Collision.Shapes.BoxShape(new JVector(1f, 1f, 1f));

            RigidBody body = new RigidBody(new BoxShape(new JVector(1.0f, 1.0f, 1.0f)));
            body.Position = new JVector(position.X, position.Y, position.Z);
            this.World.AddBody(body);
        }

        public void addCollisionShape() {
        }
    }
}
