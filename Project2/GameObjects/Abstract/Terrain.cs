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
    using SharpDX.Toolkit.Graphics;

    abstract class Terrain : PhysicsObject
    {
        public float[,] TerrainData;
        protected int terrainWidth;
        protected int terrainHeight;
        protected float maxHeight = 0;
        protected float minHeight = 0;
        //private Texture2D heightMap;

        public VertexInputLayout inputLayout;
        public Buffer vertices;
        public Buffer<int> indices;

        private int[] indices_list;
        private VertexPositionNormalColor[] vertex_list;

        protected Terrain(Project2Game game, PhysicsDescription physicsDescription)
            : base(game,physicsDescription)
        {
            this.TerrainData = GenerateTerrain();
            vertices = Buffer.Vertex.New(
                game.GraphicsDevice,
                vertex_list
            );
            
            indices = Buffer.Index.New(
                game.GraphicsDevice,
                indices_list
            );
        }

        protected abstract float[,] GenerateTerrain();

        public override void Draw(GameTime gameTime)
        {
            // setup matricies first
            base.Draw(gameTime);

            // bind buffers
            game.GraphicsDevice.SetVertexBuffer((Buffer<VertexPositionNormalColor>)vertices);
            game.GraphicsDevice.SetIndexBuffer(indices, true);
            game.GraphicsDevice.SetVertexInputLayout(inputLayout);

            // enable wireframe mode here if you want
            RasterizerState rs = SharpDX.Toolkit.Graphics.RasterizerState.New(game.GraphicsDevice,
                new SharpDX.Direct3D11.RasterizerStateDescription()
                {
                    CullMode = SharpDX.Direct3D11.CullMode.Back,
                    FillMode = SharpDX.Direct3D11.FillMode.Solid,
                });
            game.GraphicsDevice.SetRasterizerState(rs);

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                game.GraphicsDevice.DrawIndexed(PrimitiveType.TriangleList, indices.ElementCount);

            }

        }

    }
}
