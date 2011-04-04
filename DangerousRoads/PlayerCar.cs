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
            Speed = 100;
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



            HandleCollisions();



        }

        private void GetInput()
        {
            // Get input state.
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState keyboardState = Keyboard.GetState();

            // Get analog horizontal movement.
            movement = gamePadState.ThumbSticks.Left.X * MoveStickScale;

            // If any digital horizontal movement input is found, override the analog movement.
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

        }

        public void HandleCollisions()
        {


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
           
        }
    }
}
