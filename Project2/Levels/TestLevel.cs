﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Project2.GameObjects;
using Project2.GameObjects.Boids;
using Project2.GameObjects.LevelPieces;
using Project2.GameObjects.PhysicsPuzzles;

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
            // Create all the level planes

            int slopeHeight = 24;
            int levelLength = 10;

            for (int i = 0; i < levelLength; i++)
            {
                LevelPlane.SlopeType slopeType;
                float yHeight = 0f;
                // Determine what slope type this piece will be
                // This switch achieves a consistent ramping up and down across the level
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
                // Add the piece
                LevelPlane newPlane = new LevelPlane(
                    this.game, 
                    this, 
                    new Vector3(
                        0f, 
                        yHeight, 
                        (float)PreferedTileHeight * i
                    ), 
                    slopeType, 
                    (float)slopeHeight, 
                    PreferedTileWidth, 
                    PreferedTileHeight
                );
                LevelPieces.Add(newPlane);
                // Add a brick wall every 2nd flat plane
                if (i % 8 == 2)
                {
                    BrickWall newBrickWall = new BrickWall(
                        this.game,
                        newPlane, //can probably refactor this reference out through a proper 'add physics puzzle method
                        Vector3.Zero,
                        3,
                        3,
                        false);
                    newPlane.physicsPuzzles.Add(newBrickWall);
                }
            }


            // Create boids

            int flockSquareSize = 3;
            // Friendlies
            for (int i = 0; i < flockSquareSize; i++)
            {
                for (int j = 0; j < flockSquareSize; j++)
                {
                    flock.AddBoid(Flock.BoidType.Friendly, getStartPosition() + new Vector3((float)((flockSquareSize / 2.0 - i) * 4), 10f, (float)(flockSquareSize / 2.0 - j) * 4));
                }
            }
            // Enemies
            for (int i = 0; i < flockSquareSize; i++)
            {
                for (int j = 0; j < flockSquareSize; j++)
                {
                    flock.AddBoid(Flock.BoidType.Enemy, getStartPosition() + new Vector3((float)((flockSquareSize / 2.0 - i) * 4), 20f, (float)(flockSquareSize / 2.0 - j) * 4));
                }
            }
        }
    }
}
