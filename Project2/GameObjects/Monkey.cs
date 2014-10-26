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
    public class Monkey : PhysicsObject
    {
        private Effect effect;
        public Monkey(Project2Game game, Model model, Vector3 position, Boolean isStatic)
            : base(game, model, position, GeneratePhysicsDescription(position, model, isStatic))
        {
            effect = game.Content.Load<Effect>("Shaders\\Cel");
        }

        private static PhysicsDescription GeneratePhysicsDescription(Vector3 position, Model model, Boolean isStatic)
        {
            var bounds = model.CalculateBounds();
            var collisionShape = new SphereShape(bounds.Radius);
            var rigidBody = new RigidBody(collisionShape)
            {
                Position = PhysicsSystem.toJVector(position),
                IsStatic = isStatic,
                EnableDebugDraw = true,
                Mass = 20f,
            };

            var description = new PhysicsDescription()
            {
                IsStatic = isStatic,
                CollisionShape = collisionShape,
                Debug = true,
                RigidBody = rigidBody,
                Position = position
            };

            return description;
        }



        public override void Update(GameTime gametime)
        {
            //var pos = PhysicsSystem.toVector3(this.physicsBody.Position);
            //var orientation = PhysicsSystem.toMatrix(this.physicsBody.Orientation);
            ////System.Diagnostics.Debug.WriteLine(pos);
            ////System.Diagnostics.Debug.WriteLine(orientation);

            //// each call to SetX recalculates the world matrix. This is inefficient and should be fixed.
            //this.SetPosition(pos);
            //this.SetOrientation(orientation);
            this.physicsDescription.RigidBody.ApplyImpulse(PhysicsSystem.toJVector(game.inputManager.SecondaryDirection() * 10f), PhysicsSystem.toJVector(Vector3.Zero));
            this.physicsDescription.RigidBody.ApplyImpulse(PhysicsSystem.toJVector(game.inputManager.Acceleration() * 10f), PhysicsSystem.toJVector(Vector3.Zero));

            base.Update(gametime);
        }

        public override void Draw(GameTime gametime)
        {
            //basicEffect.CurrentTechnique.Passes[0].Apply();
            //basicEffect.World = this.worldMatrix;
            //basicEffect.View = game.camera.view;
            //basicEffect.Projection = game.camera.projection;

            //this.model.Draw(game.GraphicsDevice, this.worldMatrix, game.camera.view, game.camera.projection, basicEffect);
            effect.Parameters["World"].SetValue(this.worldMatrix);
            effect.Parameters["Projection"].SetValue(game.camera.projection);
            effect.Parameters["View"].SetValue(game.camera.view);
            effect.Parameters["cameraPos"].SetValue(game.camera.position);
            effect.Parameters["worldInvTrp"].SetValue(Matrix.Transpose(Matrix.Invert(this.worldMatrix)));
            //effect.Parameters["Time"].SetValue((float)gametime.TotalGameTime.TotalSeconds);

            //this.model.Draw(game.GraphicsDevice, this.worldMatrix, game.camera.view, game.camera.projection, effect);

            foreach (var pass in this.effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                if (model != null)
                {
                    //model.Draw(game.GraphicsDevice, Matrix.Identity, Matrix.Identity, Matrix.Identity, basicEffect);
                    foreach (ModelMesh mesh in model.Meshes)
                    {
                        foreach (ModelMeshPart part in mesh.MeshParts)
                        {
                            part.Effect = effect;
                            part.Draw(game.GraphicsDevice);
                        }
                    }
                }
            }
        }
    }
}
