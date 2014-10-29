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
    class Terrain : ModelPhysicsObject
    {
        public float[,] TerrainData;
        private int terrainWidth;
        private int terrainHeight;
        private float maxHeight = 0;
        private float minHeight = 0;
        //private Texture2D heightMap;

        private int[] indices_list;
        private VertexPositionNormalColor[] vertex_list;
        private System.Random rng;


        /// <summary>
        /// Creates a static terrain mesh based on a heightmap texture.
        /// </summary>
        /// <param name="game"></param> 
        /// <param name="position"></param>
        /// <param name="heightMap"></param>
        /// <param name="scale"></param>
        public Terrain(Project2Game game, Vector3 position, Texture2D heightMap, double scale)
            : this(game, GeneratePhysicsDescription(position, heightMap, scale, true))
        {
        }


        /// <summary>
        /// Construct a static physics mesh terrain procedurally using diamond-square. Density is n, a square with width 2^n-1 will be generated.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="position">Bottom left corner of heightfield</param>
        /// <param name="density">Controls number of points in height field as square of side length 2^n-1 (e.g. 6)</param>
        /// <param name="scale">Distance between height field points</param>
        /// <param name="amplitude">Variance of height field</param>
        public Terrain(Project2Game game, Vector3 position, int density, double scale, double amplitude)
            : this(game, GeneratePhysicsDescription(position, density, scale, amplitude, true))
        {

        }

        /// <summary>
        /// Constructs a flat terrain
        /// </summary>
        /// <param name="game"></param>
        /// <param name="position"></param>
        /// <param name="xScale"></param>
        /// <param name="yScale"></param>
        public Terrain(Project2Game game, Vector3 position, int xScale, int yScale, float frontHeight = 0.0f, float backHeight = 0.0f) 
            : this(game, GeneratePhyicsDescription(position, xScale, yScale, frontHeight, backHeight, true))
        {

        }

        private Terrain(Project2Game game, PhysicsDescription physicsDescription) 
            : base(game, physicsDescription)
        {       
            // This is the old super constructor call. Needs to be updated
        }

        /// <summary>
        /// Static method to construct physics description
        /// </summary>
        /// <param name="position"></param>
        /// <param name="heightMapTexture"></param>
        /// <param name="scale"></param>
        /// <param name="isStatic"></param>
        /// <returns></returns>
        private static PhysicsDescription GeneratePhysicsDescription(Vector3 position, Texture2D heightMapTexture, double scale, Boolean isStatic)
        {
            var terrainData = ProcessHeightMap(heightMapTexture);
            var minHeight = 255f;
            foreach (var h in terrainData)
            {
                if (h < minHeight)
                {
                    minHeight = h;
                }
            }
            var collisionShape = new TerrainShape(terrainData, (float) scale, (float) scale);
            var rigidBody = new RigidBody(collisionShape)
            {
                Position = PhysicsSystem.toJVector(position - new Vector3(0f, 255f, 0f)),
                IsStatic = isStatic,
                EnableDebugDraw = true,
            };

            var description = new PhysicsDescription()
            {
                IsStatic = isStatic,
                CollisionShape = collisionShape,
                IsDebugDrawable = true,
                RigidBody = rigidBody,
                Position = position
            };

            return description;
        }

        private static PhysicsDescription GeneratePhysicsDescription(Vector3 position, int density, double scale, double amplitude, Boolean isStatic)
        {
            var terrainData = GenerateDiamondSquare(density, (float)amplitude);
            var collisionShape = new TerrainShape(terrainData, (float) scale, (float) scale);
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
                IsDebugDrawable = true,
                RigidBody = rigidBody,
                Position = position
            };

            return description;
        }

        private static PhysicsDescription GeneratePhyicsDescription(Vector3 position, int xScale, int yScale, float frontHeight, float backHeight, bool isStatic)
        {
            float[,] terrainData = new float[,] { 
                {frontHeight, backHeight} ,
                {frontHeight, backHeight}
            };
            var collisionShape = new TerrainShape(terrainData, (float) xScale, (float) yScale);
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
                IsDebugDrawable = true,
                RigidBody = rigidBody,
                Position = position
            };
            
            return description;
        }

        public static float[,] GenerateDiamondSquare(int n, float amplitude)
        {
            var rng = new System.Random();

            var terrainWidth = (1 << n) + 1; // must be 2^n + 1
            //Console.WriteLine("Generating terrain with width {0}", terrainWidth);
            var terrainHeight = terrainWidth;
            var terrainData = new float[terrainWidth, terrainHeight];


            // seed corners
            terrainData[0, 0] = rng.NextFloat(-1, 1) * amplitude;
            terrainData[0, terrainHeight - 1] = rng.NextFloat(-1, 1) * amplitude;
            terrainData[terrainWidth - 1, 0] = rng.NextFloat(-1, 1) * amplitude;
            terrainData[terrainWidth - 1, terrainHeight - 1] = rng.NextFloat(-1f, 1f) * amplitude;


            // a stride represents the size of the grid to be recursed upon
            // reduce aplitude by slightly more than half
            for (var stride = terrainWidth - 1; stride >= 2; stride /= 2, amplitude *= 0.49f)  // change the amplitude scale coefficient for different terrain. 0 - flat terrain, 1 - spikes
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
                        terrainData[x * stride + delta, y * stride + delta] = average + (rng.NextFloat(-1, 1)) * amplitude * (float)Math.Sqrt(2);

                        // "square"
                        // from the midpoint, points delta distance away in the north, south, east and west directions need to be set
                        // as this is tesselated, setting the south (bottom) and left (west) points sets the top (north) and right (east) of the conjoined chunk

                        terrainData[x * stride + delta, y * stride] = ((bottomLeft + bottomRight) / 2) + rng.NextFloat(-1f, 1f) * amplitude; // south
                        terrainData[x * stride, y * stride + delta] = (topLeft + bottomLeft) / 2 + rng.NextFloat(-1f, 1f) * amplitude; // east
                        // the above statement is not true at the sides, however, so explicitly deal with those.
                        // there is room for improvement here in that the terrain could be made to "Wrap arround" - might be useful in wrapping the terrain around a sphere or something...
                        if (y == chunk - 1) // if at edge...
                        {
                            terrainData[x * stride + delta, (y + 1) * stride] = (topLeft + topRight) / 2 + rng.NextFloat(-1f, 1f) * amplitude; // north
                        }
                        if (x == chunk - 1)
                        {
                            terrainData[(x + 1) * stride, y * stride + delta] = (topRight + bottomRight) / 2 + rng.NextFloat(-1f, 1f) * amplitude; // east
                        }
                    }
                }
            }

            return terrainData;

        }

        /// <summary>
        /// Decides on a colour for terrain based on height and some noise
        /// </summary>
        /// <param name="height"></param>
        /// <param name="minHeight"></param>
        /// <param name="maxHeight"></param>
        /// <returns></returns>
        private Color getTerrainColour(float height, float minHeight, float maxHeight)
        {
            //var heightRange = maxHeight - minHeight;
            //height += 0.05f * rng.NextFloat(-heightRange, heightRange); // add 1% noise to height

            //// Look up table of colours to interpolate between
            //var LUT = new SortedList<float, Color>();
            //LUT.Add(0.03f, Color.DarkSlateGray * 0.1f);
            //LUT.Add(0.06f, Color.DarkBlue);
            //LUT.Add(0.07f, Color.DeepSkyBlue);
            //LUT.Add(0.2f, Color.SandyBrown);
            //LUT.Add(0.3f, Color.SaddleBrown);
            //LUT.Add(0.4f, Color.ForestGreen);
            //LUT.Add(0.5f, Color.LightSlateGray);
            //LUT.Add(1.0f, Color.Snow);

            //Color output = LUT.First().Value; // initialise the output colour to first value in table
            //try  // gross hack
            //{
            //    output = LUT.First(x => heightRange * x.Key >= height).Value;
            //}
            //catch (Exception)
            //{

            //    output = LUT.First().Value;
            //}


            //return output;
            return Color.Red;
        }



        /// <summary>
        /// Process a heightmap into a 2D array of integers representing the terrain data.
        /// Set the terrain object's height and width parameters.
        /// </summary>
        /// <param name="heightmap"></param>
        /// <returns></returns>
        private static float[,] ProcessHeightMap(Texture2D heightMap)
        {
            /* Code here is based upon: http://www.riemers.net/eng/Tutorials/XNA/Csharp/Series1/Terrain_from_file.php
             * 
             * I'm not sure of a more 'unique' way to load an image to a heightmap */

            // get terrain size
            var terrainWidth = heightMap.Width;
            var terrainHeight = heightMap.Height;
            // allocate array for resultant data
            var terrainData = new float[terrainWidth, terrainHeight];


            // make a 1D array to hold all pixel data in, because Texture2D.GetData() works like that
            var colourmap = new Color[terrainHeight * terrainWidth];
            heightMap.GetData(colourmap); // get pixel data, put it in 1D array

            // rework that 1D array into 2D array, taking value of pixel as height
            for (var y = 0; y < terrainHeight; y++)
            {
                for (var x = 0; x < terrainWidth; x++)
                {
                    // take the green value (0-255) as height data. heightmap should be black and white image, so it doesn't really matter.
                    // scale height down by a factor because 255 height is really high
                    terrainData[x, y] = colourmap[y * terrainHeight + x].G / 7.0f;
                }
            }

            return terrainData;
        }
    }
}
