using System;
using System.Collections.Generic;
using Project2.GameObjects.Abstract;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace Project2.GameObjects.LevelPieces
{
    class LevelPlane : LevelPiece
    {
        public enum SlopeType
        {
            SlopeUp, Flat, SlopeDown
        }
        
        int xSize;
        int ySize;

        public LevelPlane(Project2Game game, Level level, Vector3 position, SlopeType slopeType, float slopeHeight = 16f, int xSize = Level.PreferedTileWidth, int ySize = Level.PreferedTileHeight)
            : base (game, level, position)
        {
            this.xSize = xSize;
            this.ySize = ySize; // this should be z size for consistency in 3D
            float frontHeight = 0f;
            float backHeight = 0f;
            if (slopeType == SlopeType.SlopeUp) { backHeight = slopeHeight; }
            if (slopeType == SlopeType.SlopeDown) { frontHeight = slopeHeight; }

            // calculate the angle of gradient
            var angle = (float)Math.Atan2(backHeight - frontHeight, xSize - 0); //y_2 - y_2 / x_2 - x_1 = gradient
            var separation = xSize; // disatance between walls (default to tile width)
            var wallWidth = 2.0f; // width of wall
            var wallHeight = 4.0f; // height of wall
            // vertical displacement of wall, offset so it sits on the ground
            var heightDisplacement = Math.Abs((backHeight - frontHeight)/2.0f) + wallHeight / 2.0f;
            // instantiate a wall for either side of the plane
            
            AddChild(new Box(game, game.models["box"], position + new Vector3(0f, heightDisplacement, ySize / 2.0f), new Vector3(wallWidth, wallHeight, ySize), new Vector3(0, -angle, 0), true));
            AddChild(new Box(game, game.models["box"], position + new Vector3(separation, heightDisplacement, ySize / 2.0f), new Vector3(wallWidth, wallHeight, ySize), new Vector3(0, -angle, 0), true));

            if (slopeType == SlopeType.Flat)
            {
                //this.physicsPuzzles.Add(new PhysicsPuzzles.SeeSaw(game, this, new Vector3(32, 0, 32)));
            }
            // add floor
            AddChild(new Terrain(game, position, xSize, ySize, frontHeight, backHeight));

        }
    }
}
