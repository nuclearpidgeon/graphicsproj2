﻿//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Project2.GameObjects;
//using Project2.GameObjects.Abstract;
//using SharpDX;
//using SharpDX.Toolkit;

//using Jitter;
//using Jitter.Dynamics;
//using Jitter.Collision.Shapes;
//using Jitter.LinearMath;

//namespace Project2
//{
//    using SharpDX.Toolkit.Graphics;
//    class Cube : PhysicsObject
//    {

//        public Cube(Project2Game game, Vector3 position, Vector3 scale, Bool dynamic) 
//            : base(game, null, position, dynamic)
//        {

//            // physics system stuff
            
//            this.size = size / 2.0f;
//            Terrain t = new Terrain(this.game);
//            Shape boxShape;

//            if (dynamic)
//            {
//                List<Shape> shapelist = new List<Shape>();
//                shapelist.Add(new Jitter.Collision.Shapes.SphereShape(2f));
//                //shapelist.Add(new Jitter.Collision.Shapes.CapsuleShape(1f, 0.1f));
//                shapelist.Add(new Jitter.Collision.Shapes.ConeShape(2f, 0.5f));
//                boxShape = new Jitter.Collision.Shapes.MinkowskiSumShape(shapelist);

//            }
//            else {
//                boxShape = new Jitter.Collision.Shapes.TerrainShape(t.TerrainData, 1f, 1f);
//            }
//            var material = new Jitter.Dynamics.Material();

//            body = new RigidBody(boxShape);
//            body.Mass = 1.0f;
//            if (!dynamic) {
//                body.IsStatic = true;
//                //material.KineticFriction = 0.0f; // for slow surfaces
//                //material.StaticFriction = 0f; // for sticky-when-stopped surfaces
//                //material.Restitution = 1.5f;  // for bouncy surfaces
//                body.Material = material;
                
//            }
//            body.IsStatic = !dynamic;
//            body.AffectedByGravity = dynamic;
//            body.EnableDebugDraw = true;
//            body.Position = PhysicsSystem.toJVector(position);
//            if (!dynamic) {
//                body.Position = new JVector(0f, -50f, 0);
//            }
//            game.physics.World.AddBody(body);

//            // predeclare points

//            Vector3 frontBottomLeft = new Vector3(-1.0f, -1.0f, -1.0f);
//            Vector3 frontTopLeft = new Vector3(-1.0f, 1.0f, -1.0f);
//            Vector3 frontTopRight = new Vector3(1.0f, 1.0f, -1.0f);
//            Vector3 frontBottomRight = new Vector3(1.0f, -1.0f, -1.0f);
//            Vector3 backBottomLeft = new Vector3(-1.0f, -1.0f, 1.0f);
//            Vector3 backBottomRight = new Vector3(1.0f, -1.0f, 1.0f);
//            Vector3 backTopLeft = new Vector3(-1.0f, 1.0f, 1.0f);
//            Vector3 backTopRight = new Vector3(1.0f, 1.0f, 1.0f);

//            Vector3 frontBottomLeftNormal = new Vector3(-0.333f, -0.333f, -0.333f);
//            Vector3 frontTopLeftNormal = new Vector3(-0.333f, 0.333f, -0.333f);
//            Vector3 frontTopRightNormal = new Vector3(0.333f, 0.333f, -0.333f);
//            Vector3 frontBottomRightNormal = new Vector3(0.333f, -0.333f, -0.333f);
//            Vector3 backBottomLeftNormal = new Vector3(-0.333f, -0.333f, 0.333f);
//            Vector3 backBottomRightNormal = new Vector3(0.333f, -0.333f, 0.333f);
//            Vector3 backTopLeftNormal = new Vector3(-0.333f, 0.333f, 0.333f);
//            Vector3 backTopRightNormal = new Vector3(0.333f, 0.333f, 0.333f);
            

