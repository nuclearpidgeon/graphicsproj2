﻿using System;
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
        protected float maxHeight = 0;
        protected float minHeight = 0;
        // Graphics components
        protected int[] indices_list;
        protected VertexPositionNormalColor[] vertex_list;
        protected Buffer vertices;
        protected Buffer<int> indices;

        public abstract RigidBody PhysicsDescription { get; set; }

        protected Terrain(Project2Game game, Vector3 position)
            : base(game,position)
        {
            // These will probably have to happen in the subclasses because of needing access to subclass constructor arguments/fields
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
            indices_list = new int[(terrainWidth - 1) * (terrainHeight - 1) * (2 * 3)]; // each quad requires 2 tris, each tri requires 3 vertices
            vertex_list = new VertexPositionNormalColor[terrainWidth * terrainHeight];

            // create a vertex for each point on the terrain grid
            for (int z = 0; z < terrainWidth; z++)
            {
                for (int x = 0; x < terrainHeight; x++)
                {

                    // create a vertex for each point on the terrain square grid
                    // vertex list is a 1D representation of the 2D terrain
                    vertex_list[x + z * terrainWidth].Position = new Vector3(z, TerrainData[x, z], x);
                    //vertex_list[x + z * terrainWidth].Color = getTerrainColour(TerrainData[x, z], (float)minHeight, (float)maxHeight);
                    vertex_list[x + z * terrainWidth].Color = Color.Red;
                    vertex_list[x + z * terrainWidth].Normal = Vector3.Zero;

                    // add vertex indices to index buffer, creating two triangles for each quad, forming a terrain mesh grid
                    // visit vertices in order that creates CW triangle and store that path in index list
                    if (z < (terrainWidth - 1) && x < (terrainWidth - 1)) // check bounds - we fill from top left corner 
                    {
                        int upperLeft = z * terrainWidth + x;
                        int upperRight = upperLeft + 1;
                        int bottomLeft = upperLeft + terrainWidth;
                        int bottomRight = bottomLeft + 1;

                        // first triangle
                        indices_list[index++] = upperLeft;
                        indices_list[index++] = upperRight;
                        indices_list[index++] = bottomLeft;

                        // second triangle
                        indices_list[index++] = upperRight;
                        indices_list[index++] = bottomRight;
                        indices_list[index++] = bottomLeft;

                    }
                }
            }
            // each group of 3 in the index buffer represents a triangle
            for (int i = 0; i < indices_list.Length; i += 3)
            {
                // take 3 indices
                var i1 = indices_list[i];
                var i2 = indices_list[i + 1];
                var i3 = indices_list[i + 2];

                // look up their corresponding vertices such that you get the points of a triangle
                var p1 = vertex_list[indices_list[i]];
                var p2 = vertex_list[indices_list[i + 1]];
                var p3 = vertex_list[indices_list[i + 2]];

                // the normal vector to a plane is the cross product of two orthonormal vectors on the plane
                var U = p1.Position - p2.Position;
                var V = p1.Position - p3.Position;
                var N = Vector3.Cross(U, V); //Vector3.Cross(side1, side2);

                // accumulate normals for each vertex for later averaging
                vertex_list[i1].Normal += N;
                vertex_list[i2].Normal += N;
                vertex_list[i3].Normal += N;
            }

            // normalise the summed normals to get the average normal
            // this produces a normal that is the average of the normals to the triangle faces surrounding the vertex and is smoother over the terrain
            // not super sure if this needs to be done, but I guess IEEE754 floats are more accurate around 0 after being manipulated by shaders, etc
            for (int i = 0; i < vertex_list.Length; i++)
            {
                vertex_list[i].Normal.Normalize();
            }
        }

        public override void Draw(SharpDX.Toolkit.GameTime gametime)
        {
            if (PhysicsDescription.EnableDebugDraw && PhysicsDescription != null)
            {
                PhysicsDescription.DebugDraw(game.debugDrawer);
            }
            game.GraphicsDevice.SetVertexBuffer((Buffer<VertexPositionNormalColor>)vertices);
            game.GraphicsDevice.SetIndexBuffer(indices, true);
            game.GraphicsDevice.SetVertexInputLayout(inputLayout);

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                game.GraphicsDevice.DrawIndexed(PrimitiveType.TriangleList, indices.ElementCount);

            }
            base.Draw(gametime);
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