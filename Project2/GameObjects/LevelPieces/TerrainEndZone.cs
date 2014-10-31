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
    class TerrainEndZone : LevelPiece
    {
        public ModelPhysicsObject endGoal;
        public TerrainEndZone(Project2Game game, Level level, Vector3 position)
            : base(game, level, position)
        {
            var endPuzzle = new EndGoal(game, this,
                position + new Vector3(Level.PreferedTileWidth/2.0f, 0, Level.PreferedTileHeight/2.0f));
            AddChild(endPuzzle);
            this.endGoal = endPuzzle.obelisk;
        }
    }
}
