using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        Vector2 velocity;

        // Physics state
        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }
        Texture2D texture;

        public PlayerCar(Level level, Vector2 position)
        {
            this.level = level;
            this.position = position;
            LoadContent();
        }

        public void LoadContent()
        {
            // texture
            texture=Level.Content.Load<Texture2D>("Sprites/players_car");
        }


        internal void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // draw the player's car
            Vector2 player_position = new Vector2( 200 , 400 );
            spriteBatch.Draw(texture, position, Color.White);
           
        }
    }
}
