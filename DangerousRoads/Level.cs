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

        public String Name;
        public int NumberOfLanes;
        public int Length;
        public int Speed;
        public int CarProbability;
        public int CarSwitchLanesProbability;
        public int OilLeakProbability;
        public int RoadBlockProbability;
        public int TruckProbability;
                                   
        // size of road texture
        int roadTileWidth = 100;
        int roadTileHeight = 100;

        // textures
        Texture2D border;
        Texture2D road;

        ContentManager content;

        public Level(IServiceProvider serviceProvider, int level_number)
        {
            content = new ContentManager(serviceProvider, "Content");
            LevelData levelData = content.Load<LevelData>( String.Format("Levels/Level_{0}", level_number));
            
            Name = levelData.Name;
            NumberOfLanes = levelData.NumberOfLanes;
            Length = levelData.Length;
            Speed = levelData.Speed;
            CarProbability = levelData.CarProbability;
            CarSwitchLanesProbability = levelData.CarSwitchLanesProbability;
            OilLeakProbability = levelData.OilLeakProbability;
            RoadBlockProbability = levelData.RoadBlockProbability;
            TruckProbability = levelData.TruckProbability;


        }



    }
}
