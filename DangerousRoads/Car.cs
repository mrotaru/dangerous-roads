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
        Vector2 position;
        Vector2 velocity;

        int width;   // = textureOffset.Width
        int height;  // = textureOffset.Height

        Texture2D texture;
        Rectangle textureOffset;
        
        Level level;

        int speed;

        Car(Level _level, Vector2 initialPosition, int initialSpeed, string textureName, Rectangle _textureOffset)
        {
            level = _level;
            position = initialPosition;
            speed = initialSpeed;
            textureOffset = _textureOffset;
            width = textureOffset.Width;
            height = textureOffset.Height;

            LoadContent(textureName);
        }

        internal void LoadContent(string textureName)
        {
            texture = level.Content.Load<Texture2D>(textureName);

        }

        void Update(GameTime gameTime)
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

        internal void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 drawPosition = new Vector2(position.X + textureOffset.Left, level.endY - position.Y + textureOffset.Top);

            spriteBatch.Draw(texture, drawPosition, Color.White);

            if (level.game.showDebugInfo)
                spriteBatch.DrawString(level.debufInfoFont,
                      "Pos:  " + position.X.ToString() + ", " + position.Y.ToString() +
                    "\nDraw: " + drawPosition.X.ToString() + ", " + drawPosition.Y.ToString(),
                    new Vector2(
                        drawPosition.X + width + 20,
                        drawPosition.Y),
                    Color.LightCyan
                    );

        }

    }
}