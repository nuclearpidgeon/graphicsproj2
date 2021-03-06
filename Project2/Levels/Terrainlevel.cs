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
    using SharpDX.Toolkit.Graphics;
    public class TerrainLevel : Level
    {
        public TerrainLevel(Project2Game game) : base(game)
        {
            BuildLevel(); // virtual member call in constructor, like I give a 
        }

        public override void BuildLevel()
        {
            // Add a test terrain under the end zone for shits + gigs
            DiamondSquareTerrain terrain = new DiamondSquareTerrain(game, new Vector3(0f, 0f, 0f), 7, 4.0f, 4.0f, 30);
            AddChild(terrain);

            // Add the mandatory end-zone piece containing an end goal object
            // place tile behind start tile for testing
            var endZone = new TerrainEndZone(game, this, new Vector3(terrain.xScale * terrain.terrainWidth /2 , 
                                                                    terrain.TerrainData[terrain.terrainWidth/2,terrain.terrainHeight/2] * terrain.Amplitude, 
                                                                    terrain.zScale * terrain.terrainHeight / 2));
            AddChild(endZone); // add the piece
            this.endGoal = endZone.endGoal; // set the level's endGoal object for collision detection use

            // Create boids

            int flockSquareSize = 5;
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
                    flock.AddBoid(Flock.BoidType.Enemy, getStartPosition() + new Vector3((float)((flockSquareSize / 2.0 - i) * 6), 20f, (float)(flockSquareSize / 2.0 - j) * 6));
                }
            }


        }

        public override Vector3 getCameraStartPosition() {
            return new Vector3(0f, 40f, 0f);
        }

        // uncomment for better overview
        public override Vector3 getCameraOffset()
        {
            return new Vector3(0f, 1f, 2f) * 55;
        }
    }
}
