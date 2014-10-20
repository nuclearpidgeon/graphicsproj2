﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Project2.GameObjects.Abstract;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace Project2.GameObjects.Boids
{
    public abstract class Boid : PhysicsObject
    {
        public Flock.BoidType boidType;
        public Flock flock;

        public Boid(Project2Game game, Flock flock, Model model, Vector3 position, Flock.BoidType boidType)
            : base(game, model, position, GeneratePhysicsDescription(position, model, false))
        {
            this.boidType = boidType;
            this.flock = flock;
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
                Mass = 0.25f
            };

            var description = new PhysicsDescription()
            {
                IsStatic = isStatic,
                CollisionShape = collisionShape,
                Debug = false,
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
            //this.physicsDescription.RigidBody.ApplyImpulse(PhysicsSystem.toJVector(game.inputManager.SecondaryDirection() * 10f), PhysicsSystem.toJVector(Vector3.Zero));
            //this.physicsDescription.RigidBody.ApplyImpulse(PhysicsSystem.toJVector(game.inputManager.Acceleration() * 10f), PhysicsSystem.toJVector(Vector3.Zero));

            base.Update(gametime);
        }

        public override void Draw(GameTime gametime)
        {
            //basicEffect.CurrentTechnique.Passes[0].Apply();
            //basicEffect.World = this.worldMatrix;
            //basicEffect.View = game.camera.view;
            //basicEffect.Projection = game.camera.projection;

            //this.model.Draw(game.GraphicsDevice, this.worldMatrix, game.camera.view, game.camera.projection, basicEffect);
            base.Draw(gametime);
        }
    }

}