//            vertices = Buffer.Vertex.New(
//                game.GraphicsDevice,
//                new[]
//                    {
//                    new VertexPositionNormalColor(frontBottomLeft, frontBottomLeftNormal, Color.Red), // Front
//                    new VertexPositionNormalColor(frontTopLeft, frontTopLeftNormal, Color.Green),
//                    new VertexPositionNormalColor(frontTopRight, frontTopRightNormal, Color.Blue),
//                    new VertexPositionNormalColor(frontBottomLeft, frontBottomLeftNormal, Color.Orange),
//                    new VertexPositionNormalColor(frontTopRight, frontTopRightNormal, Color.Orange),
//                    new VertexPositionNormalColor(frontBottomRight, frontBottomRightNormal, Color.Orange),
//                    new VertexPositionNormalColor(backBottomLeft, backBottomLeftNormal, Color.Orange), // BACK
//                    new VertexPositionNormalColor(backTopRight, backTopRightNormal, Color.Orange),
//                    new VertexPositionNormalColor(backTopLeft, backTopLeftNormal, Color.Orange),
//                    new VertexPositionNormalColor(backBottomLeft, backBottomLeftNormal, Color.Orange),
//                    new VertexPositionNormalColor(backBottomRight, backBottomRightNormal, Color.Orange),
//                    new VertexPositionNormalColor(backTopRight, backTopRightNormal, Color.Orange),
//                    new VertexPositionNormalColor(frontTopLeft, frontTopLeftNormal, Color.OrangeRed), // Top
//                    new VertexPositionNormalColor(backTopLeft, backTopLeftNormal, Color.OrangeRed),
//                    new VertexPositionNormalColor(backTopRight, backTopRightNormal, Color.OrangeRed),
//                    new VertexPositionNormalColor(frontTopLeft, frontTopLeftNormal, Color.OrangeRed),
//                    new VertexPositionNormalColor(backTopRight, backTopRightNormal, Color.OrangeRed),
//                    new VertexPositionNormalColor(frontTopRight, frontTopRightNormal, Color.OrangeRed),
//                    new VertexPositionNormalColor(frontBottomLeft, frontBottomLeftNormal, Color.OrangeRed), // Bottom
//                    new VertexPositionNormalColor(backBottomRight, backBottomRightNormal, Color.OrangeRed),
//                    new VertexPositionNormalColor(backBottomLeft, backBottomLeftNormal, Color.OrangeRed),
//                    new VertexPositionNormalColor(frontBottomLeft, frontBottomLeftNormal, Color.OrangeRed),
//                    new VertexPositionNormalColor(frontBottomRight, frontBottomRightNormal, Color.OrangeRed),
//                    new VertexPositionNormalColor(backBottomRight, backBottomRightNormal, Color.OrangeRed),
//                    new VertexPositionNormalColor(frontBottomLeft, frontBottomLeftNormal, Color.DarkOrange), // Left
//                    new VertexPositionNormalColor(backBottomLeft, backBottomLeftNormal, Color.DarkOrange),
//                    new VertexPositionNormalColor(backTopLeft, backTopLeftNormal, Color.DarkOrange),
//                    new VertexPositionNormalColor(frontBottomLeft, frontBottomLeftNormal, Color.DarkOrange),
//                    new VertexPositionNormalColor(backTopLeft, backTopLeftNormal, Color.DarkOrange),
//                    new VertexPositionNormalColor(frontTopLeft, frontTopLeftNormal, Color.DarkOrange),
//                    new VertexPositionNormalColor(frontBottomRight, frontBottomRightNormal, Color.DarkOrange), // Right
//                    new VertexPositionNormalColor(backTopRight, backTopRightNormal, Color.DarkOrange),
//                    new VertexPositionNormalColor(backBottomRight, backBottomRightNormal, Color.DarkOrange),
//                    new VertexPositionNormalColor(frontBottomRight, frontBottomRightNormal, Color.DarkOrange),
//                    new VertexPositionNormalColor(frontTopRight, frontTopRightNormal, Color.DarkOrange),
//                    new VertexPositionNormalColor(backTopRight, backTopRightNormal, Color.DarkOrange),
//                });

//            inputLayout = VertexInputLayout.FromBuffer(0, vertices);
//            basicEffect.EnableDefaultLighting();
//        }

//        public override void Update(GameTime gameTime)
//        {
//            // Rotate the cube.
//            var time = (float)gameTime.TotalGameTime.TotalSeconds;
//            //basicEffect.World = Matrix.RotationX(time) * Matrix.RotationY(time * 2.0f) * Matrix.RotationZ(time * .7f);

//            // get the orientation from physics system
//            // TODO: Make this object owner of individual RigidBody
//            Vector3 translation = PhysicsSystem.toVector3(body.Position);
//            Matrix orientation = PhysicsSystem.toMatrix(body.Orientation);
//            orientation.TranslationVector = translation;

//            basicEffect.World = Matrix.Scaling(size) * orientation;

//            if (game.inputManager.Jump()) {
//                if (body.IsStatic == false) {
//                    body.ApplyImpulse(PhysicsSystem.toJVector(new Vector3(0f, 1f, 0f)), PhysicsSystem.toJVector(Vector3.Zero));   

//                }
//            }
//            if (game.inputManager.SecondaryDirection() != Vector3.Zero) {
//                if (body.IsStatic == false) { 
//                    body.ApplyImpulse(PhysicsSystem.toJVector(game.inputManager.SecondaryDirection() * 0.1f), PhysicsSystem.toJVector(Vector3.Zero));
//                }
//            }
//            // handle superclass update
//            base.Update(gameTime);
//        }

//        public override void Draw(GameTime gameTime)
//        {
//            // Setup the vertices
//            game.GraphicsDevice.SetVertexBuffer(vertices);
//            game.GraphicsDevice.SetVertexInputLayout(inputLayout);

//            // Apply the basic effect technique and draw the rotating cube
//            basicEffect.CurrentTechnique.Passes[0].Apply();
//            //game.GraphicsDevice.Draw(PrimitiveType.TriangleList, vertices.ElementCount);
//            body.DebugDraw(game.debugDrawer);
//        }
//    }
//}
