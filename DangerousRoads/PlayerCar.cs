using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DangerousRoads
{
    class PlayerCar
    {
        public Level Level
        {
            get { return level; }
        }
        Level level;

        public bool IsAlive
        {
            get { return isAlive; }
        }
        bool isAlive;

        public int Health
        {
            get { return health; }
            set { Health = value; }
        }
        int health;

        // Physics state
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        Vector2 position;

        private float previousBottom;

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        Vector2 velocity;

    }
}
