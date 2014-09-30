using System;
using Jitter.LinearMath;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace Project2.GameObjects.Abstract
{
    public abstract class GameObject : IUpdateable, IDrawable
    {
        protected BasicEffect basicEffect;
        protected BoundingSphere boundingSphere;
        protected Project2Game game;
        protected VertexInputLayout inputLayout;

        protected Model model;
        protected Matrix orientationMatrix;
        protected Matrix positionMatrix;
        protected Matrix scaleMatrix;
        protected Matrix worldMatrix;


        protected GameObject(Project2Game game, Vector3 position)
            : this(game, null, position, Vector3.Zero, Vector3.One)
        {
        }

        protected GameObject(Project2Game game, Model model, Vector3 position)
            : this(game, model, position, Vector3.Zero, Vector3.One)
        {
        }

        protected GameObject(Project2Game game, Model model, Vector3 position, Vector3 orientation, Vector3 scale)
        {
            this.game = game;
            scaleMatrix = Matrix.Identity;
            positionMatrix = Matrix.Identity;
            orientationMatrix = Matrix.Identity;
            worldMatrix = Matrix.Identity;

            this.SetScale(scale);
            this.SetPosition(position);
            this.SetOrientation(Matrix.RotationYawPitchRoll(orientation.X, orientation.Y, orientation.Z));

            this.model = model;
            if (model != null)
            {
                boundingSphere = model.CalculateBounds();
            }
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
        

        public Vector3 Position{ get; set; }

        public virtual void LoadContent()
        {
        }

        public virtual void Draw(GameTime gametime)
        {
            basicEffect.CurrentTechnique.Passes[0].Apply();
            basicEffect.World = this.worldMatrix;
            basicEffect.View = game.camera.view;
            basicEffect.Projection = game.camera.projection;

            //this.model.Draw(game.GraphicsDevice, this.worldMatrix, game.camera.view, game.camera.projection, basicEffect);
            
            foreach (var pass in this.basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                if (model != null)
                {
                    model.Draw(game.GraphicsDevice, worldMatrix, game.camera.view, game.camera.projection, basicEffect);
                }
            }
        }


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

        public virtual void Update(GameTime gametime)
        {
            // get matricies from camera
            basicEffect.View = game.camera.view;
            basicEffect.Projection = game.camera.projection;
        }

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

        public virtual void SetScale(Vector3 scale)
        {
            positionMatrix = Matrix.Scaling(scale);
            CalculateWorldMatrix();
        }

        public virtual void SetPosition(Vector3 position)
        {
            positionMatrix = Matrix.Translation(position);
            CalculateWorldMatrix();
        }

        public virtual void SetOrientation(Vector3 orientation)
        {
            orientationMatrix = Matrix.RotationYawPitchRoll(orientation.X, orientation.Y, orientation.Z);
            CalculateWorldMatrix();
        }

        public virtual void SetOrientation(Matrix orientation)
        {
            orientationMatrix = orientation;
            CalculateWorldMatrix();
        }

        protected void CalculateWorldMatrix()
        {
            // multiply in S R T order (Scale, Rotation, Translation)
            worldMatrix = scaleMatrix*orientationMatrix*positionMatrix;
        }
    }
}