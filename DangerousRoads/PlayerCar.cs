using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DangerousRoads
{
    class PlayerCar
    {
        private float movement;

        private const float Acceleration = 14000.0f;
        private const float MaxSpeed = 2000.0f;
        private const float LateralSpeed = 200.0f;
        
        private const float MoveStickScale = 1.0f;

        private float lastFuelUnitTime; // how many milliseconds elapsed since a fuel unit was consumed
        float FuelConsumption = 1000;// how many milliseconds until a fuel unit is consumed

        public bool isSpinning;


        public int FuelRemaining;

        public Rectangle BoundingRectangle
        {
            get
            {
                int left = (int)position.X + (texture.Width  - boundingBox.Width ) / 2;
                int top =  (int)position.Y + (texture.Height - boundingBox.Height) / 2;

                return new Rectangle(left, top, boundingBox.Width, boundingBox.Height);
            }
        }

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
        
        public Rectangle BoundingBox
        {
            get { return boundingBox; }
            set { boundingBox = value; }
        }
        Rectangle boundingBox;
        
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

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }
        Texture2D texture;

        public float Speed
        {
            get { return speed; }
            set { speed = value; }
        }
        float speed;

        public PlayerCar(Level level, Vector2 position)
        {
            this.level = level;
            this.position = position;
            FuelRemaining = level.StartFuel;
            LoadContent();
            Speed = 200;
            lastFuelUnitTime = 0;
            isSpinning = false;
            isAlive = true;
        }

        public void LoadContent()
        {
            // texture
            texture=Level.Content.Load<Texture2D>("Sprites/players_car");
            boundingBox = new Rectangle(15, 3, 33, 57);
        }

        public void Update(GameTime gameTime)
        {
            GetInput();

            ApplyPhysics(gameTime);

            // fuel consumption
            if (lastFuelUnitTime >= FuelConsumption)
            {
                FuelRemaining--;
                lastFuelUnitTime = 0.0f;
            }
            else lastFuelUnitTime += gameTime.ElapsedGameTime.Milliseconds;

            movement = 0.0f;
        }

        public void ApplyPhysics(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 previousPosition = Position;

            position.X += movement * LateralSpeed * elapsed;
            position.Y -= Speed * elapsed;

            HandleCollisions(previousPosition);
        }

        private void GetInput()
        {
            // Get input state.
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState keyboardState = Keyboard.GetState();

            // Get analog horizontal movement.
            movement = gamePadState.ThumbSticks.Left.X * MoveStickScale;

            // If any digital horizontal movement input is found, override the analog movement.
            // left/right movement
            if (gamePadState.IsButtonDown(Buttons.DPadLeft) ||
                keyboardState.IsKeyDown(Keys.Left) ||
                keyboardState.IsKeyDown(Keys.A))
            {
                movement = -1.0f;
            }
            else if (gamePadState.IsButtonDown(Buttons.DPadRight) ||
                     keyboardState.IsKeyDown(Keys.Right) ||
                     keyboardState.IsKeyDown(Keys.D))
            {
                movement = 1.0f;
            }
            
            // acceleration/deceleration
            if (gamePadState.IsButtonDown(Buttons.DPadUp) || keyboardState.IsKeyDown(Keys.Up))
                Speed = 400;
            else if (gamePadState.IsButtonDown(Buttons.DPadDown) || keyboardState.IsKeyDown(Keys.Down))
                Speed = 100;
            else Speed = 200;

        }

        public void HandleCollisions(Vector2 initialPosition)
        {
            // collision with left wall ( road border )
            if ( (position.X + (texture.Width - boundingBox.Width)/2 ) < level.roadX1)
            {
                position.X = level.roadX1 - (texture.Width - boundingBox.Width) / 2;
            }

            // collision with right wall ( road border ) 
            else if(( position.X + (texture.Width - boundingBox.Width)/2 + boundingBox.Width ) > level.roadX2 )
            {
                position.X = level.roadX2 - ((texture.Width - boundingBox.Width)/2 + boundingBox.Width);
            }

            // collision with other cars
            foreach (AICar car in level.AICars)
            {
                int xdiff = car.BoundingRectangle.X - BoundingRectangle.X;
                int ydiff = BoundingRectangle.Y - car.BoundingRectangle.Y;
                if (ydiff < car.BoundingRectangle.Height)
                {
                    //System.Windows.Forms.MessageBox.Show("AI Car\nBB:    " + car.BoundingRectangle.ToString() +
                    //                                             "pos:   " + car.position.ToString() +
                    //                                   "\nPlayer\nBB:    " + BoundingRectangle.ToString() +
                    //                                             "pos:   " + position.ToString()); 
                    // possible collision
                    if (BoundingRectangle.X + BoundingRectangle.Width > car.BoundingRectangle.X) // also intersecting X
                        if (xdiff > 0) // ai car is to the right
                        {
                            position.X = car.BoundingRectangle.X - BoundingRectangle.Width + 1;
                        }
                }
            }
        }


        public void Reset(Vector2 position)
        {
            Position = position;
            Velocity = Vector2.Zero;
            isAlive = true;
        }

        internal void Draw(GameTime gameTime, SpriteBatch spriteBatch,Vector2 drawPosition)
        {
            // draw the player's car
            spriteBatch.Draw(texture, drawPosition, Color.White);
            
            if (level.game.showDebugInfo)
                spriteBatch.DrawString(level.debufInfoFont,
                    position.ToString() + "\n" + BoundingRectangle.ToString() + "\n" + drawPosition.ToString(),
                    new Vector2(
                        drawPosition.X + boundingBox.Width + 20,
                        drawPosition.Y),
                    Color.DarkGreen
                    );
           
        }
    }
}
