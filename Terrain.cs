using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.Toolkit;



namespace Project2
{

    using SharpDX.Toolkit.Graphics;


    public class Terrain : GameObject
    {

        Vector3 modelRotation = Vector3.Zero;
        Vector3 modelPosition = Vector3.Zero;
        Vector3 rotationVelocity = Vector3.Zero;
        Vector3 positionVelocity = Vector3.Zero;

        public float[,] terrainData;
        private int terrainWidth;
        private int terrainHeight;
        private float maxHeight = 0;
        private float minHeight = 0;
        //private Texture2D heightMap;

        private int[] indices_list;
        private VertexPositionNormalColor[] vertex_list;
        private System.Random rng;




        /// <summary>
        /// Construct square terrain of width 2^n + 1 using procedural generation (diamond-square algorithm)
        /// </summary>
        /// <param name="game"></param>
        /// <param name="worldMatrix"></param>
        public Terrain(Project2Game game) : base(game)
        {
            Console.WriteLine("Creating terrain.");
            //this.worldMatrix = worldMatrix;

            // Initialise random number generator
            this.rng = new System.Random(); // automagically seeded with current time

            // heightmap to generate from
            //var textureName = "heightmap.bmp";
            //heightMap = game.Content.Load<Texture2D>(textureName);

            // turn texture into square array
            //this.terrainData = ProcessHeightMap(heightMap);
            this.terrainData = GenerateDiamondSquare(6, 4f);


            // recenter over origin
            modelPosition = new Vector3(-terrainWidth / 2.0f, 0, -terrainHeight / 2.0f);



            Initialise();
        }

        public override void Draw(GameTime t) { 
        }

        private void Initialise()
        {
            // I couldn't get stuff I was initialising in the class constructor to work here. Maybe I'm missing something about C#
        }

        public float getHeight(Vector3 position)
        {
            const float defaultHeight = -100000; // height to return when out of map bounds
            position += new Vector3((terrainWidth / 2.0f) + 0.5f, 0, (terrainHeight / 2.0f) + 0.5f); // center camera coordinates over terrain coordinates
            if ((position.X >= 1 && position.X <= (terrainWidth - 1)) && (position.Z >= 1 && position.Z <= (terrainHeight - 1)))
            {
                // note that these are not the "closest" points on terrain, but truncated floats (1.9999 returns point at 1, not 2)
                // probably should use a different rounding strategy but meh...
                return terrainData[(int)position.Z, (int)position.X];
            }
            else
            {
                return defaultHeight;
            }
        }


        public float[,] GenerateDiamondSquare(int n, float amplitude)
        {

            terrainWidth = (1 << n) + 1; // must be 2^n + 1
            Console.WriteLine("Generating terrain with width {0}", terrainWidth);
            terrainHeight = terrainWidth;
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

            // find minimum and maximum heights of generated terrain
            this.maxHeight = terrainData[0, 0];
            this.minHeight = terrainData[0, 0];
            for (int x = 0; x < terrainWidth; x++)
            {
                for (int y = 0; y < terrainHeight; y++)
                {
                    if (terrainData[x, y] > maxHeight)
                    {
                        maxHeight = terrainData[x, y];
                    }
                    if (terrainData[x, y] < minHeight)
                    {
                        minHeight = terrainData[x, y];
                    }
                }
            }
            Console.WriteLine("Finished generating terrain!");

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
            var heightRange = maxHeight - minHeight;
            height += 0.05f * rng.NextFloat(-heightRange, heightRange); // add 1% noise to height

            // Look up table of colours to interpolate between
            var LUT = new SortedList<float, Color>();
            LUT.Add(0.03f, Color.DarkSlateGray * 0.1f);
            LUT.Add(0.06f, Color.DarkBlue);
            LUT.Add(0.07f, Color.DeepSkyBlue);
            LUT.Add(0.2f, Color.SandyBrown);
            LUT.Add(0.3f, Color.SaddleBrown);
            LUT.Add(0.4f, Color.ForestGreen);
            LUT.Add(0.5f, Color.LightSlateGray);
            LUT.Add(1.0f, Color.Snow);

            Color output = LUT.First().Value; // initialise the output colour to first value in table
            try  // gross hack
            {
                output = LUT.First(x => heightRange * x.Key >= height).Value;
            }
            catch (Exception)
            {

                output = LUT.First().Value;
            }


            return output;
        }



        /// <summary>
        /// Process a heightmap into a 2D array of integers representing the terrain data.
        /// Set the terrain object's height and width parameters.
        /// </summary>
        /// <param name="heightmap"></param>
        /// <returns></returns>
        private float[,] ProcessHeightMap(Texture2D heightMap)
        {
            /* Code here is based upon: http://www.riemers.net/eng/Tutorials/XNA/Csharp/Series1/Terrain_from_file.php
             * 
             * I'm not sure of a more 'unique' way to load an image to a heightmap */

            // get terrain size
            this.terrainWidth = heightMap.Width;
            this.terrainHeight = heightMap.Height;
            // allocate array for resultant data
            var terrainData = new float[terrainWidth, terrainHeight];


            // make a 1D array to hold all pixel data in, because Texture2D.GetData() works like that
            Color[] colourmap = new Color[terrainHeight * terrainWidth];
            heightMap.GetData(colourmap); // get pixel data, put it in 1D array

            // rework that 1D array into 2D array, taking value of pixel as height
            for (int y = 0; y < terrainHeight; y++)
            {
                for (int x = 0; x < terrainWidth; x++)
                {
                    // take the green value (0-255) as height data. heightmap should be black and white image, so it doesn't really matter.
                    // scale height down by a factor because 255 height is really high
                    terrainData[x, y] = colourmap[y * terrainHeight + x].G / 7;
                }
            }

            return terrainData;
        }
    }
}
