﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using LevelDataLibrary;
using Microsoft.Xna.Framework;
using System.Timers;

namespace DangerousRoads
{
    class Level
    {

        public String Name;
        public int NumberOfLanes;
        public float Length;
        public int Speed;
        public int StartFuel;
        public int CarProbability;
        public int CarSwitchLanesProbability;
        public int CarMinSpeed;
        public int CarMaxSpeed;
        public int OilLeakProbability;
        public int RoadBlockProbability;
        public int TruckProbability;

        public bool ReachedFinish;

        public DangerousRoads game;

        public SpriteFont debufInfoFont;

        public int screenWidth;
        public int screenHeight;
        int minMsBtwEval = 200; // minimum milliseconds between evaluating chances of creating new cars
        int msSinceEval = 0;

        // portion of the road currently being rendered
        public float startY;
        public float endY;
        public int roadWidth;
        public int roadX1;
        public int roadX2;

        public List<Car> AICars = new List<Car>();
        
        Rectangle RoadRect;
        // size of road texture
        int roadTileWidth = 100;
        int roadTileHeight = 100;

        // size of road border
        int roadBorderWidth = 50;
        int roadBorderHeight = 100;

        private Random random = new Random(354668); // Arbitrary, but constant seed

        // textures
        public Texture2D SimpleTexture;
        public Texture2D BorderTexture
        {
            get { return borderTexture; }
            set { borderTexture = value; }
        }
        Texture2D borderTexture;
        
        public Texture2D RoadTexture
        {
            get { return roadTexture; }
            set { roadTexture = value; }
        }
        Texture2D roadTexture;
        
        public PlayerCar playerCar;

        // Level content.        
        public ContentManager Content
        {
            get { return content; }
        }
        ContentManager content;

        public Level(IServiceProvider serviceProvider, DangerousRoads _game, int level_number, int windowWidth, int windowHeight)
        {
            content = new ContentManager(serviceProvider, "Content");
            game = _game;
            screenWidth = windowWidth;
            screenHeight = windowHeight;

            LevelData levelData = content.Load<LevelData>( String.Format("Levels/Level_{0}", level_number));
            
            Name = levelData.Name;
            NumberOfLanes = levelData.NumberOfLanes;
            Length = levelData.Length*25*1000;
            Speed = levelData.Speed;
            StartFuel = levelData.StartFuel;
            CarProbability = levelData.CarProbability;
            CarSwitchLanesProbability = levelData.CarSwitchLanesProbability;
            CarMinSpeed = levelData.CarMinSpeed;
            CarMaxSpeed = levelData.CarMaxSpeed;
            OilLeakProbability = levelData.OilLeakProbability;
            RoadBlockProbability = levelData.RoadBlockProbability;
            TruckProbability = levelData.TruckProbability;

            LoadContent();
            
            playerCar = new PlayerCar(this,new Vector2(200,Length - PlayerCar.DrawingOffset - 70));
            ReachedFinish = false;
            roadWidth = NumberOfLanes * roadTileWidth;
            roadX1 = (windowWidth - roadWidth) / 2;
            roadX2 = roadX1 + roadWidth;
        }

        public void Update(GameTime gameTime)
        {
            startY = playerCar.Position.Y - (screenHeight - (playerCar.PhysicalBounds.Height + PlayerCar.DrawingOffset));
            endY = startY + screenHeight;

            // new cars ?
            int r = (int)startY % 100;
            
            if ( r <= 10 && msSinceEval > minMsBtwEval)
            {
                //System.Windows.Forms.MessageBox.Show(r.ToString() + " " + msSinceEval.ToString());
                int k = random.Next(1, 100);
                if (k <= CarProbability)
                    CreateCar();
                msSinceEval = 0;
            }
            else msSinceEval += gameTime.ElapsedGameTime.Milliseconds;
            
            // Pause while the player is dead or time is expired.
            if (!playerCar.IsAlive || playerCar.FuelRemaining == 0)
            {
                // Still want to perform physics on the player.
                playerCar.ApplyPhysics(gameTime);
            }
            else if (ReachedFinish)
            {
                // Animate the time being converted into points.
                //int seconds = (int)Math.Round(gameTime.ElapsedGameTime.TotalSeconds * 100.0f);
                //seconds = Math.Min(seconds, (int)Math.Ceiling(TimeRemaining.TotalSeconds));
                //timeRemaining -= TimeSpan.FromSeconds(seconds);
                //score += seconds * PointsPerSecond;
            }
            else
            {
                playerCar.Update(gameTime);


                UpdateItems(gameTime);

                // Hitting the road border while the car is spinning is fatal
                if (playerCar.IsSpinning && 
                    (playerCar.Position.X + playerCar.PhysicalBounds.X) < NumberOfLanes*roadTileWidth
                    )
                    OnPlayerKilled();

                // eliminate cars not in view and below
                for (int i = 0; i < AICars.Count; i++)
                {
                    Car car = AICars.ElementAt(i);
                    if ( (car.position.Y - endY ) >= 300 )
                        AICars.Remove(car);
                }

                foreach (Car car in AICars)
                {
                    car.Update(gameTime);
                }

                // The player has reached the exit if they are standing on the ground and
                // his bounding rectangle contains the center of the exit tile. They can only
                // exit when they have collected all of the gems.
                if (playerCar.IsAlive &&
                    playerCar.Position.Y >= Length)
                {
                    OnExitReached();
                }
            }
        }

