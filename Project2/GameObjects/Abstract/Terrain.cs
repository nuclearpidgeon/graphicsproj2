using System;

using SharpDX;
using SharpDX.Toolkit;

using Jitter.Collision.Shapes;
using Jitter.Dynamics;

namespace Project2.GameObjects.Abstract
{
    abstract class Terrain : GameObject, IPhysicsObject
    {
        public float[,] TerrainData;
        protected int terrainWidth;
        protected int terrainHeight;
        protected float maxHeight = 0;
        protected float minHeight = 0;

        private int[] indices_list;
        private VertexPositionNormalColor[] vertex_list;

        public RigidBody PhysicsDescription { get; set; }

        protected Terrain(Project2Game game, Vector3 position)
            : base(game,position)
        {
            this.PhysicsDescription = GeneratePhysicsDescription();
            this.Position = PhysicsSystem.toVector3(PhysicsDescription.Position);

            game.physics.AddBody(PhysicsDescription);
        }

        protected abstract RigidBody GeneratePhysicsDescription();

        public override void Draw(SharpDX.Toolkit.GameTime gametime)
        {
            if (PhysicsDescription.EnableDebugDraw && PhysicsDescription != null)
            {
                PhysicsDescription.DebugDraw(game.debugDrawer);
            }
            base.Draw(gametime);
        }

        public void Destroy()
        {
            throw new NotImplementedException();
        }
    }
}