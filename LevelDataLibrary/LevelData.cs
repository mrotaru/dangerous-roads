using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace LevelDataLibrary
{
    public class LevelData
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
    }
}
