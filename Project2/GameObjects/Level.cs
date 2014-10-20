﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project2.GameObjects.Boids;
using Project2.GameObjects.LevelPieces;
using SharpDX;
using SharpDX.Toolkit;

using Jitter.Collision.Shapes;
using Jitter.Dynamics;

using Project2.GameObjects.Abstract;

namespace Project2.GameObjects
{
    using SharpDX.Toolkit.Graphics;
    abstract public class Level : IUpdateable, IDrawable
    {
        public Project2Game game;
        public PhysicsObject player;
        //private Matrix worldMatrix;

        public List<LevelPiece> LevelPieces;
        public List<GameObject> ChildObjects; 

        public Flock flock;
        
        //private BasicEffect basicEffect;

        public const int PreferedTileWidth = 72;
        public const int PreferedTileHeight = 72;

        public Level(Project2Game game)
        {
            this.game = game;
            LevelPieces = new List<LevelPiece>();
            ChildObjects = new List<GameObject>();
            flock = new Flock(this.game);
            player = new Monkey(this.game, game.models["bigmonkey"], getStartPosition(), false);
            ChildObjects.Add(player);

        }

        public abstract void BuildLevel();

        public void ResetPlayer()
        {
            ChildObjects.Remove(player);
            player.Destroy();
            player = new Monkey(this.game, game.models["bigmonkey"], getStartPosition(), false);
            ChildObjects.Add(player);
            game.camera.SetFollowObject(player);
        }

        public void Update(GameTime gameTime)
        {
            flock.Update(gameTime);
            foreach (var childObject in ChildObjects)
            {
                childObject.Update(gameTime);
            }
            foreach (var lp in LevelPieces)
            {
                lp.Update(gameTime);
            }

            
        }

        public void Draw(GameTime gameTime)
        {
            flock.Draw(gameTime);
            foreach (var childObject in ChildObjects)
            {
                childObject.Draw(gameTime);
            }
            foreach (var lp in LevelPieces)
            {
                lp.Draw(gameTime);
            }

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
            return new Vector3(((float)PreferedTileWidth)/2.0f, 10.0f, ((float)PreferedTileHeight)/2.0f);
        }

        // stub methods from IDrawable and IUpdateable

        #region interface crap
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
        #endregion
    }
}
