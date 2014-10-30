using System;
using Jitter.Dynamics;
using SharpDX.Toolkit;

namespace Project2.GameObjects.Interface
{
    public interface IPhysicsObject
    {
        RigidBody PhysicsDescription { get; set; }
        void Destroy(Boolean Async = false);
        void Draw(GameTime gametime);
        void Update(GameTime gametime);
    }
}
