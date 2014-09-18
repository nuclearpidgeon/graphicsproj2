using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BulletSharp;
using SharpDX;
using SharpDX.Toolkit;

using  Matrix = SharpDX.Matrix;
using  Vector3 = SharpDX.Vector3;
using  Vector4 = SharpDX.Vector4;


namespace Project2
{
    class PhysicsSystem: GameSystem
    {

        public DynamicsWorld World;

        CollisionDispatcher Dispatcher;
        BroadphaseInterface Broadphase;
        //ConstraintSolver Solver;
        AlignedCollisionShapeArray CollisionShapes;
        CollisionConfiguration collisionConf;

        public PhysicsSystem(Game game) : base(game) {

            // collision configuration contains default setup for memory, collision setup
            collisionConf = new DefaultCollisionConfiguration();
            Dispatcher = new CollisionDispatcher(collisionConf);
            Broadphase = new DbvtBroadphase();

            World = new DiscreteDynamicsWorld(Dispatcher, Broadphase, null, collisionConf); // whole_new_world.wav
            World.Gravity = new Vector3(0, -10, 0);
            CollisionShapes = new AlignedCollisionShapeArray();
            game.GameSystems.Add(this);
        }

        public void addTestBox(Vector3 position) {
            float mass = 1.0f;

            // these can actually be reused between objects to save memory
            CollisionShape colShape = new BoxShape(1);
            CollisionShapes.Add(colShape);
            
            Vector3 localInertia = colShape.CalculateLocalInertia(mass);
            
            Matrix startTransform = Matrix.Translation(position);
            DefaultMotionState myMotionState = new DefaultMotionState(startTransform);
            RigidBodyConstructionInfo rbInfo = new RigidBodyConstructionInfo(mass, myMotionState, colShape, localInertia);
            RigidBody body = new RigidBody(rbInfo);

            // make it drop from a height
            body.Translate(new SharpDX.Vector3(0, 20, 0));
            World.AddRigidBody(body);
        }
        public void addCollisionShape() {
        }
    }
}
