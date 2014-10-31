using System;
using System.Threading;
using System.Collections.Generic;

using SharpDX;
using SharpDX.Toolkit;
using Texture2D = SharpDX.Toolkit.Graphics.Texture2D;

using Jitter.Collision.Shapes;
using Jitter.Dynamics;

using Project2.GameObjects.Abstract;

namespace Project2.GameObjects
{
    class DiamondSquareTerrain : Terrain
    {
        private System.Random rng;

        private int Density;
        public float Amplitude;
        /// <summary>
        /// Construct a static physics mesh terrain procedurally using diamond-square. Density is n, a square with width 2^n-1 will be generated.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="position">Bottom left corner of heightfield</param>
        /// <param name="density">Controls number of points in height field as square of side length 2^n-1 (e.g. 6)</param>
        /// <param name="scale">Distance between height field points</param>
        /// <param name="amplitude">Variance of height field</param>
        public DiamondSquareTerrain(Project2Game game, Vector3 position, int density, float xScale, float zScale, float amplitude)
            : base(game, position,xScale, zScale)
        {
            this.Density = density;
            this.Amplitude = amplitude;

            this.TerrainData = GenerateTerrainData();
            this.GenerateGeometry();
            this.CreateBuffers();
            this.PhysicsDescription = GeneratePhysicsDescription();
            game.physics.AddBody(PhysicsDescription);
        }
        public override float[,] TerrainData { get; set; }

        public override RigidBody PhysicsDescription { get; set; }

        protected override RigidBody GeneratePhysicsDescription()
        {
            var collisionShape = new TerrainShape(TerrainData, xScale, zScale);
            return new RigidBody(collisionShape)
            {
                Position = PhysicsSystem.toJVector(Position),
                IsStatic = true,
                EnableDebugDraw = false,
            };
        }
        protected override float[,] GenerateTerrainData()
        {
            var rng = new System.Random();

            this.terrainWidth = (1 << Density) + 1; // must be 2^n + 1
            //Console.WriteLine("Generating terrain with width {0}", terrainWidth);
            this.terrainHeight = terrainWidth;
            var terrainData = new float[terrainWidth, terrainHeight];


            // seed corners
            terrainData[0, 0] = rng.NextFloat(-1, 1) * Amplitude;
            terrainData[0, terrainHeight - 1] = rng.NextFloat(-1, 1) * Amplitude;
            terrainData[terrainWidth - 1, 0] = rng.NextFloat(-1, 1) * Amplitude;
            terrainData[terrainWidth - 1, terrainHeight - 1] = rng.NextFloat(-1f, 1f) * Amplitude;


            // a stride represents the size of the grid to be recursed upon
            // reduce aplitude by slightly more than half
            for (var stride = terrainWidth - 1; stride >= 2; stride /= 2, Amplitude *= 0.49f)  // change the amplitude scale coefficient for different terrain. 0 - flat terrain, 1 - spikes
            {

                var delta = stride / 2; // distance from midpoint to closest edge of chunk

                var chunk = terrainWidth / stride; // number of chunks at this iteration
                // iterate over the chunks of terrain at each stride length
                // initially there is one chunk comprising the entire terrain, then increases geometrically to 4, 16, etc.
                for (var x = 0; x < chunk; x++)
                {
                    for (var y = 0; y < chunk; y++)
                    {
                        // "diamond"
                        // the midpoint of the current chunk is the average of the corners surrounding it
                        var bottomLeft = terrainData[x * stride, y * stride]; //bottom left
                        var bottomRight = terrainData[(x + 1) * stride, y * stride]; //bottom right
                        var topLeft = terrainData[x * stride, (y + 1) * stride]; //top left
                        var topRight = terrainData[(x + 1) * stride, (y + 1) * stride]; //top right
                        var average = (bottomLeft + bottomRight + topLeft + topRight) / 4;
                        // setting the midpoint completes a diamond when the chunks are tesselated
                        // multiply by sqrt(2) because the offset should be scaled based on the distance of the midpoint to the sampled points
                        terrainData[x * stride + delta, y * stride + delta] = average + (rng.NextFloat(-1, 1)) * Amplitude * (float)Math.Sqrt(2);

                        // "square"
                        // from the midpoint, points delta distance away in the north, south, east and west directions need to be set
                        // as this is tesselated, setting the south (bottom) and left (west) points sets the top (north) and right (east) of the conjoined chunk

                        terrainData[x * stride + delta, y * stride] = ((bottomLeft + bottomRight) / 2) + rng.NextFloat(-1f, 1f) * Amplitude; // south
                        terrainData[x * stride, y * stride + delta] = (topLeft + bottomLeft) / 2 + rng.NextFloat(-1f, 1f) * Amplitude; // east
                        // the above statement is not true at the sides, however, so explicitly deal with those.
                        // there is room for improvement here in that the terrain could be made to "Wrap arround" - might be useful in wrapping the terrain around a sphere or something...
                        if (y == chunk - 1) // if at edge...
                        {
                            terrainData[x * stride + delta, (y + 1) * stride] = (topLeft + topRight) / 2 + rng.NextFloat(-1f, 1f) * Amplitude; // north
                        }
                        if (x == chunk - 1)
                        {
                            terrainData[(x + 1) * stride, y * stride + delta] = (topRight + bottomRight) / 2 + rng.NextFloat(-1f, 1f) * Amplitude; // east
                        }
                    }
                }
            }

            return terrainData;

        }
    }
}
