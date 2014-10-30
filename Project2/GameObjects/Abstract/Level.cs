using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jitter;
using Project2.GameObjects.Boids;
using Project2.GameObjects.Interface;
using Project2.GameObjects.LevelPieces;
using SharpDX;
using SharpDX.Toolkit;

using Jitter.Collision.Shapes;
using Jitter.Dynamics;

using Project2.GameObjects.Abstract;

namespace Project2.GameObjects
{
    using SharpDX.Toolkit.Graphics;
    abstract public class Level : IUpdateable, IDrawable, INode
    {
        public Project2Game game;
        public ModelPhysicsObject player;
        public ModelPhysicsObject endGoal;
        //private Matrix worldMatrix;

        public Flock flock;
        
        //private BasicEffect basicEffect;

        public const int PreferedTileWidth = 128;
        public const int PreferedTileHeight = 128;

        public Level(Project2Game game)
        {
            this.game = game;
            Children = new List<INode>();

            player = new Monkey(this.game, game.models["bigmonkey"], getStartPosition());
            AddChild(player);

            flock = new Flock(this.game, this);
            AddChild(flock);
        }

        public abstract void BuildLevel();

        public void ResetPlayer()
        {
            player.Destroy();
            player = new Monkey(this.game, game.models["bigmonkey"], getStartPosition());
            AddChild(player);
            game.camera.SetFollowObject(player);
        }

        public void Update(GameTime gameTime)
        {
            foreach (var o in Children.ToList())
            {
                o.Update(gameTime);
            }
        }

        public void Draw(GameTime gameTime)
        {
            foreach (var o in Children.ToList())
            {
                o.Draw(gameTime);
            }
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

        public INode Parent { get; set; }

        public List<INode> Children { get; set; }

        public void AddChild(INode childNode)
        {
            childNode.Parent = this;
            Children.Add(childNode);
        }

        public void RemoveChild(INode childNode)
        {
            Children.Remove(childNode);
        }
    }
}
