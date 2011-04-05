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
        public int OilLeakProbability;
        public int RoadBlockProbability;
        public int TruckProbability;

        public bool ReachedFinish;

        public int screenWidth;
        public int screenHeight;
        int playerCarDrawingOffset = 20;
        int timeToCheckNewCars = 3;//seconds
        int timeSinceLastCheckNewCars = 0;

        // portion of the road currently being rendered
        public float startY;
        public float endY;

        public List<AICar> AICars = new List<AICar>();
        
        Rectangle RoadRect;
        // size of road texture
        int roadTileWidth = 100;
        int roadTileHeight = 100;

        private Random random = new Random(354668); // Arbitrary, but constant seed

        // textures
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

        public Level(IServiceProvider serviceProvider, int level_number, int windowWidth, int windowHeight)
        {
            content = new ContentManager(serviceProvider, "Content");

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
            OilLeakProbability = levelData.OilLeakProbability;
            RoadBlockProbability = levelData.RoadBlockProbability;
            TruckProbability = levelData.TruckProbability;

            LoadContent();
            playerCar = new PlayerCar(this,new Vector2(200,Length));
            ReachedFinish = false;
        }

        public void Update(GameTime gameTime)
        {
            startY = playerCar.Position.Y - (screenHeight - (playerCar.BoundingBox.Height + playerCarDrawingOffset));
            endY = startY + screenHeight;

            // new cars ?
            int k = random.Next(1, 100);
            if (k <= CarProbability && timeSinceLastCheckNewCars >= timeToCheckNewCars*1000)
            {
                AICars.Add(new AICar(this, new Vector2(200, startY - screenHeight - 50), 100));
                //global::System.Windows.Forms.MessageBox.Show(k.ToString() + "\nCreated a car at " + ((AICar)AICars.ElementAt(0)).position.ToString() +
                //    "\nTotal cars: " + AICars.Count +
                //    "\nstartY: " + startY);
                timeSinceLastCheckNewCars = 0;
            }
            else
                timeSinceLastCheckNewCars += gameTime.ElapsedGameTime.Milliseconds;


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
                if (playerCar.isSpinning && 
                    (playerCar.Position.X + playerCar.BoundingBox.X) < NumberOfLanes*roadTileWidth
                    )
                    OnPlayerKilled();

                // eliminate cars not in view and below
                for (int i = 0; i < AICars.Count; i++)
                {
                    AICar car = AICars.ElementAt(i);
                    if (car.position.Y >= (startY + car.boundingBox.Height) && car.position.Y <= endY)
                        AICars.Remove(car);
                }

                foreach (AICar car in AICars)
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
        }


        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, int windowWidth, int windowHeight,GraphicsDevice GraphicsDevice)
        {
            // draw road texture
            int road_width = NumberOfLanes * roadTileWidth;
            Rectangle destRect = new Rectangle( (windowWidth-road_width)/2-0, 0, road_width, windowHeight);

            // draw the tiles
            for (int i = 0; i < destRect.Height / roadTileHeight+1; i++)
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

            // draw other cars
            foreach (AICar car in AICars)
            {
                
                car.Draw(spriteBatch,new Vector2(car.position.X,endY-car.position.Y));
            }
            
            playerCar.Draw(gameTime, spriteBatch, new Vector2(playerCar.Position.X, 500));
        }
    }
}
