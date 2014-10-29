using System;
using Jitter.Dynamics;
using SharpDX.Toolkit;

namespace Project2.GameObjects.Interface
{
    public interface IPhysicsObject
    {
        RigidBody PhysicsDescription { get; set; }
        Boolean ToDestroy { get; set; }
        void Destroy();
        void Draw(GameTime gametime);
        void Update(GameTime gametime);
    }
}
