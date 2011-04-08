using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DangerousRoads
{
    class Car
    {
        public Vector2 position;

        public Vector2 Velocity { get { return velocity; } }
        Vector2 velocity;

        public int Width { get { return width; } }
        int width;   // = textureOffset.Width

        public int Height { get { return height; } }
        int height;  // = textureOffset.Height

        Texture2D texture;
        Rectangle textureOffset;
        
        Level level;

        int speed;
        
        public Car(Level _level, Vector2 initialPosition, int initialSpeed, string textureName, Rectangle _textureOffset)
        {
            level = _level;
            position = initialPosition;
            speed = initialSpeed;
            velocity = new Vector2(0, initialSpeed);
            textureOffset = _textureOffset;
            width = textureOffset.Width;
            height = textureOffset.Height;

            LoadContent(textureName);
        }

        internal void LoadContent(string textureName)
        {
            texture = level.Content.Load<Texture2D>(textureName);

        }

        public void Update(GameTime gameTime)
        {
            ApplyPhysics(gameTime);
        }

        public void ApplyPhysics(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            position.X += velocity.X * elapsed;
            position.Y -= velocity.Y * elapsed;
        }

        public void HandleCollisions()
        {

        }

        internal void Draw( SpriteBatch spriteBatch)
        {
             //only draw cars that are in the currently rendered
             //portion of the road. 
            if (! (position.Y < level.endY  &&
                  (level.startY - position.Y) <= height))
                return;

            Vector2 drawPosition = new Vector2(
                position.X - textureOffset.Left, 
                (-1)*(level.startY - position.Y) + textureOffset.Top );

            spriteBatch.Draw(texture, drawPosition, Color.White);

            if (level.game.showDebugInfo)
                spriteBatch.DrawString(level.debufInfoFont,
                      "Pos:  " + position.X.ToString() + ", " + position.Y.ToString() +
                    "\nDraw: " + drawPosition.X.ToString() + ", " + drawPosition.Y.ToString(),
                    new Vector2(
                        drawPosition.X + width + 10,
                        drawPosition.Y),
                    Color.LightCyan
                    );

        }

    }
}