using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using SharpDX.Toolkit;
using Project2.GameObjects.Abstract;

namespace Project2.GameObjects.PhysicsPuzzles
{
    public class BrickWall : PhysicsPuzzle
    {
        public BrickWall(Project2Game game, LevelPiece levelPiece, Vector3 offset, int height, int width, bool interleaved) :
            base(game, levelPiece, offset)
        {
            for (int i = 0; i < width; i++)
            {
                Box newBox = new Box(
                    game,
                    game.models["box"],
                    this.originPosition + new Vector3(0f,2f*i,0f),
                    new Vector3(1f),
                    false
                );
                this.AddChild(newBox);
            }
        }
    }
}
