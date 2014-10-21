using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

using Jitter.Collision.Shapes;
using Jitter.Dynamics;

using Project2.GameObjects.Abstract;


namespace Project2.GameObjects
{
    class TexturedHeightMapTerrain : Terrain
    {
        /// <summary>
        /// Creates a static terrain mesh based on a heightmap texture.
        /// </summary>
        /// <param name="game"></param> 
        /// <param name="position"></param>
        /// <param name="heightMap"></param>
        /// <param name="scale"></param>
        public TexturedHeightMapTerrain(Project2Game game, Vector3 position, Texture2D heightMap, double scale)
            : base(game, GeneratePhysicsDescription(position, heightMap, scale, true))
        {
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
                Debug = true,
                RigidBody = rigidBody,
                Position = position
            };

            return description;
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
