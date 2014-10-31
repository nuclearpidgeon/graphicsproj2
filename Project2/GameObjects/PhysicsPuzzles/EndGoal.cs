using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jitter.Collision;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Project2.GameObjects.Abstract;
using Project2.GameObjects.Boids;
using SharpDX;
using SharpDX.Toolkit;

namespace Project2.GameObjects.PhysicsPuzzles
{
    class EndGoal : PhysicsPuzzle
    {
        public ModelPhysicsObject obelisk;
        public EndGoal(Project2Game game, LevelPiece levelPiece, Vector3 offset) : base(game, levelPiece, offset)
        {
            // place obelisk
            var scale = new Vector3(6, 6, 6);
            obelisk = new Obelisk(game, offset + new Vector3(0, scale.Y * 0.5f, 0), scale);
            obelisk.PhysicsDescription.IsStatic = true;
            this.AddChild(obelisk);
        }
    }

}
