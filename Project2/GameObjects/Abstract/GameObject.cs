using System;
using System.Collections.Generic;
using Jitter.LinearMath;
using Project2.GameObjects.Interface;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace Project2.GameObjects.Abstract
{
    public abstract class GameObject : IUpdateable, IDrawable, INode
    {
        protected Project2Game game; 
        
        protected BasicEffect basicEffect;
        protected VertexInputLayout inputLayout;

        private Vector3 position;
        private Vector3 orientation;
        private Vector3 scale;
        private Matrix orientationMatrix;
        private Matrix positionMatrix;
        private Matrix scaleMatrix;
        private Matrix worldMatrix;

        protected Matrix WorldMatrix
        {
            get { return worldMatrix; }
            private set { worldMatrix = value; }
        }

        public Vector3 Position 
        {
            get { return position; }
            set
            {
                position = value;
                positionMatrix = Matrix.Translation(position);
            } 
        }

        public Vector3 Scale
        {
            get { return scale; }
            set
            {
                scale = value;
                scaleMatrix = Matrix.Scaling(scale);
            }
        }

        public Vector3 Orientation
        {
            get { return orientation; }
            set
            {
                orientation = value;
                this.orientationMatrix = Matrix.RotationYawPitchRoll(orientation.X, orientation.Y, orientation.Z);
            }
        }

        protected Matrix OrientationMatrix
        {
            get { return orientationMatrix; }
            set
            {
                orientationMatrix = value;
                UpdateWorldMatrix();
            }
        }
        protected Matrix PositionMatrix
        {
            get { return positionMatrix; }
            private set
            {
                positionMatrix = value;
                UpdateWorldMatrix();
            }
        }
        protected Matrix ScaleMatrix
        {
            get { return scaleMatrix; }
            private set
            {
                scaleMatrix = value;
                UpdateWorldMatrix();
            }
        }
        
        #region extra constructors
        /// <summary>
        /// Create new GameObject with default orientation and size.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="position"></param>
        protected GameObject(Project2Game game, Vector3 position)
            : this(game, position, Vector3.Zero, Vector3.One)
        {
        }

        /// <summary>
        /// Create a new GameObject with default orientation.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="position"></param>
        /// <param name="scale"></param>
        protected GameObject(Project2Game game, Vector3 position, Vector3 scale)
            : this(game, position, Vector3.Zero, scale)
        {
        }
        #endregion

        protected GameObject(Project2Game game, Vector3 position, Vector3 orientation, Vector3 scale)
        {
            this.game = game;
            Children = new List<INode>();
            // Just in case...?
            WorldMatrix = Matrix.Identity;

            Scale = scale;
            Orientation = orientation;
            Position = position;

            // Setup rendering effect
            basicEffect = new BasicEffect(game.GraphicsDevice)
            {
                VertexColorEnabled = false,
                View = game.camera.view,
                Projection = game.camera.projection,
                World = Matrix.Identity,
                LightingEnabled = true
            };
            basicEffect.EnableDefaultLighting();
        }

        public virtual void LoadContent()
        {
        }

        /// <summary>
        /// Sets up basicEffect parameters for drawing
        /// </summary>
        /// <param name="gametime"></param>
        public virtual void Draw(GameTime gametime)
        {
            basicEffect.CurrentTechnique.Passes[0].Apply();
            basicEffect.World = this.WorldMatrix;
            basicEffect.View = game.camera.view;
            basicEffect.Projection = game.camera.projection;

            // Actual drawing logic is implemented in subclasses.
        }

        public virtual void Update(GameTime gametime)
        {
            // get matricies from camera
            basicEffect.View = game.camera.view;
            basicEffect.Projection = game.camera.projection;
        }

        protected void UpdateWorldMatrix()
        {
            // multiply in S R T order (Scale, Rotation, Translation)
            WorldMatrix = scaleMatrix*orientationMatrix*positionMatrix;
        }

        #region unimplemented-inteface stuff
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

        public INode Parent
        {
            get;
            set;
        }

        public List<INode> Children { get; set; }


        public void AddChild(INode childNode)
        {
            childNode.Parent = this;
            Children.Add(childNode);
        }

        public void RemoveChild(INode childNode)
        {
            childNode.RemoveChild(childNode);
        }
    }
}