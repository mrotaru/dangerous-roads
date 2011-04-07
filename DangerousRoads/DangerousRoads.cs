using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using System.IO;

namespace DangerousRoads
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class DangerousRoads : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;

        // global game variables
        public const int pixelsPerMeter = 40;
 
        Level level;
        int currentLevel;
        int totalLevels;
        bool paused;

        public bool showDebugInfo;

        // global resources
        SpriteFont hudFont;
        Texture2D players_car;
        Texture2D car1, car2, car3;

        private const int TargetFrameRate = 60;
        private const int BackBufferWidth = 800;
        private const int BackBufferHeight = 600;
        public int windowHeight;
        public int windowWidth;

        // keyboard
        private int msPaused;

        public DangerousRoads()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = BackBufferWidth;
            graphics.PreferredBackBufferHeight = BackBufferHeight;

            Content.RootDirectory = "Content";

            TargetElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / TargetFrameRate);

            paused = false;
            showDebugInfo = true;
            msPaused = 0;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            windowHeight = GraphicsDevice.Viewport.Height;
            windowWidth = GraphicsDevice.Viewport.Width;
            
            currentLevel = 0;
            totalLevels = 1;
            
            LoadNextLevel();
            
            base.Initialize();


        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            // fonts
            hudFont = Content.Load<SpriteFont>("gui_font");
                        
            // textures - loaded by the Level's 'LoadContent'
        }

        private void LoadNextLevel()
        {
            if (currentLevel == totalLevels)
                return;
            else
            {
                // 'Level complete' screen, if != 1

                currentLevel++;
                level = new Level(Services, this,currentLevel, windowWidth, windowHeight);

                // reset player position 
            }

        }


        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            HandleInput(gameTime);

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();


            if (!paused)
            {
                level.Update(gameTime);
                base.Update(gameTime);
            }
        }

        private void HandleInput(GameTime gameTime)
        {
            // Get input state.
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState keyboardState = Keyboard.GetState();

            if (gamePadState.IsButtonDown(Buttons.Start) ||
                keyboardState.IsKeyDown(Keys.Space))
            {
                if (paused && msPaused > 300)
                {
                    paused = false;
                    msPaused = 0;
                }
                else if (!paused && msPaused >300)
                {
                    paused = true;
                    msPaused = 0;
                }
            }
            msPaused += gameTime.ElapsedGameTime.Milliseconds;

            if( keyboardState.IsKeyDown(Keys.D))
                showDebugInfo=!showDebugInfo;

            if (keyboardState.IsKeyDown(Keys.Escape))
                this.Exit();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.ForestGreen);

            spriteBatch.Begin();

            level.Draw(gameTime, spriteBatch, windowWidth, windowHeight,GraphicsDevice);

            DrawHud();
            
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawHud()
        {
            Rectangle titleSafeArea = GraphicsDevice.Viewport.TitleSafeArea;
            Vector2 hudLocation = new Vector2(titleSafeArea.X, titleSafeArea.Y);
            Vector2 center = new Vector2(titleSafeArea.X + titleSafeArea.Width / 2.0f,
                                         titleSafeArea.Y + titleSafeArea.Height / 2.0f);

            spriteBatch.DrawString(hudFont, level.Name, new Vector2(1.0f, 1.0f), Color.Black);
            spriteBatch.DrawString(hudFont, "Fuel: " + level.playerCar.FuelRemaining.ToString(), new Vector2(1.0f, 20.0f), Color.Black);
            spriteBatch.DrawString(hudFont, "startY: " + level.startY + ", endY: " + level.endY, new Vector2(1.0f, 40.0f), Color.Black);
            if (level.AICars.Count() > 0)
            {
                spriteBatch.DrawString(hudFont, "1st car pos: \n" + level.AICars.ElementAt(0).position.ToString() + ", velocity: " +
                level.AICars.ElementAt(0).Velocity,
                    new Vector2(1.0f, 60.0f), Color.Black);
                spriteBatch.DrawString(hudFont, "Total cars: " + level.AICars.Count, new Vector2(1.0f, 100.0f), Color.Black);
            }
            spriteBatch.DrawString(hudFont, "Player's pos: " + level.playerCar.Position.ToString(), new Vector2(1.0f, 120.0f), Color.Black);
        }
    }
}
