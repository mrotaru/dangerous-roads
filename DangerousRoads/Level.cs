using System;
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
        public int Length;
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

        // portion of the road currently being rendered
        float startY;
        float endY;

        Rectangle RoadRect;
        // size of road texture
        int roadTileWidth = 100;
        int roadTileHeight = 100;

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
            Length = levelData.Length;
            Speed = levelData.Speed;
            StartFuel = levelData.StartFuel;
            CarProbability = levelData.CarProbability;
            CarSwitchLanesProbability = levelData.CarSwitchLanesProbability;
            OilLeakProbability = levelData.OilLeakProbability;
            RoadBlockProbability = levelData.RoadBlockProbability;
            TruckProbability = levelData.TruckProbability;

            LoadContent();
            playerCar = new PlayerCar(this,new Vector2(200,400));
            ReachedFinish = false;
        }

        public void Update(GameTime gameTime)
        {
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
                    (playerCar.Position.X+playerCar.BoundingBox.X) < NumberOfLanes*roadTileWidth
                    )
                    OnPlayerKilled();

                UpdateAiCars(gameTime);

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

        private void UpdateAiCars(GameTime gameTime)
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
            for (int i = 0; i < destRect.Height / roadTileHeight; i++)
                for (int j = 0; j < destRect.Width / roadTileWidth; j++)
                    spriteBatch.Draw(RoadTexture,
                                     new Rectangle(destRect.X + j*roadTileWidth, destRect.Y + i*roadTileHeight,roadTileWidth,roadTileHeight),
                                     Color.White);


            playerCar.Draw(gameTime,spriteBatch);

            // draw other cars
            
        }
    }
}
