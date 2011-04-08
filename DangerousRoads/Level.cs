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
        // these are loaded from XML files
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
        public int FuelProbability;
        public int RoadBlockProbability;
        public int TruckProbability;
        public float RoadCrr;

        public bool ReachedFinish;
        public string debugString;

        public DangerousRoads game;

        public SpriteFont debufInfoFont;
        public SpriteBatch _spriteBatch;

        public int screenWidth;
        public int screenHeight;
        int minMsBtwEval = 200; // minimum milliseconds between evaluating chances of creating new cars
        int msSinceEval = 0;
        int msSinceFuelEval = 0;

        // portion of the road currently being rendered
        public float startY;
        public float endY;
        public int roadWidth;
        public int roadX1;
        public int roadX2;

        public List<Car> AICars = new List<Car>();
        public List<Vector2> FuelItems = new List<Vector2>();
        
        Rectangle RoadRect;
        // size of road texture
        int roadTileWidth = 100;
        int roadTileHeight = 100;

        // size of road border
        int roadBorderWidth = 50;
        int roadBorderHeight = 100;

        
        TimeSpan t;
        int timestamp;
        private Random random;

        // textures
        public Texture2D SimpleTexture;
        public Texture2D sparkTexture;
        Texture2D roadFinish;
        Texture2D roadNoLines;
        Texture2D finishLine;
        public Texture2D fuelTexture;

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

            t = (DateTime.UtcNow - new DateTime(1970, 1, 1));
            timestamp = (int) t.TotalSeconds;
            random = new Random(timestamp); // Arbitrary, but constant seed


            LevelData levelData = content.Load<LevelData>( String.Format("Levels/Level_{0}", level_number));
            
            Name = levelData.Name;
            NumberOfLanes = levelData.NumberOfLanes;
            Length = levelData.Length*40*1000;
            Speed = levelData.Speed;
            StartFuel = levelData.StartFuel;
            CarProbability = levelData.CarProbability;
            CarSwitchLanesProbability = levelData.CarSwitchLanesProbability;
            CarMinSpeed = levelData.CarMinSpeed;
            CarMaxSpeed = levelData.CarMaxSpeed;
            OilLeakProbability = levelData.OilLeakProbability;
            FuelProbability = levelData.FuelProbability;
            RoadBlockProbability = levelData.RoadBlockProbability;
            TruckProbability = levelData.TruckProbability;
            RoadCrr = levelData.RoadCrr;

            LoadContent();
            
            playerCar = new PlayerCar(this,new Vector2(200,Length - PlayerCar.DrawingOffset - 70));
            ReachedFinish = false;
            roadWidth = NumberOfLanes * roadTileWidth;
            roadX1 = (windowWidth - roadWidth) / 2;
            roadX2 = roadX1 + roadWidth;
        }

        public void Update(GameTime gameTime)
        {
            startY = playerCar.Position.Y - (screenHeight - (playerCar.textureOffset.Height + PlayerCar.DrawingOffset));
            endY = startY + screenHeight;


            int r = (int)startY % 100;
            if (r <= 10 && msSinceEval > minMsBtwEval)
            {
                // new cars ?
                int k = random.Next(1, 100);
                if (k <= CarProbability)
                    CreateCar();
                // new items ?
                k = random.Next(1, 100);
                if (k <= FuelProbability)
                    CreateFuelItem();
                msSinceEval = 0;
            }
            else msSinceEval += gameTime.ElapsedGameTime.Milliseconds;

            if (playerCar.FuelRemaining <= 0)
                RoadCrr = 1000;

            if (playerCar.Position.Y <= 0)
            {
                // Animate the time being converted into points.
                //int seconds = (int)Math.Round(gameTime.ElapsedGameTime.TotalSeconds * 100.0f);
                //seconds = Math.Min(seconds, (int)Math.Ceiling(TimeRemaining.TotalSeconds));
                //timeRemaining -= TimeSpan.FromSeconds(seconds);
                //score += seconds * PointsPerSecond;
                game.LoadNextLevel();
            }
            else
            {
                playerCar.Update(gameTime);

                UpdateItems(gameTime);

                // Hitting the road border while the car is spinning is fatal
                if (playerCar.IsSpinning &&
                    (playerCar.Position.X + playerCar.PhysicalBounds.X) < NumberOfLanes * roadTileWidth
                    )
                    OnPlayerKilled();

                // eliminate cars not in view and below
                for (int i = 0; i < AICars.Count; i++)
                {
                    Car car = AICars.ElementAt(i);
                    if ((car.position.Y - endY) >= 300)
                        AICars.Remove(car);
                }

                foreach (Car car in AICars)
                {
                    car.Update(gameTime);
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
            
            AICars.Add(new Car(this, new Vector2(xpos, startY - 150), speed, String.Format("Sprites/ai_car_{0}", random.Next(1,5)),
                new Rectangle(15,3,33,57)));

            msSinceEval = 0;
        }

        private void CreateFuelItem()
        {
            // on which lane ?
            int road_width = NumberOfLanes * roadTileWidth;
            int ftWidth = fuelTexture.Width;
            int lane = random.Next(1, NumberOfLanes);
            int xpos = (screenWidth - road_width) / 2 + (lane - 1) * roadTileWidth + (roadTileWidth - ftWidth) / 2;
            FuelItems.Add(new Vector2(xpos, startY - 150));            
            msSinceEval = 0;
        }

        private void UpdateItems(GameTime gameTime)
        {
            //throw new NotImplementedException();
        }

        private void OnExitReached()
        {
            game.LoadNextLevel();
        }

        public void OnPlayerKilled()
        {
            System.Windows.Forms.MessageBox.Show("Your car is damaged beyound repair,\n you lose.");
            game.Exit();
        }

        public void OnPlayerOutOfFuel()
        {
            System.Windows.Forms.MessageBox.Show("Your run out of fuel,\n you lose.");
            game.Exit();
        }

        public void LoadContent()
        {
            roadTexture = content.Load<Texture2D>("Sprites/road1");
            roadNoLines = content.Load<Texture2D>("Sprites/road-no_lines");
            roadFinish = content.Load<Texture2D>("Sprites/road-finish");
            borderTexture = content.Load<Texture2D>("Sprites/road_border1");
            debufInfoFont = Content.Load<SpriteFont>("debugInfo");
            sparkTexture = Content.Load<Texture2D>("Sprites/spark");
            finishLine = Content.Load<Texture2D>("Sprites/finish_line");
            fuelTexture = Content.Load<Texture2D>("Sprites/fuel");
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
            for ( int i = -1; i < destRect.Height / roadTileHeight+1; i++)
                for (int j = 0; j < destRect.Width / roadTileWidth; j++)
                {             
                    Texture2D tex;
                    if (j == destRect.Width / roadTileHeight - 1 ) tex = roadNoLines;
                    else tex = roadTexture;

                    //if (startY < 0 && startY > -100 ) tex = roadFinish;
                    spriteBatch.Draw(tex,
                                     new Rectangle(
                                         destRect.X + j * roadTileWidth,  
                                         destRect.Y + i * roadTileHeight - (int)(startY % roadTileHeight), 
                                         roadTileWidth, 
                                         roadTileHeight
                                         ),
                                     Color.White);
                }

            // finish line
            if (startY <= 0 && endY >= 0)
                for (int j = 0; j < destRect.Width / finishLine.Width; j++)
                    spriteBatch.Draw(finishLine,
                        new Rectangle(
                            destRect.X + j * finishLine.Width,
                            screenHeight - (int)Math.Abs(endY),
                            finishLine.Width,
                            finishLine.Height
                            ),
                            Color.White);

            // draw road border
            for (int i = -1; i < windowHeight / roadBorderHeight + 1; i++)
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

            // draw items
            for (int i = 0; i < FuelItems.Count; i++)
            {
                Vector2 fi = FuelItems.ElementAt(i);
                if (fi.Y < endY && (startY - fi.Y) <= 40)
                {
                    Vector2 drawPosition = new Vector2(
                    fi.X - 5,
                    (-1) * ( startY - fi.Y) + 5);

                    spriteBatch.Draw(fuelTexture, drawPosition, Color.White);
                }
            }

            // draw other cars
            if (game.showDebugInfo)
                spriteBatch.DrawString(debufInfoFont, "AI Cars\n", new Vector2(10, screenHeight- 200), Color.LightCyan);
            for (int i = 0; i < AICars.Count; i++)
            {
                Car car = AICars.ElementAt(i);
                car.Draw(spriteBatch);

                // debug info
                if (game.showDebugInfo)
                {
                    spriteBatch.DrawString(debufInfoFont,
                        car.position.Y.ToString() + ", DfS: " + ( Length - car.position.Y ).ToString() + ", DfP: " + ( playerCar.Position.Y - car.position.Y + car.Height).ToString(),
                        new Vector2( 10, screenHeight - 180 + i*12),
                        Color.LightCyan);

                }
            }

            if (game.showDebugInfo)
            {
                spriteBatch.DrawString(debufInfoFont, startY.ToString(), new Vector2(windowWidth - 60,                10), Color.LightCyan);
                spriteBatch.DrawString(debufInfoFont, endY.ToString(),   new Vector2(windowWidth - 60, windowHeight - 20), Color.LightCyan);
            }
                        
            playerCar.Draw(gameTime, spriteBatch);
        }
    }
}
