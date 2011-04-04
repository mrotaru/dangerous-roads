﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace DangerousRoads
{
    class AICar
    {
        public int lane;
        public float speed;
        public Vector2 position;
        Texture2D texture;
        public Rectangle boundingBox;
        public int movement;
        Level level;
        
        float LateralSpeed=100;
        
        public bool isSwitchingLanes;

        public AICar( Level mlevel,Vector2 startPosition, float startSpeed )
        {
            level = mlevel;
            texture = level.Content.Load<Texture2D>("Sprites/ai_car_1");
            boundingBox = new Rectangle(15, 3, 33, 57);
            position = startPosition;
            speed = startSpeed;
            movement = 0;
        }

        public void Update(GameTime gameTime)
        {

            ApplyPhysics(gameTime);
        }

        public void ApplyPhysics(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 previousPosition = position;

            position.X += movement*LateralSpeed * elapsed;
            position.Y -= speed * elapsed;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 drawPosition)
        {
            //global::System.Windows.Forms.MessageBox.Show("Drawing a car at " +
            //"\nX: " +drawPosition.X +
            //"\nY: " + drawPosition.Y);
            spriteBatch.Draw(texture, drawPosition, Color.White);
        }
    }
}
