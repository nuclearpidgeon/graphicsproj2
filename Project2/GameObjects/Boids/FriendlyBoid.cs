using System;
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
    class FriendlyBoid : Boid
    {

        private const double playerRadius = 20;
        private const double selfRadius = 5;
        private const double enemyRadius = 5;
        private const double endzoneRadius = 3;


        public FriendlyBoid(Project2Game game, Flock flock, Model model, Vector3 position) : base(game, flock, model, position, Flock.BoidType.Friendly)
        {

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
                boid_centroid += distance;

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


            if (dist_to_player.Length() < playerRadius)
            {
                var dir_to_player = Vector3.Normalize(dist_to_player);
                this.PhysicsDescription.ApplyImpulse(PhysicsSystem.toJVector(dir_to_player) * 0.17f);
            }

            // magnetise the boids to the end zone obelisk when in range
            var dir_to_endzone = game.level.endGoal.Position - this.Position;
            if (dir_to_endzone.Length() < endzoneRadius)
            {
                this.PhysicsDescription.ApplyImpulse(PhysicsSystem.toJVector(dir_to_endzone) * 0.5f);
            }

            base.Update(gametime);
        }
        public override void Draw(GameTime gametime)
        {
            base.Draw(gametime);
        }
    }
}
