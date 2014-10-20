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
                for (int j = 0; j < height; j++)
                {
                    float sizeScaler = 4f;
                    Vector3 brickPosition = this.originPosition + offset + new Vector3(2f * i, 1f * j, 0f) * (sizeScaler);
                    brickPosition.Y += sizeScaler/2f; //push all bricks off the ground
                    if (interleaved) {
                        if (j % 2 == 1) {
                            brickPosition.X += sizeScaler;
                        }
                    }
                    Brick newBrick = new Brick(
                        game,
                        game.models["box"],
                        brickPosition,
                        new Vector3(2f, 1f, 1f) * sizeScaler,
                        false
                    );
                    this.AddChild(newBrick);
                }
            }
        }
    }
}
