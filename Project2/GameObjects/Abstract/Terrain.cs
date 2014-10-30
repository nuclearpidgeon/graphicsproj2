using System;
using Project2.GameObjects.Interface;
using SharpDX;
using SharpDX.Toolkit;

using Jitter.Collision.Shapes;
using Jitter.Dynamics;


namespace Project2.GameObjects.Abstract
{
    using SharpDX.Toolkit.Graphics;
    abstract class Terrain : GameObject, IPhysicsObject
    {
        public abstract float[,] TerrainData { get; set; }
        protected int terrainWidth;
        protected int terrainHeight;
        protected float xScale;
        protected float zScale;
        protected float maxHeight = 0;
        protected float minHeight = 0;
        // Graphics components
        protected int[] Indices;
        protected VertexPositionNormalColor[] Vertices;
        protected Buffer VertexBuffer;
        protected Buffer<int> IndexBuffer;

        public abstract RigidBody PhysicsDescription { get; set; }

        protected Terrain(Project2Game game, Vector3 position, float xScale, float zScale)
            : base(game,position)
        {
            this.xScale = xScale;
            this.zScale = zScale;
            basicEffect.VertexColorEnabled = true;
            // These will have to happen in the subclasses because of needing access to subclass constructor arguments/fields
            //this.TerrainData = GenerateTerrainData();
            //this.PhysicsDescription = GeneratePhysicsDescription();
            //this.Position = PhysicsSystem.toVector3(PhysicsDescription.Position);
            //game.physics.AddBody(PhysicsDescription);
        }

        protected abstract RigidBody GeneratePhysicsDescription();

        protected abstract float[,] GenerateTerrainData();

        protected void GenerateGeometry()
        {
            int index = 0; // vertex index counter, used to create index buffer
            Indices = new int[(terrainWidth - 1) * (terrainHeight - 1) * (2 * 3)]; // each quad requires 2 tris, each tri requires 3 vertices
            Vertices = new VertexPositionNormalColor[terrainWidth * terrainHeight];

            // create a vertex for each point on the terrain grid
            for (int z = 0; z < terrainWidth; z++)
            {
                for (int x = 0; x < terrainHeight; x++)
                {

                    // create a vertex for each point on the terrain square grid
                    // vertex list is a 1D representation of the 2D terrain
                    Vertices[x + z * terrainWidth].Position = this.Position + new Vector3(z*xScale, TerrainData[z, x], x*zScale);
                    //vertex_list[x + z * terrainWidth].Color = getTerrainColour(TerrainData[x, z], (float)minHeight, (float)maxHeight);
                    Vertices[x + z * terrainWidth].Color = colorFromHeight(TerrainData[z, x]);
                    Vertices[x + z * terrainWidth].Normal = Vector3.Zero;

                    // add vertex indices to index buffer, creating two triangles for each quad, forming a terrain mesh grid
                    // visit vertices in order that creates CW triangle and store that path in index list
                    if (z < (terrainWidth - 1) && x < (terrainWidth - 1)) // check bounds - we fill from top left corner 
                    {
                        int upperLeft = z * terrainWidth + x;
                        int upperRight = upperLeft + 1;
                        int bottomLeft = upperLeft + terrainWidth;
                        int bottomRight = bottomLeft + 1;

                        // first triangle
                        Indices[index++] = upperLeft;
                        Indices[index++] = bottomLeft;
                        Indices[index++] = upperRight;

                        // second triangle
                        Indices[index++] = upperRight;
                        Indices[index++] = bottomLeft;
                        Indices[index++] = bottomRight;

                    }
                }
            }
            // each group of 3 in the index buffer represents a triangle
            for (int i = 0; i < Indices.Length; i += 3)
            {
                // take 3 indices
                var i1 = Indices[i];
                var i2 = Indices[i + 1];
                var i3 = Indices[i + 2];

                // look up their corresponding vertices such that you get the points of a triangle
                var p1 = Vertices[Indices[i]];
                var p2 = Vertices[Indices[i + 1]];
                var p3 = Vertices[Indices[i + 2]];

                // the normal vector to a plane is the cross product of two orthonormal vectors on the plane
                var U = p1.Position - p2.Position;
                var V = p1.Position - p3.Position;
                var N = Vector3.Cross(V, U); //Vector3.Cross(side1, side2);

                // accumulate normals for each vertex for later averaging
                Vertices[i1].Normal += N;
                Vertices[i2].Normal += N;
                Vertices[i3].Normal += N;
            }

            // normalise the summed normals to get the average normal
            // this produces a normal that is the average of the normals to the triangle faces surrounding the vertex and is smoother over the terrain
            // not super sure if this needs to be done, but I guess IEEE754 floats are more accurate around 0 after being manipulated by shaders, etc
            for (int i = 0; i < Vertices.Length; i++)
            {
                Vertices[i].Normal.Normalize();
            }            
        }

