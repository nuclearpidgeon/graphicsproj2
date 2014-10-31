using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project2.GameSystems;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using Jitter.Dynamics;
using Jitter.Collision.Shapes;

namespace Project2.GameObjects
{
    class Obelisk : Box
    {
        private Effect effect;

        public Obelisk(Project2Game game, Vector3 position, Vector3 size)
            : base(game, game.Content.Load<Model>("Models\\Crate_Barrel"), position, size, Vector3.Zero)
        {
            
            effect = game.Content.Load<Effect>("Shaders\\Cel");
        }

        protected override RigidBody GeneratePhysicsDescription()
        {
            // there is almost certainly a better way to do this
            BoundingSphere boundingSphere = model.CalculateBounds();
            Shape collisionShape = new CylinderShape(Scale.Y*4f, boundingSphere.Radius*4f);
            var rigidBody = new RigidBody(collisionShape)
            {
                Position = PhysicsSystem.toJVector(Position),
                Orientation = PhysicsSystem.toJMatrix(OrientationMatrix),
                IsStatic = true,
                EnableDebugDraw = true,
            };

            return rigidBody;
        }

        public override void Draw(GameTime gametime)
        {

            effect.Parameters["World"].SetValue(this.WorldMatrix);
            effect.Parameters["Projection"].SetValue(game.camera.projection);
            effect.Parameters["View"].SetValue(game.camera.view);
            effect.Parameters["cameraPos"].SetValue(game.camera.position);
            effect.Parameters["worldInvTrp"].SetValue(Matrix.Transpose(Matrix.Invert(this.WorldMatrix)));
            // For Rainbow (required)
            //effect.Parameters["Time"].SetValue((float)gametime.TotalGameTime.TotalSeconds);

            // For Cel (both optional)
            effect.Parameters["lightAmbCol"].SetValue<Color4>(Color.Lerp(Color.Red, Color.Blue, (float)Math.Abs((Math.Cos(2.0 * gametime.TotalGameTime.TotalSeconds)))));
            effect.Parameters["objectCol"].SetValue<Color4>(new Color4(0.5f, 0.5f, 0.5f, 1.0f));
            effect.Parameters["quant"].SetValue<float>(3.0f);

            //this.model.Draw(game.GraphicsDevice, this.worldMatrix, game.camera.view, game.camera.projection, effect);

            foreach (var pass in this.effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                if (this.model != null)
                {
                    this.model.Draw(game.GraphicsDevice, WorldMatrix, game.camera.view, game.camera.projection, effect);
                }
            }
            if (PhysicsDescription.EnableDebugDraw && PersistentStateManager.debugRender && PhysicsDescription != null)
            {
                PhysicsDescription.DebugDraw(game.debugDrawer);
            }
        }


    }
}
