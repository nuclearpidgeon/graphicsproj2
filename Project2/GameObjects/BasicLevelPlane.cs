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
    class BasicLevelPlane : IUpdateable, IDrawable
    {
        private Project2Game game;
        private Matrix worldMatrix;

        public List<GameObject> gameObjects;
        
        public Terrain floor;
        private List<Box> walls;

        private BasicEffect basicEffect;

        int xSize;
        int ySize;

        public BasicLevelPlane(Project2Game game, Vector3 position, int xSize = 64, int ySize = 64)
        {
            this.game = game;
            this.worldMatrix = Matrix.Identity;
            this.xSize = xSize;
            this.ySize = ySize;
            this.floor = new Terrain(game, position, xSize, ySize);
            this.walls = new List<Box>();
            for (int i = 0; i < (ySize/4); i++)
            {
                var yPos = 2f;
                var zPos = i * 4;
                var size = 4f;
                walls.Add(
                    new Box(game, game.models["box"], position + new Vector3(0f,yPos,zPos), new Vector3(size), true)
                );
                walls.Add(
                    new Box(game, game.models["box"], position + new Vector3(xSize, yPos, zPos), new Vector3(size), true)
                );
            }
            this.gameObjects = new List<GameObject>();
            this.gameObjects.Add(floor);
            this.gameObjects.AddRange(walls);

            //basicEffect = new BasicEffect(game.GraphicsDevice)
            //{
            //    VertexColorEnabled = false,
            //    View = game.camera.view,
            //    Projection = game.camera.projection,
            //    World = this.worldMatrix,
            //    LightingEnabled = true
            //};
            //basicEffect.EnableDefaultLighting();
        }

        public void Update(GameTime gameTime)
        {
            floor.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            floor.Draw(gameTime);

            // saving this for later if we use a single effect for all level things??
            //basicEffect.CurrentTechnique.Passes[0].Apply();
            //basicEffect.World = this.worldMatrix;
            //basicEffect.View = game.camera.view;
            //basicEffect.Projection = game.camera.projection;

            //foreach (var pass in this.basicEffect.CurrentTechnique.Passes)
            //{
            //    pass.Apply();
            //}
        }

        public Vector3 getStartPosition()
        {
            return new Vector3(((float)this.xSize)/2.0f, 10.0f, ((float)this.ySize)/2.0f);
        }

        // stub methods from IDrawable and IUpdateable

        public bool BeginDraw()
        {
            throw new NotImplementedException();
        }        

        public int DrawOrder
        {
            get { throw new NotImplementedException(); }
        }

        public event EventHandler<EventArgs> DrawOrderChanged;

        public void EndDraw()
        {
            throw new NotImplementedException();
        }

        public bool Visible
        {
            get { throw new NotImplementedException(); }
        }

        public event EventHandler<EventArgs> VisibleChanged;

        public bool Enabled
        {
            get { throw new NotImplementedException(); }
        }

        public event EventHandler<EventArgs> EnabledChanged;

        public int UpdateOrder
        {
            get { throw new NotImplementedException(); }
        }

        public event EventHandler<EventArgs> UpdateOrderChanged;
    }
}
