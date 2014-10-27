using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Toolkit;

namespace Project2.GameObjects.Boids
{
    public class Flock: IUpdateable, IDrawable
    {

        public enum BoidType
        {
            Friendly,
            Enemy,
            Neutral
        }

        public List<Boid> boidList;
        public Project2Game game;
        public Level level;

        public Flock(Project2Game game, Level level)
        {
            this.game = game;
            this.level = level;
            boidList = new List<Boid>();
        }

        public void AddBoid(BoidType boidType, Vector3 position)
        {
            switch (boidType)
            {
                case BoidType.Friendly:
                    boidList.Add(new FriendlyBoid(game, this, game.models["Sphere"], position));
                    break;

                case BoidType.Enemy:
                    boidList.Add(new EnemyBoid(game, this, game.models["Teapot"], position));
                    break;

                case BoidType.Neutral:
                    boidList.Add(new NeutralBoid(game, this, game.models["Sphere"], position));
                    break;
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (var boid in boidList)
            {
                boid.Update(gameTime);
            }
        }

        public void Draw(GameTime gameTime)
        {
            foreach (var boid in boidList)
            {
                boid.Draw(gameTime);
            }
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
