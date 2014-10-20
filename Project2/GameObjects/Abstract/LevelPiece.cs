using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Toolkit;
using Project2.GameObjects.LevelPieces;


namespace Project2.GameObjects.Abstract
{
    public abstract class LevelPiece: IUpdateable, IDrawable
    {
        public Game game; // parent game
        public Level level; // parent level
        public Vector3 OriginPosition;
        public List<PhysicsPuzzle> physicsPuzzles = new List<PhysicsPuzzle>();
        private List<GameObject> childObjects = new List<GameObject>();

        public LevelPiece(Game game, Level level, Vector3 originPosition)
        {
            this.game = game;
            this.level = level;
            this.OriginPosition = originPosition;
        }

        public void AddChild(GameObject o)
        {
            this.childObjects.Add(o);
        }

        public void RemoveChild(GameObject o)
        {
            this.childObjects.Remove(o);
        }

        public void Update(GameTime gameTime)
        {
            foreach (var o in childObjects)
            {
                o.Update(gameTime);
            }
            foreach (var o in physicsPuzzles)
            {
                o.Update(gameTime);
            }
        }

        public void Draw(GameTime gameTime)
        {
            foreach (var o in childObjects)
            {
                o.Draw(gameTime);
            }
            foreach (var o in physicsPuzzles)
            {
                o.Draw(gameTime);
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
