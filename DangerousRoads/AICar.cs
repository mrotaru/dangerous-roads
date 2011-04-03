using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace DangerousRoads
{
    class AICar
    {
        int lane;
        float speed;
        Vector2 position;
        Texture2D texture;
        Rectangle boundingBox;
        bool isSwitchingLanes;
    }
}
