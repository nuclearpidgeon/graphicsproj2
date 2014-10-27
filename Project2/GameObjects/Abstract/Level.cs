using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jitter;
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
        public PhysicsObject endGoal;
        //private Matrix worldMatrix;

        public List<LevelPiece> LevelPieces;
        public List<GameObject> ChildObjects; 

        public Flock flock;
        
        //private BasicEffect basicEffect;

        public const int PreferedTileWidth = 128;
        public const int PreferedTileHeight = 128;

        public Level(Project2Game game)
        {
            this.game = game;
            LevelPieces = new List<LevelPiece>();
            ChildObjects = new List<GameObject>();
            flock = new Flock(this.game, this);
            
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
            // This feels janky, maybe it should be a class attribute? Will get updated a lot though?
            List<GameObject> garbageList = new List<GameObject>();

            flock.Update(gameTime);
            
            foreach (var childObject in ChildObjects)
            {
                childObject.Update(gameTime);
                if (childObject.Position.Y < -75)
                {
                    // Kill Kill Kill
                    if (childObject is PhysicsObject)
                    {
                        PhysicsObject physicsObject = (PhysicsObject)childObject;
                        physicsObject.Destroy();
                    }
                    // Can't remove object inside foreach. Need to delete outside the loop
                    garbageList.Add(childObject);
                }
            }
            // Cleanup
            foreach (GameObject obj in garbageList)
            {
                ChildObjects.Remove(obj);
                if (player == obj)
                {
                    ResetPlayer();
                }
            }
            foreach (var lp in LevelPieces)
            {
                lp.Update(gameTime);
            }
            // Check for player death
            if (player.Position.Y < -75)
            {
                ResetPlayer();
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

        public virtual Vector3 getStartPosition()
        {
            return new Vector3(((float)PreferedTileWidth)/2.0f, 10.0f, ((float)PreferedTileHeight)/2.0f);
        }

        public virtual Vector3 getCameraStartPosition()
        {
            return new Vector3(0f, 1f, 2f) * 25;
        }

        public virtual Vector3 getCameraOffset()
        {
            return new Vector3(0f, 1f, 2f) * 25;
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
