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
        /// <summary>
        /// Constructs a flat terrain
        /// </summary>
        /// <param name="game"></param>
        /// <param name="position"></param>
        /// <param name="xScale"></param>
        /// <param name="yScale"></param>
        public FlatTerrain(Project2Game game, Vector3 position, int xScale, int yScale, float frontHeight = 0.0f, float backHeight = 0.0f) 
            : base(game, GeneratePhyicsDescription(position, xScale, yScale, frontHeight, backHeight, true))
        {

        }
        private static PhysicsDescription GeneratePhyicsDescription(Vector3 position, int xScale, int yScale, float frontHeight, float backHeight, bool isStatic)
        {
            float[,] terrainData = new float[,] { 
                {frontHeight, backHeight} ,
                {frontHeight, backHeight}
            };
            var collisionShape = new TerrainShape(terrainData, (float)xScale, (float)yScale);
            var rigidBody = new RigidBody(collisionShape)
            {
                Position = PhysicsSystem.toJVector(position),
                IsStatic = isStatic,
                EnableDebugDraw = true,
            };
            var description = new PhysicsDescription()
            {
                IsStatic = isStatic,
                CollisionShape = collisionShape,
                Debug = true,
                RigidBody = rigidBody,
                Position = position
            };

            return description;
        }
    }
}
