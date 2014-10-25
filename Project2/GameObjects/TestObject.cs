using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jitter.LinearMath;
using Project2.GameObjects.Abstract;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

using Jitter;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using SharpDX.Direct3D11;

namespace Project2.GameObjects
{
    class TestObject : PhysicsObject
    {
        public TestObject(Project2Game game, Model model, Vector3 position, Boolean isStatic)
            : base(game, model, position, GeneratePhysicsDescription(position, model, isStatic))
        {
            this.basicEffect = game.Content.Load<Effect>("Shaders/BlurCray");

        }

        private static PhysicsDescription GeneratePhysicsDescription(Vector3 position, Model model, Boolean isStatic)
        {
            
            var collisionShape = PhysicsSystem.BuildConvexHullShape(model);
            var rigidBody = new RigidBody(collisionShape)
            {
                Position = PhysicsSystem.toJVector(position),
                IsStatic = isStatic,
                EnableDebugDraw = true,
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
            base.Update(gametime);
        }

        public override void Draw(GameTime gametime)
        {
            //basicEffect.CurrentTechnique.Passes[0].Apply();
            //basicEffect.World = this.worldMatrix;
            //basicEffect.View = game.camera.view;
            //basicEffect.Projection = game.camera.projection;

            //this.model.Draw(game.GraphicsDevice, this.worldMatrix, game.camera.view, game.camera.projection, basicEffect);
            // Lights
            basicEffect.ConstantBuffers[1].Set(0, new Color4(0.8f, 0.8f, 0.8f, 1.0f)); // Ambient light
            basicEffect.ConstantBuffers[1].IsDirty = true;

            // Object
            basicEffect.ConstantBuffers[2].Set(0, positionMatrix); // LocalToWorld
            basicEffect.ConstantBuffers[2].Set(1, game.camera.projection); // LocalToProjected
            basicEffect.ConstantBuffers[2].Set(2, worldMatrix); // WorldToLocal
            basicEffect.ConstantBuffers[2].Set(3, game.camera.view); // WorldToview
            basicEffect.ConstantBuffers[2].Set(4, Matrix.Identity); // UVTransform
            basicEffect.ConstantBuffers[2].Set(5, game.camera.position); // EyePosition
            basicEffect.ConstantBuffers[2].IsDirty = true;

            // Materials
            basicEffect.ConstantBuffers[0].Set(0, new Color4(0.2f, 0.2f, 0.2f, 1.0f)); // Ambient
            basicEffect.ConstantBuffers[0].Set(1, new Color4(0.6f, 0.2f, 0.2f, 1.0f)); // Diffuse
            basicEffect.ConstantBuffers[0].Set(2, new Color4(0.2f, 0.6f, 0.2f, 1.0f)); // Specular
            basicEffect.ConstantBuffers[0].Set(4, 1.0f); // Specular power
            basicEffect.ConstantBuffers[0].IsDirty = true;

            // Misc vars
            /*basicEffect.ConstantBuffers[3].Set(0, game.GraphicsDevice.Viewport.Width); // Viewport height
            basicEffect.ConstantBuffers[3].Set(1, game.GraphicsDevice.Viewport.Height); // Viewport width
            basicEffect.ConstantBuffers[3].Set(2, gametime.TotalGameTime.TotalSeconds); // Elapsed time in seconds
            basicEffect.ConstantBuffers[3].IsDirty = true;*/

            basicEffect.Parameters["SamplerState"].SetResource(game.GraphicsDevice.SamplerStates.PointClamp);
            basicEffect.Parameters["Texture1"].SetResource(game.Content.Load<Texture>("Models\\Rocks_Brown_D"));

            foreach (var pass in this.basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                if (model != null)
                {
                    //model.Draw(game.GraphicsDevice, Matrix.Identity, Matrix.Identity, Matrix.Identity, basicEffect);
                    foreach (ModelMesh mesh in model.Meshes)
                    {
                        foreach (ModelMeshPart part in mesh.MeshParts)
                        {
                            part.Effect = basicEffect;
                            part.Draw(game.GraphicsDevice);
                        }
                    }
                }
            }
        }
    }
}
