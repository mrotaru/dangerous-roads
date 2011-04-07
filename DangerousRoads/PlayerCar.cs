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
        // variables that control movement
        private float movement;
        private float acceleration = 14000.0f;
        private float maxSpeed = 2000.0f;
        private float lateralSpeed = 200.0f;
        
        private const float MoveStickScale = 1.0f;

        // fuel related variables
        private float lastFuelUnitTime; // how many milliseconds elapsed since the last fuel unit was consumed
        private float fuelConsumption = 1000;// how many milliseconds until a fuel unit is consumed

        public static int DrawingOffset=20;

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

        public bool IsSpinning
        {
            get { return isSpinning; }
        }
        bool isSpinning;

        public int FuelRemaining
        {
            get { return fuelRemaining; }
        }
        int fuelRemaining;

        public int Health
        {
            get { return health; }
            set { health = value; }
        }
        int health;
        
        public Rectangle PhysicalBounds
        {
            get { return physicalBounds; }
            set { physicalBounds = value; }
        }
        Rectangle physicalBounds;
        
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
            fuelRemaining = level.StartFuel;
            LoadContent();
            Speed = 200;
            physicalBounds = new Rectangle(15, 3, 33, 57);
            lastFuelUnitTime = 0;
            isSpinning = false;
            isAlive = true;
        }

        public void LoadContent()
        {
            // texture
            texture=level.Content.Load<Texture2D>("Sprites/players_car");
        }

        public void Update(GameTime gameTime)
        {
            GetInput();

            ApplyPhysics(gameTime);

            // fuel consumption
            if (lastFuelUnitTime >= fuelConsumption)
            {
                fuelRemaining--;
                lastFuelUnitTime = 0.0f;
            }
            else lastFuelUnitTime += gameTime.ElapsedGameTime.Milliseconds;

            movement = 0.0f;
        }

        public void ApplyPhysics(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 previousPosition = Position;

            position.X += movement * lateralSpeed * elapsed;
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
                keyboardState.IsKeyDown(Keys.Left))
            {
                movement = -1.0f;
            }
            else if (gamePadState.IsButtonDown(Buttons.DPadRight) ||
                     keyboardState.IsKeyDown(Keys.Right))
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
            if ( (position.X + (texture.Width - physicalBounds.Width)/2 ) < level.roadX1)
            {
                position.X = level.roadX1 - (texture.Width - physicalBounds.Width) / 2;
            }

            // collision with right wall ( road border ) 
            else if(( position.X + (texture.Width - physicalBounds.Width)/2 + physicalBounds.Width ) > level.roadX2 )
            {
                position.X = level.roadX2 - ((texture.Width - physicalBounds.Width)/2 + physicalBounds.Width);
            }

            // collision with other cars
            foreach (AICar car in level.AICars)
            {
                float xdiff = car.position.X - position.X;
                float ydiff = position.Y - car.position.Y;
                if (ydiff < car.PhysicalBounds.Height)
                {
                    System.Windows.Forms.MessageBox.Show("AI Car pos:    " + car.position.ToString() +
                                                       "\nPlayer pos:    " + position.ToString()); 
                    // possible collision
                }
            }
        }


        public void Reset(Vector2 position)
        {
            Position = position;
            Velocity = Vector2.Zero;
            isAlive = true;
        }

        internal void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 drawPosition = new Vector2(position.X + physicalBounds.Left, position.Y - level.startY - DrawingOffset - physicalBounds.Top);

            // draw the player's car
            spriteBatch.Draw(texture, drawPosition, Color.White);
            
            if (level.game.showDebugInfo)
                spriteBatch.DrawString(level.debufInfoFont,
                      "Pos:  " + position.X.ToString() + ", " + position.Y.ToString() +
                    "\nDraw: " + drawPosition.X.ToString() + ", " + drawPosition.Y.ToString(),
                    new Vector2(
                        drawPosition.X + physicalBounds.Width + 20,
                        drawPosition.Y),
                    Color.LightCyan
                    );
           
        }
    }
}
