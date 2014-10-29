using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using SharpDX;
using SharpDX.Toolkit;
using System;

namespace Project2.GameObjects.Abstract
{
    public interface IPhysicsObject
    {
        RigidBody PhysicsDescription { get; set; }
        void Destroy();
        void Draw(GameTime gametime);
        void Update(GameTime gametime);
    }
}