        private void CreateCar()
        {
            // on which lane ?
            int road_width = NumberOfLanes * roadTileWidth;
            int carWidth = 33;
            int lane = random.Next(1, NumberOfLanes);
            int xpos = (screenWidth - road_width)/2 + (lane - 1) * roadTileWidth + (roadTileWidth - carWidth) / 2;
            
            // at which speed ?
            int speed = random.Next(CarMinSpeed, CarMaxSpeed);
            
            AICars.Add(new Car(this, new Vector2(xpos, startY - 150), speed, "Sprites/ai_car_1",
                new Rectangle(15,3,33,57)));

            msSinceEval = 0;
        }

        private void UpdateItems(GameTime gameTime)
        {
            //throw new NotImplementedException();
        }

        private void OnExitReached()
        {
            //throw new NotImplementedException();
        }

        private void OnPlayerKilled()
        {
        }

        public void LoadContent()
        {
            roadTexture = content.Load<Texture2D>("Sprites/road1");
            borderTexture = content.Load<Texture2D>("Sprites/road_border1");
            debufInfoFont = Content.Load<SpriteFont>("debugInfo");
        }


        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, int windowWidth, int windowHeight,GraphicsDevice GraphicsDevice)
        {
            int road_width = NumberOfLanes * roadTileWidth;

            Rectangle destRect = new Rectangle(
                    (windowWidth - road_width) / 2,
                    0,
                    road_width,
                    windowHeight);

            // draw road texture
            for ( int i = 0; i < destRect.Height / roadTileHeight+1; i++)
                for (int j = 0; j < destRect.Width / roadTileWidth; j++)
                {
                    spriteBatch.Draw(RoadTexture,
                                     new Rectangle(
                                         destRect.X + j * roadTileWidth,  
                                         destRect.Y + i * roadTileHeight - (int)(startY % roadTileHeight), 
                                         roadTileWidth, 
                                         roadTileHeight
                                         ),
                                     Color.White);
                }

            // draw road border
            for (int i = 0; i < windowHeight / roadBorderHeight + 1; i++)
            {
                // draw left border
                spriteBatch.Draw(borderTexture,
                 new Rectangle(
                     (windowWidth - road_width) / 2-roadBorderWidth/2,
                     i * roadBorderHeight - (int)(startY % roadBorderHeight),
                     roadBorderWidth,
                     roadBorderHeight
                     ),
                 Color.White);

                // draw right border
                spriteBatch.Draw(borderTexture,
                 new Rectangle(
                     (windowWidth - road_width) / 2 + road_width,
                     i * roadBorderHeight - (int)(startY % roadBorderHeight),
                     roadBorderWidth,
                     roadBorderHeight
                     ),
                 Color.White);
            }

            // draw other cars
            for (int i = 0; i < AICars.Count; i++)
            {
                Car car = AICars.ElementAt(i);
                car.Draw(spriteBatch);

                // debug info
                if (game.showDebugInfo)
                {
                    spriteBatch.DrawString(debufInfoFont,
                        car.position.Y.ToString() + ", DfS: " + ( Length - car.position.Y ).ToString() + ", DfP: " + ( playerCar.Position.Y - car.position.Y + car.Height).ToString(),
                        new Vector2( 5, screenHeight - 200 + i*12),
                        Color.LightCyan);
                }

            }
            
            playerCar.Draw(gameTime, spriteBatch);
        }
    }
}
