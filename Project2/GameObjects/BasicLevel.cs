﻿using System;
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
    class BasicLevel : IUpdateable, IDrawable
    {
        private Project2Game game;
        private Matrix worldMatrix;

        public List<BasicLevelPlane> levelPieces;
        
        private BasicEffect basicEffect;

        int length;
        int xSize;
        int ySize;

        public BasicLevel(Project2Game game, int length = 8)
        {
            this.game = game;
            this.xSize = 64;
            this.ySize = 64;
            levelPieces = new List<BasicLevelPlane>();
            for (int i = 0; i < length; i++)
            {
                levelPieces.Add(new BasicLevelPlane(game, new Vector3(0f,0f,(float)ySize*i), xSize, ySize));
            }

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
            
        }

        public void Draw(GameTime gameTime)
        {
            

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
