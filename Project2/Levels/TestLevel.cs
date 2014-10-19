using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project2.GameObjects;
using Project2.GameObjects.Boids;
using Project2.GameObjects.LevelPieces;
using SharpDX;
using SharpDX.Toolkit;


namespace Project2.Levels
{
    class TestLevel : Level
    {
        public TestLevel(Project2Game game) : base(game)
        {
            BuildLevel(); // virtual member call in constructor, like I give a 
        }

        public override void BuildLevel()
        {


            var slopeHeight = 24;
            for (int i = 0; i < 10; i++)
            {
                LevelPlane.SlopeType slopeType;
                float yHeight = 0f;
                switch (i % 4)
                {
                    case 0:
                        slopeType = LevelPlane.SlopeType.Flat;
                        break;
                    case 1:
                        slopeType = LevelPlane.SlopeType.SlopeUp;
                        break;
                    case 2:
                        slopeType = LevelPlane.SlopeType.Flat;
                        yHeight = (float)slopeHeight;
                        break;
                    case 3:
                        slopeType = LevelPlane.SlopeType.SlopeDown;
                        break;
                    default:
                        // just in case yea
                        slopeType = LevelPlane.SlopeType.Flat;
                        break;
                }
                LevelPieces.Add(new LevelPlane(this.game, this, new Vector3(0f, yHeight, (float)PreferedTileHeight * i), slopeType, (float)slopeHeight, PreferedTileWidth, PreferedTileHeight));
            }

            int size = 3;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    flock.AddBoid(Flock.BoidType.Friendly, getStartPosition() + new Vector3((float)((size / 2.0 - i) * 4), 10f, (float)(size / 2.0 - j) * 4));
                }
            }

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    flock.AddBoid(Flock.BoidType.Enemy, getStartPosition() + new Vector3((float)((size / 2.0 - i) * 4), 20f, (float)(size / 2.0 - j) * 4));
                }
            }
        }
    }
}
