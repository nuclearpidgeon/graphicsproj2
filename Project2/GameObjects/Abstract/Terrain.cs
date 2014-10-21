using System;
using System.Threading;

using SharpDX;
using SharpDX.Toolkit;
using Texture2D = SharpDX.Toolkit.Graphics.Texture2D;

using Jitter.Collision.Shapes;
using Jitter.Dynamics;

using Project2.GameObjects.Abstract;

namespace Project2.GameObjects
{
    abstract class Terrain : PhysicsObject
    {
        public float[,] TerrainData;
        protected int terrainWidth;
        protected int terrainHeight;
        protected float maxHeight = 0;
        protected float minHeight = 0;
        //private Texture2D heightMap;

        private int[] indices_list;
        private VertexPositionNormalColor[] vertex_list;

        protected Terrain(Project2Game game, PhysicsDescription physicsDescription)
            : base(game,physicsDescription)
        {
            // this is needed for subclasses
        }

    }
}
