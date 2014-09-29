using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SharpDX;
using SharpDX.Toolkit;

using Jitter;
using Jitter.Dynamics;
using Jitter.Collision;
using Jitter.Collision.Shapes;


namespace Project2
{
    using SharpDX.Toolkit.Graphics;
    abstract public class GameObject : IUpdateable, IDrawable
    {
        protected BasicEffect basicEffect;
        protected VertexInputLayout inputLayout;

        protected Model model;
        protected BoundingSphere boundingSphere;

        protected Matrix scaleMatrix;
        protected Matrix positionMatrix;
        protected Matrix orientationMatrix;
        protected Matrix worldMatrix;

        protected Boolean DebugDrawStatus;

        protected RigidBody physicsBody;
        protected Shape physicsShape;
        protected Project2Game game;

        public GameObject(Project2Game game)
        {
            this.game = game;
            this.scaleMatrix = Matrix.Identity;
            this.positionMatrix = Matrix.Identity;
            this.orientationMatrix = Matrix.Identity;
            this.worldMatrix = Matrix.Identity;
            // Setup rendering effect
            basicEffect = new BasicEffect(game.GraphicsDevice)
            {
                VertexColorEnabled = false,
                View = game.camera.view,
                Projection = game.camera.projection,
                World = Matrix.Identity,
            };

            this.DebugDrawStatus = false;
        }

        public virtual void SetScale(Vector3 scale)
        {
            this.positionMatrix = Matrix.Scaling(scale);
            CalculateWorldMatrix();
        }

        public virtual void SetPosition(Vector3 position) {
            this.positionMatrix = Matrix.Translation(position);
            CalculateWorldMatrix();
        }

        public virtual void SetOrientation(Vector3 orientation) {
            this.orientationMatrix = Matrix.RotationYawPitchRoll(orientation.X, orientation.Y, orientation.Z);
            CalculateWorldMatrix();
        }

        public virtual void SetOrientation(Matrix orientation)
        {
            
            this.orientationMatrix = orientation;
            CalculateWorldMatrix();
        }

        protected void CalculateWorldMatrix() {
            // multiply in S R T order (Scale, Rotation, Translation)
            this.worldMatrix = this.scaleMatrix * this.orientationMatrix * this.positionMatrix;
        }

        public virtual void Update(GameTime gametime)
        {
            // get matricies from camera
            basicEffect.View = game.camera.view;
            basicEffect.Projection = game.camera.projection;
        }
        public virtual void Draw(GameTime gametime) {

            if (this.physicsBody.EnableDebugDraw && physicsBody != null)
            {
                this.physicsBody.DebugDraw(game.debugDrawer);
            }
        }

        public void DebugDrawEnabled(Boolean t)
        {
            this.physicsBody.EnableDebugDraw = t;
            this.DebugDrawStatus = t;
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
    }
}
