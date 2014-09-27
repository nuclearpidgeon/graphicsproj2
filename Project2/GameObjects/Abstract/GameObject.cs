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

            // Setup rendering effect
            basicEffect = new BasicEffect(game.GraphicsDevice)
            {
                VertexColorEnabled = true,
                View = game.camera.view,
                Projection = game.camera.projection,
                World = Matrix.Identity
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

        protected void CalculateWorldMatrix() {
            this.worldMatrix = this.orientationMatrix * this.scaleMatrix * this.positionMatrix;
        }

        public virtual void Update(GameTime gametime)
        {
            // get matricies from camera
            basicEffect.View = game.camera.view;
            basicEffect.Projection = game.camera.projection;
        }
        public virtual void Draw(GameTime gametime) {
            if (this.model != null) {
                model.Draw(game.GraphicsDevice, this.worldMatrix, game.camera.view, game.camera.projection);
            }
            if (DebugDrawStatus == true && physicsBody != null)
            {
                this.physicsBody.DebugDraw(game.debugDrawer);
            }
        }

        public void DebugDrawEnabled(Boolean t)
        {
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
