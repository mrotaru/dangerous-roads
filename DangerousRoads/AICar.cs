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
        public int lane;
        public float speed;
        public Vector2 position;
        Texture2D texture;
        public Rectangle boundingBox;
        public int movement;
        Level level;
        
        float LateralSpeed=100;
        
        public bool isSwitchingLanes;

        public static int count;
        public int number;

        public Rectangle PhysicalBounds
        {
            get { return physicalBounds; }
            set { physicalBounds = value; }
        }
        Rectangle physicalBounds;

        public AICar( Level mlevel,Vector2 startPosition, float startSpeed )
        {
            level = mlevel;
            texture = level.Content.Load<Texture2D>("Sprites/ai_car_1");
            boundingBox = new Rectangle(15, 3, 33, 57);
            position = startPosition;
            speed = startSpeed;
            movement = 0;
            count++;
            number = count;
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
            position.Y -= this.speed * elapsed;
        }

        public void Draw(SpriteBatch spriteBatch )
        {
            //global::System.Windows.Forms.MessageBox.Show("Drawing a car at " +
            //"\nX: " +drawPosition.X +
            //"\nY: " + drawPosition.Y);
            Vector2 drawPosition = new Vector2( position.X - physicalBounds.Left, position.Y + physicalBounds.Top);
            spriteBatch.Draw(texture,drawPosition, Color.White);
           
            if( level.game.showDebugInfo)
            spriteBatch.DrawString(level.debufInfoFont,
                position.ToString() + 
                "\n Pos:  " + position.ToString() + 
                "\n Draw: " + drawPosition.ToString() +
                "\nydiff: " + (level.playerCar.Position.Y-position.Y).ToString(),
                new Vector2( 
                    drawPosition.X + boundingBox.Width + 20, 
                    drawPosition.Y ), 
                Color.LightCyan
                );
        }
    }
}