        protected void CreateBuffers()
        {
            // Finally, create the buffers
            this.VertexBuffer = Buffer.Vertex.New(
                game.GraphicsDevice,
                Vertices
            );
            this.IndexBuffer = Buffer.Index.New(
                game.GraphicsDevice,
                Indices
            );
            this.inputLayout = VertexInputLayout.FromBuffer<VertexPositionNormalColor>(0, (Buffer<VertexPositionNormalColor>)VertexBuffer);
        }

        public override void Draw(SharpDX.Toolkit.GameTime gametime)
        {
            if (PhysicsDescription.EnableDebugDraw && PhysicsDescription != null)
            {
                PhysicsDescription.DebugDraw(game.debugDrawer);
            }
            game.GraphicsDevice.SetVertexBuffer((Buffer<VertexPositionNormalColor>)VertexBuffer);
            game.GraphicsDevice.SetIndexBuffer(IndexBuffer, true);
            game.GraphicsDevice.SetVertexInputLayout(inputLayout);

            // Not sure if this makes a difference?
            RasterizerState rs = SharpDX.Toolkit.Graphics.RasterizerState.New(game.GraphicsDevice,
                new SharpDX.Direct3D11.RasterizerStateDescription()
                {
                    CullMode = SharpDX.Direct3D11.CullMode.Back,
                    FillMode = SharpDX.Direct3D11.FillMode.Solid,
                });
            game.GraphicsDevice.SetRasterizerState(rs);
            
            // Apply BasicEffect stuff in superclass
            base.Draw(gametime);
            
            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                game.GraphicsDevice.DrawIndexed(PrimitiveType.TriangleList, IndexBuffer.ElementCount);

            }
        }

        /// <summary>
        /// Converts a height value between 0 and 1 into a color, based on a terrain height color scheme
        /// </summary>
        /// <param name="heightval">The height value between 0.0f and 1.0f to convert into a color</param>
        /// <returns>SharpDX.Color struct with the appropriate color data for the given height value</returns>
        protected virtual Color colorFromHeight(float heightval)
        {
            Color color;
            Random rand = new Random(System.DateTime.Now.Millisecond);
            float waterlevel = 0.3f;
            //heightval += rand.NextFloat(-0.05f,0.05f);

            // setup colors
            Vector3 brown = new Vector3(150f, 120f, 90f) / 255f;
            Vector3 green = Color.ForestGreen.ToVector3();
            Vector3 white = Color.White.ToVector3();
            Vector3 stone = new Vector3(71f / 255, 79f / 255, 100f / 255);
            Vector3 black = Color.Black.ToVector3();

            // shift scaling float to between 0.0f and 1.0f

            if (heightval < waterlevel)
            {
                // decay from brown to black
                color = interpolateColors(black, brown, heightval / waterlevel);
            }
            else
            {
                // adjust to 0->1 scale 
                float newScale = (heightval - waterlevel) / (1 - waterlevel);
                if (newScale < 0.25f)
                {
                    color = interpolateColors(brown, green, newScale / 0.25f);
                }
                else if (newScale < 0.50f)
                {
                    color = interpolateColors(green, stone, (newScale - 0.25f) / 0.25f);
                }
                else
                {
                    color = interpolateColors(stone, white, (newScale - 0.5f) / 0.5f);
                }
                //color = interpolateColors(brown, white, newScale);
            }

            return color;
        }

        /// <summary>
        /// Returns a color at a certain amount between two given colors in the RGB space
        /// </summary>
        /// <param name="colorA">Color to start at (returned if 0.0f distance given)</param>
        /// <param name="colorB">Color to end at (returned if 1.0f distance given)</param>
        /// <param name="distance">Float value between 0 and 1</param>
        /// <returns></returns>
        private Color interpolateColors(Vector3 colorA, Vector3 colorB, float distance)
        {
            if (distance < 0.0f)
            {
                //Console.WriteLine("Invalid color value " + distance + " supplied for interpolation, clipping to 0");
                distance = 0.0f;
            }
            else if (distance > 1.0f)
            {
                //Console.WriteLine("Invalid color value " + distance + " supplied for interpolation, clipping to 1");
                distance = 1.0f;
            }
            // calculate distance vector from A -> B
            Vector3 diff = colorB - colorA;
            return new Color(colorA + (distance * diff));
        }

        public void Destroy()
        {
            this.Parent.RemoveChild(this);
        }

        public bool ToDestroy
        {
            get { return ToDestroy; }
            set { ToDestroy = value; }
        }
    }
}