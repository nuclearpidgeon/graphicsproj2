using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project2.GameObjects.Interface;
using SharpDX;
using SharpDX.Toolkit;

namespace Project2.GameObjects.Boids
{
    public class Flock: IUpdateable, IDrawable, INode
    {

        public enum BoidType
        {
            Friendly,
            Enemy,
            Neutral
        }

        public Project2Game game;
        public Level level;

        public Flock(Project2Game game, Level level)
        {
            this.game = game;
            this.level = level;
        }

        public void AddBoid(BoidType boidType, Vector3 position)
        {
            Boid newBoid;
            switch (boidType)
            {
                case BoidType.Friendly:
                    newBoid = new FriendlyBoid(game, this, game.models["Sphere"], position);
                    break;

                case BoidType.Enemy:
                    newBoid = new EnemyBoid(game, this, game.models["Teapot"], position);
                    break;

                case BoidType.Neutral:
                    newBoid = new NeutralBoid(game, this, game.models["Sphere"], position);
                    break;

                default:
                    newBoid = new NeutralBoid(game, this, game.models["Sphere"], position);
                    break;

            }
            AddChild(newBoid);
        }

        public void Update(GameTime gameTime)
        {
            foreach (var o in Children)
            {
                o.Update(gameTime);
            }
        }

        public void Draw(GameTime gameTime)
        {
            foreach (var o in Children)
            {
                o.Draw(gameTime);
            }
        }

        public INode Parent
        {
            get;
            set;
        }

        public List<INode> Children
        {
            get;
            set;
        }


        public void AddChild(INode childNode)
        {
            childNode.Parent = this;
            Children.Add(childNode);
        }

        public void RemoveChild(INode childNode)
        {
            childNode.RemoveChild(childNode);
        }

        #region interface crap
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
        #endregion
    }
}
