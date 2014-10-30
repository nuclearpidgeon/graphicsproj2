using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Project2.GameObjects.Abstract;
using SharpDX;

namespace Project2.GameObjects
{
    using SharpDX.Toolkit.Graphics;
    using Jitter.Collision.Shapes;
    using Jitter.Dynamics;
    class HeightMapTerrain : Terrain
    {
        public override float[,] TerrainData { get; set; }
        public override Jitter.Dynamics.RigidBody PhysicsDescription { get; set; }
        private Texture2D HeightMap;

        public HeightMapTerrain(Project2Game game, Vector3 position, Texture2D heightMap, float xScale, float zScale) :
            base(game,position,xScale,zScale)
        {
            this.HeightMap = heightMap;
            this.TerrainData = GenerateTerrainData();
            this.GenerateGeometry();
            this.PhysicsDescription = GeneratePhysicsDescription();
            game.physics.AddBody(PhysicsDescription);
        }

        protected override Jitter.Dynamics.RigidBody GeneratePhysicsDescription()
        {
            var collisionShape = new TerrainShape(TerrainData, xScale, zScale);
            return new RigidBody(collisionShape)
            {
                Position = PhysicsSystem.toJVector(Position),
                IsStatic = true,
                EnableDebugDraw = false,
            };
        }

        /// <summary>
        /// Process a heightmap into a 2D array of integers representing the terrain data.
        /// Set the terrain object's height and width parameters.
        /// </summary>
        protected override float[,] GenerateTerrainData()
        {
            /* Code here is based upon: http://www.riemers.net/eng/Tutorials/XNA/Csharp/Series1/Terrain_from_file.php
             * 
             * I'm not sure of a more 'unique' way to load an image to a heightmap */

            // get terrain size
            this.terrainWidth = HeightMap.Width;
            this.terrainHeight = HeightMap.Height;
            // allocate array for resultant data
            float[,] terrainData = new float[terrainWidth, terrainHeight];

            // make a 1D array to hold all pixel data in, because Texture2D.GetData() works like that
            var colourmap = new Color[terrainHeight * terrainWidth];
            HeightMap.GetData(colourmap); // get pixel data, put it in 1D array

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
