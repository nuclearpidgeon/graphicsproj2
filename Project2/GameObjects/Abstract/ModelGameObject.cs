using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project2.GameObjects.Abstract
{
    public class ModelGameObject : GameObject
    {
        protected BoundingSphere boundingSphere;
        protected Model model;

        public ModelGameObject(Project2Game game, Model model, Vector3 position, Vector3 orientation, Vector3 scale)
            : base(game, position, orientation, scale)
        {
            this.model = model;
            if (model != null)
            {
                boundingSphere = model.CalculateBounds(WorldMatrix);
            }
        }

        public override void Draw(GameTime gametime)
        {
            base.Draw(gametime);
            //this.model.Draw(game.GraphicsDevice, this.worldMatrix, game.camera.view, game.camera.projection, basicEffect);

            foreach (var pass in this.basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                if (model != null)
                {
                    model.Draw(game.GraphicsDevice, WorldMatrix, game.camera.view, game.camera.projection, basicEffect);
                }
            }
        }
    }
}
