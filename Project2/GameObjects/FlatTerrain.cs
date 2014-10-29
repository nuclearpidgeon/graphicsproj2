using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using SharpDX.Toolkit;

using Jitter.Collision.Shapes;
using Jitter.Dynamics;

using Project2.GameObjects.Abstract;

namespace Project2.GameObjects
{
    class FlatTerrain : Terrain
    {
        private int xScale, yScale;
        private float frontHeight, backHeight;
        /// <summary>
        /// Constructs a flat terrain
        /// </summary>
        /// <param name="game"></param>
        /// <param name="position"></param>
        /// <param name="xScale"></param>
        /// <param name="yScale"></param>
        public FlatTerrain(Project2Game game, Vector3 position, int xScale, int yScale, float frontHeight = 0.0f, float backHeight = 0.0f)
            : base(game, position)
        {
            this.xScale = xScale;
            this.yScale = yScale;
            this.frontHeight = frontHeight;
            this.backHeight = backHeight;
        }
        protected override RigidBody GeneratePhysicsDescription()
        {
            float[,] terrainData = new float[,] { 
                {frontHeight, backHeight} ,
                {frontHeight, backHeight}
            };
            var collisionShape = new TerrainShape(terrainData, (float)xScale, (float)yScale);
            var rigidBody = new RigidBody(collisionShape)
            {
                Position = PhysicsSystem.toJVector(Position),
                IsStatic = false,
                EnableDebugDraw = true,
            };

            return rigidBody;
        }
    }
}