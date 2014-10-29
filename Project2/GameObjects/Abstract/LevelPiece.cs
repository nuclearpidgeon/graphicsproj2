﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project2.GameObjects.Interface;
using SharpDX;
using SharpDX.Toolkit;
using Project2.GameObjects.LevelPieces;


namespace Project2.GameObjects.Abstract
{
    public abstract class LevelPiece: IUpdateable, IDrawable, INode
    {
        public Game game; // parent game
        public Level level; // parent level
        public Vector3 OriginPosition;

        public LevelPiece(Game game, Level level, Vector3 originPosition)
        {
            this.game = game;
            this.level = level;
            this.OriginPosition = originPosition;
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
            get { return Parent; }
            set { Parent = value; }
        }

        public List<INode> Children
        {
            get { return Children; }
            set { Children = value; }
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
