using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Project2.GameObjects.Abstract;

using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

using Jitter;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;

namespace Project2.GameObjects
{
    public class Monkey : ModelPhysicsObject
    {
        private Effect effect;
        public Monkey(Project2Game game, Model model, Vector3 position)
            : base(game, model, position)
        {
            // Load custom rainbox monkey effect
            effect = game.Content.Load<Effect>("Shaders\\Rainbow");
            PhysicsDescription.Mass = 20f;
            //PhysicsDescription.Tag = "player";
        }

        public override void Update(GameTime gametime)
        {
            this.PhysicsDescription.ApplyImpulse(PhysicsSystem.toJVector(game.inputManager.SecondaryDirection() * 10f), PhysicsSystem.toJVector(Vector3.Zero));
            this.PhysicsDescription.ApplyImpulse(PhysicsSystem.toJVector(game.inputManager.Acceleration() * 10f), PhysicsSystem.toJVector(Vector3.Zero));

            base.Update(gametime);
        }

        public override void Draw(GameTime gametime)
        {
            //basicEffect.CurrentTechnique.Passes[0].Apply();
            //basicEffect.World = this.worldMatrix;
            //basicEffect.View = game.camera.view;
            //basicEffect.Projection = game.camera.projection;
            //
            //this.model.Draw(game.GraphicsDevice, this.worldMatrix, game.camera.view, game.camera.projection, basicEffect);

            effect.Parameters["World"].SetValue(this.WorldMatrix);
            effect.Parameters["Projection"].SetValue(game.camera.projection);
            effect.Parameters["View"].SetValue(game.camera.view);
            effect.Parameters["cameraPos"].SetValue(game.camera.position);
            effect.Parameters["worldInvTrp"].SetValue(Matrix.Transpose(Matrix.Invert(this.WorldMatrix)));
            // For Rainbow (required)
            effect.Parameters["Time"].SetValue((float)gametime.TotalGameTime.TotalSeconds);

            // For Cel (both optional)
            //effect.Parameters["objectCol"].SetValue<Color4>(new Color4(0.5f, 0.5f, 0.5f, 1.0f));
            //effect.Parameters["quant"].SetValue<float>(3.0f);

            //this.model.Draw(game.GraphicsDevice, this.worldMatrix, game.camera.view, game.camera.projection, effect);

            foreach (var pass in this.effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                if (this.model != null)
                {
                    this.model.Draw(game.GraphicsDevice, WorldMatrix, game.camera.view, game.camera.projection, effect);
                }
            }
            if (PhysicsDescription.EnableDebugDraw && PhysicsDescription != null)
            {
                PhysicsDescription.DebugDraw(game.debugDrawer);
            }
        }
    }
}
