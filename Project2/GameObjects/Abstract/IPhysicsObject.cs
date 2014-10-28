using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using SharpDX;
using System;

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
    public interface IPhysicsObject
    {
        PhysicsDescription PhysicsDescription { get; set; }
        void Destroy();
        void Draw(SharpDX.Toolkit.GameTime gametime);
        void Update(SharpDX.Toolkit.GameTime gametime);
    }
}
