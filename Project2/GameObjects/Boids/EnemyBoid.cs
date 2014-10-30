using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jitter.Dynamics;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace Project2.GameObjects.Boids
{
    class EnemyBoid : Boid
    {
        const double playerAvoidRadius = 20;
        const double attackRadius = 20;
        
        public EnemyBoid(Project2Game game, Flock flock, Model model, Vector3 position)
            : base(game, flock, model, position, Flock.BoidType.Enemy)
        {
            maxHealth = 80;
            health = maxHealth;
            attack = 20;
            healthyColor = Color.Red;
        }

        public override void Update(GameTime gametime)
        {
            var dist_to_player = game.level.player.Position - this.Position;
            if (dist_to_player.Length() < playerAvoidRadius)
            {
                var dir_to_player =Vector3.Normalize(dist_to_player); // avoid player
                this.PhysicsDescription.ApplyImpulse(PhysicsSystem.toJVector(dir_to_player) * -0.3f);
            }

            // attack friendly boids
            foreach (var boid in flock.Children.Where(b => ((Boid)b).boidType == Flock.BoidType.Friendly && b != this))
            {
                var distance = ((Boid)boid).Position - this.Position;
                if (distance.Length() < attackRadius)
                {
                    var maxImpulse = 0.006f;
                    // compare health percentages of prey to self
                    var healthRatio = (this.health / this.maxHealth) / (Math.Abs(((Boid)boid).health / ((Boid)boid).maxHealth) + 1);
                    var dir_to_enemy = Vector3.Normalize(distance);
                    var impulse = healthRatio*maxImpulse;
                    if (healthRatio < 1)
                    {
                        impulse *= 5; // make the boids daring if it's risky
                    }
                    this.PhysicsDescription.ApplyImpulse(PhysicsSystem.toJVector(dir_to_enemy) * (float)impulse);
                }
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
                var otherBoid = ((Boid) other.Tag);
                // if we collide with an opposing boid, deal damage
                if (otherBoid.boidType == Flock.BoidType.Friendly)
                {
                    this.health -= otherBoid.attack;
                }
            }
            base.Collision(other);
        }
    }
}
