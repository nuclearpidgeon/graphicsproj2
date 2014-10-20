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
    public abstract class PhysicsPuzzle : IUpdateable, IDrawable
    {
        public Game game; // parent game
        public LevelPiece levelPiece; // parent level piece
        public Vector3 originPosition;

        private List<GameObject> childObjects = new List<GameObject>();

        public PhysicsPuzzle(Game game, LevelPiece levelPiece, Vector3 offset ) {
            this.game = game;
            this.levelPiece = levelPiece;
            this.originPosition = levelPiece.OriginPosition += offset;
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
        }

        public void Draw(GameTime gameTime)
        {
            foreach (var o in childObjects)
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
