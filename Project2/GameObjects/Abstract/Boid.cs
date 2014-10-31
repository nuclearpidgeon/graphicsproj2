using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Project2.GameObjects.Abstract;
using Project2.GameSystems;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace Project2.GameObjects.Boids
{
    public abstract class Boid : ModelPhysicsObject
    {
        private Effect effect;
        public double health;
        public double attack = 10;

        public double maxHealth = 100;

        protected Color sickColor = Color.Black;
        protected Color healthyColor = Color.Blue;

        protected Boolean ToDestroy;
        public Flock.BoidType boidType;
        public Flock flock;

        public Boid(Project2Game game, Flock flock, Model model, Vector3 position, Flock.BoidType boidType)
            : base(game, model, position)
        {
            this.health = maxHealth;
            this.PhysicsDescription.Mass = 0.25f;
            this.boidType = boidType;
            this.flock = flock;
            this.game.physics.World.CollisionSystem.CollisionDetected += HandleCollision;
            effect = game.Content.Load<Effect>("Shaders\\Cel");
        }


        /// <summary>
        /// This method handles explicit object collision logic. Is registered to physics engine CollisionDetected event handler.
        /// Fired on any detected collision, so must check if the collision applies to this object
        /// </summary>
        /// <param name="body1"></param>
        /// <param name="body2"></param>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="normal"></param>
        /// <param name="penetration"></param>
        virtual public void HandleCollision(RigidBody body1, RigidBody body2, JVector point1, JVector point2, JVector normal, float penetration)
        {
            // work out which, if any, of the collided bodies is this object, and name them semantically
            RigidBody other;
            var self = this.PhysicsDescription;
            if (body1 == self)
                other = body2;
            else if (body2 == self)
                other = body1;
            else return;

            if (other == this.flock.level.endGoal.PhysicsDescription) // we've collided with the end zone
            {
                // be careful of what you modify in this handler as it may be called during an Update()
                // attempting to modify any list (such as destroying game objects, etc) will cause an exception

                if (!ToDestroy) // incremement score once before destroy
                {
                    this.game.incScore(10);
                }
                this.Destroy(true); // remove self
            }    

            Collision(other); // do other collision stuff
        }

        protected virtual void Collision(RigidBody other)
        {

        }

        public override void Draw(GameTime gametime)
        {
            
            var healthColour = Color4.Lerp(sickColor, healthyColor, (float)(health/maxHealth));
            
        {

            effect.Parameters["World"].SetValue(this.WorldMatrix);
            effect.Parameters["Projection"].SetValue(game.camera.projection);
            effect.Parameters["View"].SetValue(game.camera.view);
            effect.Parameters["cameraPos"].SetValue(game.camera.position);
            effect.Parameters["worldInvTrp"].SetValue(Matrix.Transpose(Matrix.Invert(this.WorldMatrix)));
            // For Rainbow (required)
            //effect.Parameters["Time"].SetValue((float)gametime.TotalGameTime.TotalSeconds);

            // For Cel (both optional)
            effect.Parameters["lightAmbCol"].SetValue<Color4>(healthColour);
            effect.Parameters["objectCol"].SetValue<Color4>(new Color4(1f, 1f, 1f, 1.0f));
            effect.Parameters["quant"].SetValue<float>(2.6f);

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

        public override void Update(GameTime gametime)
        {
            if (ToDestroy)
            {
                Destroy();
            }

            if (health < 0) // destroy the boid when its health is too low
            {
                Destroy();
            }

            base.Update(gametime);
        }

        /// <summary>
        /// Implements object destroying in a manner that can be called asynchronously
        /// The first call to this function doesn't actually destroy the object, but sets a flag
        /// to destroy it. Calling this function again will remove the object.
        /// A suggested use case it to call this function if ToDestroy is inside Update()
        /// </summary>
        override public void Destroy(Boolean Async = false)
        {

            // the first call to this function
            if (ToDestroy == false)
            {
                ToDestroy = true;
                return;
            }

            if (ToDestroy && !Async)
            {
                // remove from physics system
                game.physics.RemoveBody(this.PhysicsDescription);
                // remove from graph
                this.Parent.RemoveChild(this);
            }
        }

    }

}
