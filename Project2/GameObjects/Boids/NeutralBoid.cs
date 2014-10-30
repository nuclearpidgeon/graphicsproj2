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
    class NeutralBoid : Boid
    {
        public NeutralBoid(Project2Game game, Flock flock, Model model, Vector3 position)
            : base(game, flock, model, position, Flock.BoidType.Neutral)
        {

        }

        public override void Update(GameTime gametime)
        {

            base.Update(gametime);
        }
        public override void Draw(GameTime gametime)
        {
            base.Draw(gametime);
        }
    }
}
