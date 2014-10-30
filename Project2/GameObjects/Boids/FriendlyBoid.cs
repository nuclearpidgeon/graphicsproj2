using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Jitter.Dynamics;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace Project2.GameObjects.Boids
{
    class FriendlyBoid : Boid
    {

        private const double playerRadius = 20;
        private const double playerMagnetRadius = 3;
        private const double selfRadius = 5;
        private const double enemyRadius = 5;
        private const double endzoneRadius = 5;


        public FriendlyBoid(Project2Game game, Flock flock, Model model, Vector3 position) : base(game, flock, model, position, Flock.BoidType.Friendly)
        {
            maxHealth = 200;
            health = maxHealth;
            attack = 10;
            healthyColor = Color.Green;
        }


        public override void Update(GameTime gametime)
        {
            var dist_to_player = game.level.player.Position - this.Position;
            
            var boid_centroid = Vector3.Zero; // interact with boids of the same type
            foreach (var boid in flock.Children.Where(b => ((Boid)b).boidType == this.boidType && b != this))
            {
                var distance = ((Boid)boid).Position - this.Position;
                if (distance.Length() < selfRadius)
                {
                    this.PhysicsDescription.ApplyImpulse(PhysicsSystem.toJVector(distance) * -0.01f);

                }
                boid_centroid += Vector3.Normalize(distance) * (1 / distance.LengthSquared());

            }
            this.PhysicsDescription.ApplyImpulse(PhysicsSystem.toJVector(Vector3.Normalize(boid_centroid)) * 0.05f);

            // avoid enemy boids
            foreach (var boid in flock.Children.Where(b => ((Boid)b).boidType == Flock.BoidType.Enemy && b != this))
            {
                
                var distance = ((Boid)boid).Position - this.Position;
                if (distance.Length() < enemyRadius)
                {
                    this.PhysicsDescription.ApplyImpulse(PhysicsSystem.toJVector(distance) * -0.2f);
                }
                boid_centroid += distance;

            }
            this.PhysicsDescription.ApplyImpulse(PhysicsSystem.toJVector(Vector3.Normalize(boid_centroid)) * 0.05f);


            if (dist_to_player.Length() < playerRadius) // attract boids to player
            {
                var dir_to_player = Vector3.Normalize(dist_to_player);
                this.PhysicsDescription.ApplyImpulse(PhysicsSystem.toJVector(dir_to_player) * 0.17f);
                if (dist_to_player.Length() < playerMagnetRadius) // hold on tight if we're very close
                {
                    this.PhysicsDescription.ApplyImpulse(PhysicsSystem.toJVector(dir_to_player) * 0.17f);
                }
            }

            // magnetise the boids to the end zone obelisk when in range
            var dir_to_endzone = game.level.endGoal.Position - this.Position;
            if (dir_to_endzone.Length() < endzoneRadius)
            {
                this.PhysicsDescription.ApplyImpulse(PhysicsSystem.toJVector(dir_to_endzone) * 3f);
            }

            base.Update(gametime);
        }

        protected override void Collision(RigidBody other)
        {
            if (other.Tag == null)
            {
                return;
            }
            // determine if it's a boid we collided with
            if ((other.Tag).GetType().GetTypeInfo().IsSubclassOf(typeof(Boid)))
            {
                var otherBoid = ((Boid)other.Tag);
                // if we collide with an opposing boid, deal damage
                if (otherBoid.boidType == Flock.BoidType.Enemy)
                {
                    this.health -= otherBoid.attack;
                }
            }
            base.Collision(other);
        }
    }
}
