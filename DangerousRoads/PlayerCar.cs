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
        private float mass;

        // physics
        private float enginePower;
        private Vector2 F;
        private Vector2 Ftraction;
        private Vector2 Fdrag;
        private Vector2 Frr;
        private Vector2 u;
        private Vector2 Fbrake;
        private float Cdrag;
                
        private const float MoveStickScale = 1.0f;

        // fuel related variables
        private float lastFuelUnitTime; // how many milliseconds elapsed since the last fuel unit was consumed
        private float fuelConsumption = 1000;// how many milliseconds until a fuel unit is consumed

        public static int DrawingOffset=20;
        public static int spinAngle;

        public int Width { get { return width; } }
        int width;   // = textureOffset.Width

        public int Height { get { return height; } }
        int height;  // = textureOffset.Height

        public float X { get { return position.X; } }
        public float Y { get { return position.Y; } }

        public Rectangle textureOffset;

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
            mass = 1000; // kg
            textureOffset = new Rectangle(15, 3, 33, 57);
            width = textureOffset.Width;
            height = textureOffset.Height;
            u.X = 0;
            u.Y = 1;
            Cdrag = 0.4257f;
            lastFuelUnitTime = 0;
            enginePower = 1000;
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
            Vector2 ip = position;

            GetInput();
            HandleCollisions(ip);

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

            //position.X += movement * lateralSpeed * elapsed;
            //position.Y -= Speed * elapsed;

            // compute physics
            Ftraction = u * enginePower;
            // velocity, in m/s
            Vector2 velm = velocity / DangerousRoads.pixelsPerMeter;
            float mspeed = (float)Math.Sqrt(velm.X * velm.X + velm.Y * velm.Y);
            Fdrag = -Cdrag * mspeed * velm;
            Frr = -level.RoadCrr * velm;
            F = Ftraction + Fdrag + Frr + Fbrake;
            Vector2 a = F / mass;
            velm = velm + elapsed*a;
            velocity = velm * DangerousRoads.pixelsPerMeter;
            position = position - elapsed * velocity;

            // display debug info
            if (level.game.showDebugInfo)
            {
                level.debugString =
                    "F_traction:   " + Ftraction.ToString() +
                  "\nmspeed:       " + mspeed.ToString() +
                  "\nF_drag:       " + Fdrag.ToString() +
                  "\nFrr:          " + Frr.ToString() +
                  "\nF:            " + F.ToString() +
                  "\nAcceleration: " + a.ToString() +
                  "\nVelocity:     " + velocity.ToString() +
                  "\nPosition:     " + position.ToString() +
                  "\nEngine power: " + enginePower.ToString() +
                  "\nF_brake:      " + Fbrake.ToString();
            }
            //System.Windows.Forms.MessageBox.Show(level.debugString);
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
                if (keyboardState.IsKeyDown(Keys.RightShift) || keyboardState.IsKeyDown(Keys.LeftShift))
                    position.X -= 1.0f;
                else
                    position.X -= 3.0f;
            }
            else if (gamePadState.IsButtonDown(Buttons.DPadRight) ||
                     keyboardState.IsKeyDown(Keys.Right))
            {
                if (keyboardState.IsKeyDown(Keys.RightShift) || keyboardState.IsKeyDown(Keys.LeftShift))
                    position.X += 1.0f;
                else
                    position.X += 3.0f;
            }
                        
            // acceleration/deceleration
            if (gamePadState.IsButtonDown(Buttons.DPadUp) || keyboardState.IsKeyDown(Keys.Up))
            {
                Fbrake = Vector2.Zero;
                if (fuelRemaining > 0) enginePower = 5000;
                else enginePower = 0;
            }
            else if (gamePadState.IsButtonDown(Buttons.DPadDown) || keyboardState.IsKeyDown(Keys.Down))
            {
                enginePower = 1000;
                if (velocity.Y > 0)
                    Fbrake = new Vector2(0, -5000);
                else
                    Fbrake = Vector2.Zero;
            }
            else
            {
                if (fuelRemaining > 0) enginePower = 100;
                else enginePower = 0;
                Fbrake = Vector2.Zero;
            }

        }

        public void HandleCollisions(Vector2 initialPosition)
        {
            // collision with left wall ( road border )
            if ( position.X < level.roadX1)
            {
                position.X = level.roadX1 + 1;
            }

            // collision with right wall ( road border ) 
            else if( position.X + width > level.roadX2 )
            {
                position.X = level.roadX2 - width + 1;
            }

            // collision with other cars
            foreach (Car car in level.AICars)
            {
                float diffx = ( position.X + width ) - car.position.X;
                float diffy = position.Y - ( car.position.Y + car.Height );
                if ( position.X + width >= car.position.X &&
                     position.X < car.position.X + car.Width &&
                     position.Y < car.position.Y + car.Height &&
                     position.Y + height >= car.position.Y
                    )
                {
                    position = initialPosition;
                    velocity = car.Velocity;
                    
                    {
                        level.debugString += ("\ndiff_X: " + diffx.ToString() +
                                          "\ndiff_Y: " + diffy.ToString());
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

        internal void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //Vector2 drawPosition = new Vector2(
            //    position.X + textureOffset.Left, 
            //    position.Y - level.startY - DrawingOffset - textureOffset.Top);
            
            Vector2 drawPosition = new Vector2(
                position.X - textureOffset.Left,
                (-1) * (level.startY - position.Y) + textureOffset.Top);
            
            // draw the player's car
            if(isSpinning)
                spriteBatch.Draw(texture, drawPosition, Color.White);
            else
                spriteBatch.Draw(texture, drawPosition, Color.White);

            if (level.game.showDebugInfo)
            {
                if (level.game.showDebugInfo)
                    spriteBatch.DrawString(level.debufInfoFont,
                          "Pos:   " + position.X.ToString() + ", " + position.Y.ToString() +
                        "\nDraw:  " + drawPosition.X.ToString() + ", " + drawPosition.Y.ToString() + 
                        "\nAngle: " + spinAngle,
                        new Vector2(
                            drawPosition.X + width + 10,
                            drawPosition.Y),
                        Color.LightCyan
                        );

                spriteBatch.DrawString(level.debufInfoFont,
                    "Physics\n=======\n" + level.debugString,
                    new Vector2(
                        10,
                        100),
                        Color.LightYellow);
            }
 
           
        }
    }
}
