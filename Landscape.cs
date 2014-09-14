using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace Project2
{
    using SharpDX.Toolkit.Graphics;
    class Landscape : ColoredGameObject
    {  
        public Landscape(Game game)
        {
            Vector3 frontBottomLeft = new Vector3(-1.0f, -1.0f, -1.0f);
            Vector3 frontTopLeft = new Vector3(-1.0f, 1.0f, -1.0f);
            Vector3 frontTopRight = new Vector3(1.0f, 1.0f, -1.0f);
            Vector3 frontBottomRight = new Vector3(1.0f, -1.0f, -1.0f);
            Vector3 backBottomLeft = new Vector3(-1.0f, -1.0f, 1.0f);
            Vector3 backBottomRight = new Vector3(1.0f, -1.0f, 1.0f);
            Vector3 backTopLeft = new Vector3(-1.0f, 1.0f, 1.0f);
            Vector3 backTopRight = new Vector3(1.0f, 1.0f, 1.0f);

            Vector3 frontBottomLeftNormal = new Vector3(-0.333f, -0.333f, -0.333f);
            Vector3 frontTopLeftNormal = new Vector3(-0.333f, 0.333f, -0.333f);
            Vector3 frontTopRightNormal = new Vector3(0.333f, 0.333f, -0.333f);
            Vector3 frontBottomRightNormal = new Vector3(0.333f, -0.333f, -0.333f);
            Vector3 backBottomLeftNormal = new Vector3(-0.333f, -0.333f, 0.333f);
            Vector3 backBottomRightNormal = new Vector3(0.333f, -0.333f, 0.333f);
            Vector3 backTopLeftNormal = new Vector3(-0.333f, 0.333f, 0.333f);
            Vector3 backTopRightNormal = new Vector3(0.333f, 0.333f, 0.333f);

            vertices = Buffer.Vertex.New(
                game.GraphicsDevice,
                new[]
                    {
                    new VertexPositionNormalColor(frontBottomLeft, frontBottomLeftNormal, Color.Orange), // Front
                    new VertexPositionNormalColor(frontTopLeft, frontTopLeftNormal, Color.Orange),
                    new VertexPositionNormalColor(frontTopRight, frontTopRightNormal, Color.Orange),
                    new VertexPositionNormalColor(frontBottomLeft, frontBottomLeftNormal, Color.Orange),
                    new VertexPositionNormalColor(frontTopRight, frontTopRightNormal, Color.Orange),
                    new VertexPositionNormalColor(frontBottomRight, frontBottomRightNormal, Color.Orange),
                    new VertexPositionNormalColor(backBottomLeft, backBottomLeftNormal, Color.Orange), // BACK
                    new VertexPositionNormalColor(backTopRight, backTopRightNormal, Color.Orange),
                    new VertexPositionNormalColor(backTopLeft, backTopLeftNormal, Color.Orange),
                    new VertexPositionNormalColor(backBottomLeft, backBottomLeftNormal, Color.Orange),
                    new VertexPositionNormalColor(backBottomRight, backBottomRightNormal, Color.Orange),
                    new VertexPositionNormalColor(backTopRight, backTopRightNormal, Color.Orange),
                    new VertexPositionNormalColor(frontTopLeft, frontTopLeftNormal, Color.OrangeRed), // Top
                    new VertexPositionNormalColor(backTopLeft, backTopLeftNormal, Color.OrangeRed),
                    new VertexPositionNormalColor(backTopRight, backTopRightNormal, Color.OrangeRed),
                    new VertexPositionNormalColor(frontTopLeft, frontTopLeftNormal, Color.OrangeRed),
                    new VertexPositionNormalColor(backTopRight, backTopRightNormal, Color.OrangeRed),
                    new VertexPositionNormalColor(frontTopRight, frontTopRightNormal, Color.OrangeRed),
                    new VertexPositionNormalColor(frontBottomLeft, frontBottomLeftNormal, Color.OrangeRed), // Bottom
                    new VertexPositionNormalColor(backBottomRight, backBottomRightNormal, Color.OrangeRed),
                    new VertexPositionNormalColor(backBottomLeft, backBottomLeftNormal, Color.OrangeRed),
                    new VertexPositionNormalColor(frontBottomLeft, frontBottomLeftNormal, Color.OrangeRed),
                    new VertexPositionNormalColor(frontBottomRight, frontBottomRightNormal, Color.OrangeRed),
                    new VertexPositionNormalColor(backBottomRight, backBottomRightNormal, Color.OrangeRed),
                    new VertexPositionNormalColor(frontBottomLeft, frontBottomLeftNormal, Color.DarkOrange), // Left
                    new VertexPositionNormalColor(backBottomLeft, backBottomLeftNormal, Color.DarkOrange),
                    new VertexPositionNormalColor(backTopLeft, backTopLeftNormal, Color.DarkOrange),
                    new VertexPositionNormalColor(frontBottomLeft, frontBottomLeftNormal, Color.DarkOrange),
                    new VertexPositionNormalColor(backTopLeft, backTopLeftNormal, Color.DarkOrange),
                    new VertexPositionNormalColor(frontTopLeft, frontTopLeftNormal, Color.DarkOrange),
                    new VertexPositionNormalColor(frontBottomRight, frontBottomRightNormal, Color.DarkOrange), // Right
                    new VertexPositionNormalColor(backTopRight, backTopRightNormal, Color.DarkOrange),
                    new VertexPositionNormalColor(backBottomRight, backBottomRightNormal, Color.DarkOrange),
                    new VertexPositionNormalColor(frontBottomRight, frontBottomRightNormal, Color.DarkOrange),
                    new VertexPositionNormalColor(frontTopRight, frontTopRightNormal, Color.DarkOrange),
                    new VertexPositionNormalColor(backTopRight, backTopRightNormal, Color.DarkOrange),
                });

            basicEffect = new BasicEffect(game.GraphicsDevice)
            {
                VertexColorEnabled = true,
                View = Matrix.LookAtLH(new Vector3(0, 0, -5), new Vector3(0, 0, 0), Vector3.UnitY),
                Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, (float)game.GraphicsDevice.BackBuffer.Width / game.GraphicsDevice.BackBuffer.Height, 0.1f, 100.0f),
                World = Matrix.Identity
            };

            inputLayout = VertexInputLayout.FromBuffer(0, vertices);
            this.game = game;
        }

        public override void Update(GameTime gameTime)
        {
            // Rotate the cube.
            var time = (float)gameTime.TotalGameTime.TotalSeconds;
            basicEffect.World = Matrix.RotationX(time) * Matrix.RotationY(time * 2.0f) * Matrix.RotationZ(time * .7f);
            basicEffect.Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, (float)game.GraphicsDevice.BackBuffer.Width / game.GraphicsDevice.BackBuffer.Height, 0.1f, 100.0f);
        }

        public override void Draw(GameTime gameTime)
        {
            // Setup the vertices
            game.GraphicsDevice.SetVertexBuffer(vertices);
            game.GraphicsDevice.SetVertexInputLayout(inputLayout);

            // Apply the basic effect technique and draw the rotating cube
            basicEffect.CurrentTechnique.Passes[0].Apply();
            game.GraphicsDevice.Draw(PrimitiveType.TriangleList, vertices.ElementCount);
        }
    }
}
