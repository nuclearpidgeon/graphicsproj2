using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;

namespace Project2
{
    public class Camera
    {
        public Project2Game game;
        public Matrix view { get; set; }
        public Matrix projection;

        public Vector3 position { get; private set; }
        public Vector3 direction { get; private set; }
        public Vector3 movement { get; private set; }

        Vector2 lastMousePos;

        public Camera(Project2Game game, Vector3 position, Vector3 target)
        {
            this.game = game;
            this.position = position;
            this.direction = target - position;
            this.movement = new Vector3(0.0f, 0.0f, 0.0f);

            this.view = Matrix.LookAtLH(position, target, Vector3.Up);
            this.projection = Matrix.PerspectiveFovLH(
                (float)Math.PI / 4.0f,
                (float)game.GraphicsDevice.BackBuffer.Width / game.GraphicsDevice.BackBuffer.Height,
                0.1f,
                100.0f
            );
        }

        /// <summary>
        /// Updates the Camera based on keyboard and mouse input
        /// </summary>
        /// <param name="gameTime"></param>

        public void Update(GameTime gameTime)
        {
            // convert time to more useful format
            float time = gameTime.ElapsedGameTime.Milliseconds;

            // setup vector for new position for collision detection
            Vector3 newPosition = this.position;

            ////////////////////
            // KEYBOARD LOGIC //
            ////////////////////

            newPosition += game._inputManager.PrimaryDirection() * new Vector3(0.1f, 0.025f, 0.025f) * time;
            newPosition += time * 0.001f * direction * game._inputManager.PrimaryDirection();


            /////////////////
            // MOUSE LOGIC //
            /////////////////

            if (game.mouseState.LeftButton.Pressed)
            {
                this.lastMousePos = new Vector2(game.mouseState.X, game.mouseState.Y);
            }
            if (game.mouseState.LeftButton.Down)
            {
                Vector2 change = new Vector2(game.mouseState.X, game.mouseState.Y) - this.lastMousePos;
                this.direction = (Vector3)Vector3.Transform(
                    this.direction,
                    Matrix.RotationY(change.X) * Matrix.RotationAxis(
                        Vector3.Normalize(Vector3.Cross(Vector3.Up, this.direction)),
                        change.Y
                    )
                );
                this.lastMousePos += change;
            }

            
            this.position = newPosition;

            // update view with new position
            this.view = Matrix.LookAtLH(this.position, this.direction + this.position, Vector3.Up);

        }

        public void SetEffects(BasicEffect basicEffect)
        {

        }
    }
}
