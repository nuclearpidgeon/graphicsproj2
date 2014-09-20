﻿using System;
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

        public World World; // handles dynamics

        // collision system used by world (or on its own)
        Jitter.Collision.CollisionSystem collisionSystem = new Jitter.Collision.CollisionSystemSAP(); // SAP = Scan and Prune (good for large scenes, bruteforce might be fine for small scenes too)

        public PhysicsSystem(Game game) : base(game) {

            World = new World(collisionSystem); // whole_new_world.wav
            // gravity defaults to -9.8 m.s^-2
            //World.Gravity = new JVector(0, -1, 0); // -10 m.s^-2

            
            // caller should do this? 
            //game.GameSystems.Add(this);
            buildScene();
        }

        override public void Update(GameTime time) {
            World.Step((float)time.TotalGameTime.TotalSeconds, true, 1.0f/60.0f, 1);
        }

        private void buildScene() {
            addTestBox(new Vector3(0f, 10f, 0f), 1.0f);
            addTestBox(new Vector3(0.5f, 1f, 0f), 1.0f);

            Shape groundShape = new BoxShape(new JVector(10, 0f, 10));
            RigidBody groundBody = new RigidBody(groundShape);

            groundBody.Tag = Color.LightGreen;
            // make the body static, so it can't be moved
            groundBody.IsStatic = true;
            World.AddBody(groundBody);
            
        }


        public static Vector3 toVector3(JVector vector) {
            return new Vector3(vector.X, vector.Y, vector.Z);
        }

        public static Matrix toMatrix(JMatrix matrix) {
            return new Matrix(
                matrix.M11, matrix.M12, matrix.M13, 0f,
                matrix.M21, matrix.M22, matrix.M23, 0f,
                matrix.M31, matrix.M32, matrix.M33, 0f,
                        0f,         0f,         0f, 1.0f);
        }

        public void addTestBox(Vector3 position, float mass)
        {


            // these can actually be reused between objects to save memory
            Shape boxShape = new Jitter.Collision.Shapes.BoxShape(new JVector(1f, 1f, 1f));

            RigidBody body = new RigidBody(new BoxShape(new JVector(1.0f, 1.0f, 1.0f)));
            body.IsStatic = false;
            body.AffectedByGravity = true;
            body.Position = new JVector(position.X, position.Y, position.Z);
            this.World.AddBody(body);
        }

        public void addCollisionShape() {
        }
    }
}
