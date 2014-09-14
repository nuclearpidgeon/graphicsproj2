using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace Project2
{
    using SharpDX.Toolkit.Graphics;
    abstract public class GameObject
    {
        public BasicEffect basicEffect;
        public VertexInputLayout inputLayout;
        public Project2Game game;

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
        }

        public virtual void Update(GameTime gametime)
        {
            // get matricies from camera
            basicEffect.View = game.camera.view;
            basicEffect.Projection = game.camera.projection;
        }
        public abstract void Draw(GameTime gametime);
    }
}
