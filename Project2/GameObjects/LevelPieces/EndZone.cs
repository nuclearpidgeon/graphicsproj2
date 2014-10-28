using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project2.GameObjects.Abstract;
using Project2.GameObjects.PhysicsPuzzles;
using SharpDX;

namespace Project2.GameObjects.LevelPieces
{
    /// <summary>
    /// End zone piece contains a end zone object in the centre of a flat plane
    /// </summary>
    class EndZone : LevelPlane
    {
        public ModelPhysicsObject endGoal;
        public EndZone(Project2Game game, Level level, Vector3 position)
            : base(game, level, position, SlopeType.Flat)
        {
            var endPuzzle = new EndGoal(game, this,
                position + new Vector3(Level.PreferedTileWidth/2.0f, 0, Level.PreferedTileHeight/2.0f));
            this.physicsPuzzles.Add(endPuzzle);
            this.endGoal = endPuzzle.obelisk;
        }
    }
}
