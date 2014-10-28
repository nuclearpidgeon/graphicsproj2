using System;
using Jitter.LinearMath;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace Project2.GameObjects.Abstract
{
    public abstract class GameObject : IUpdateable, IDrawable
    {
        protected Project2Game game; 
        
        protected BasicEffect basicEffect;
        protected VertexInputLayout inputLayout;

        private Vector3 position;
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
                positionMatrix = Matrix.Translation(value);
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
            
            // Just in case...?
            WorldMatrix = Matrix.Identity;

            // scaleMatrix = Matrix.Identity;
            this.SetScale(scale);
            // positionMatrix = Matrix.Identity;
            Position = position;
            
            orientationMatrix = Matrix.RotationYawPitchRoll(orientation.X, orientation.Y, orientation.Z);

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

        /// <summary>
        /// Set object's scale matrix from a Vector input
        /// </summary>
        /// <param name="scale">Vector to use for scaling matrix</param>
        public void SetScale(Vector3 scale)
        {
            scaleMatrix = Matrix.Scaling(scale);
        }

        /// <summary>
        /// Set object's orientation matrix from a Vector input
        /// </summary>
        /// <param name="orientation">Vector to use for orientation matrix</param>
        public void SetOrientation(Vector3 orientation)
        { 
            this.orientationMatrix = Matrix.RotationYawPitchRoll(orientation.X, orientation.Y, orientation.Z)
                                        * Matrix.Identity * (float)(2 * Math.PI);
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
    }
}