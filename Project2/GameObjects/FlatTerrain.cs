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
    using SharpDX.Toolkit.Graphics;
    class FlatTerrain : Terrain
    {
        public override float[,] TerrainData { get; set; }
        public override RigidBody PhysicsDescription { get; set; }
        private float frontHeight, backHeight;
        /// <summary>
        /// Constructs a flat terrain
        /// </summary>
        /// <param name="game"></param>
        /// <param name="position"></param>
        /// <param name="xScale"></param>
        /// <param name="yScale"></param>
        public FlatTerrain(Project2Game game, Vector3 position, float xScale, float zScale, float frontHeight = 0.0f, float backHeight = 0.0f)
            : base(game, position, xScale, zScale)
        {
            this.frontHeight = frontHeight;
            this.backHeight = backHeight;

            this.TerrainData = GenerateTerrainData();
            GenerateGeometry();
            SetColour();
            CreateBuffers();
            this.PhysicsDescription = GeneratePhysicsDescription();
            this.Position = PhysicsSystem.toVector3(PhysicsDescription.Position);
            game.physics.AddBody(PhysicsDescription);

            //this.basicEffect.DirectionalLight0.Direction = new Vector3(0f, -2f, -1f);
            this.basicEffect.SpecularPower = 5000f;
        }
        protected override RigidBody GeneratePhysicsDescription()
        {
            var collisionShape = new TerrainShape(TerrainData, (float)xScale, (float)zScale);
            var rigidBody = new RigidBody(collisionShape)
            {
                Position = PhysicsSystem.toJVector(Position),
                IsStatic = true,
                EnableDebugDraw = true,
            };
            return rigidBody;
        }

        protected override float[,] GenerateTerrainData()
        {
            this.terrainWidth = 2;
            this.terrainHeight = 2;
            return new float[,] { 
                {frontHeight, backHeight} ,
                {frontHeight, backHeight}
            };
        }

        private void SetColour()
        {
            for (int i = 0; i < this.Vertices.Length;i++ )
            {
                this.Vertices[i].Color = Color.DarkOliveGreen;
            }
        }
    }
}