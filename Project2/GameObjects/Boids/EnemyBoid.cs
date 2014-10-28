using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        }

        public override void Update(GameTime gametime)
        {
            var dist_to_player = game.level.player.Position - this.Position;
            if (dist_to_player.Length() < playerAvoidRadius)
            {
                var dir_to_player =Vector3.Normalize(dist_to_player); // avoid player
                this.PhysicsDescription.RigidBody.ApplyImpulse(PhysicsSystem.toJVector(dir_to_player) * -0.3f);
            }

            // attack friendly boids
            foreach (var boid in flock.boidList.Where(b => b.boidType == Flock.BoidType.Friendly && b != this))
            {
                var distance = boid.Position - this.Position;
                if (distance.Length() < attackRadius)
                {
                    this.PhysicsDescription.RigidBody.ApplyImpulse(PhysicsSystem.toJVector(distance) * 0.001f);
                }
            }


            base.Update(gametime);
        }
        public override void Draw(GameTime gametime)
        {
            base.Draw(gametime);
        }
    }
}
