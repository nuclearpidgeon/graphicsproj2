using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Project2.GameObjects;
using Project2.GameObjects.Abstract;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;

namespace Project2
{
    public class ThirdPersonCamera
    {
        public Project2Game game;
        public Matrix view { get; set; }
        public Matrix projection;

        public Vector3 position { get; private set; }
        public Vector3 direction { get; private set; }
        public Vector3 offset { get; private set; }

        Vector2 lastMousePos;

        private PhysicsObject followObject;

        public ThirdPersonCamera(Project2Game game, Vector3 position, Vector3 offset)
        {
           

            this.game = game;
            this.position = position;
            this.offset = offset;

            this.view = Matrix.LookAtLH(position, Vector3.Zero, Vector3.Up);
            this.projection = Matrix.PerspectiveFovLH(
                (float)Math.PI / 4.0f,
                (float)game.GraphicsDevice.BackBuffer.Width / game.GraphicsDevice.BackBuffer.Height,
                0.1f,
                100.0f
            );
        }

        public void SetFollowObject(PhysicsObject toFollow)
        {
            this.followObject = toFollow;
        }


        /// <summary>
        /// Updates the Camera based on keyboard and mouse input
        /// </summary>
        /// <param name="gameTime"></param>

        public void Update(GameTime gameTime)
        {
            if (followObject == null) return;

            this.position = Vector3.Transform(offset, (Matrix3x3)Matrix.RotationAxis(Vector3.Up, MathUtil.Pi));
            this.position += followObject.Position;

            //Vector3 camup = Vector3.Up;
            //camup = Vector3.Transform(camup, (Matrix3x3)followObject.Orientation.Transpose());
            
            view = Matrix.LookAtLH(this.position, followObject.Position, Vector3.Up);
            projection = Matrix.PerspectiveFovLH(MathUtil.PiOverFour, (float)game.GraphicsDevice.BackBuffer.Width / game.GraphicsDevice.BackBuffer.Height, 0.2f, 500.0f);
        }

        public void SetEffects(BasicEffect basicEffect)
        {

        }
    }
}
