﻿using System;
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
        public Monkey(Project2Game game, Model model, Vector3 position, Boolean isStatic)
            : base(game, model, position, GeneratePhysicsDescription(position, model, isStatic))
        {
            effect = game.Content.Load<Effect>("Shaders\\Rainbow");
        }

        private static PhysicsDescription GeneratePhysicsDescription(Vector3 position, Model model, Boolean isStatic)
        {
            var bounds = model.CalculateBounds();
            var collisionShape = new SphereShape(bounds.Radius*3);
            var rigidBody = new RigidBody(collisionShape)
            {
                Position = PhysicsSystem.toJVector(position),
                IsStatic = isStatic,
                EnableDebugDraw = true,
                Mass = 20f,
                Tag = "player"
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
            this.PhysicsDescription.RigidBody.ApplyImpulse(PhysicsSystem.toJVector(game.inputManager.SecondaryDirection() * 10f), PhysicsSystem.toJVector(Vector3.Zero));
            this.PhysicsDescription.RigidBody.ApplyImpulse(PhysicsSystem.toJVector(game.inputManager.Acceleration() * 10f), PhysicsSystem.toJVector(Vector3.Zero));

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
        }
    }
}
