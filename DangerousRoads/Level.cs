using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using LevelDataLibrary;

namespace DangerousRoads
{
    class Level
    {

        String Name;
        int NumberOfLanes;
        int Length;
        int Speed;
        int CarProbability;
        int CarSwitchLanesProbability;
        int OilLeakProbability;
        int RoadBlockProbability;
        int TruckProbability;

        // size of road texture
        int roadTileWidth = 100;
        int roadTileHeight = 100;

        // textures
        Texture2D border;
        Texture2D road;

        ContentManager content;

        public Level(IServiceProvider serviceProvider, string path)
        {
            LevelData levelData = content.Load<LevelData>("Level_1");
            

        }



    }
}
