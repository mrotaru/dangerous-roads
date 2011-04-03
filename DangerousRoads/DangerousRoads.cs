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
        SpriteBatch spriteBatch;

        // global game variables
 
        Level level;
        int currentLevel;
        int totalLevels;

        // global resources
        SpriteFont hudFont;
        Texture2D players_car;
        Texture2D car1, car2, car3;

        private const int TargetFrameRate = 60;
        private const int BackBufferWidth = 800;
        private const int BackBufferHeight = 600;
        public int windowHeight;
        public int windowWidth;

        public DangerousRoads()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = BackBufferWidth;
            graphics.PreferredBackBufferHeight = BackBufferHeight;

            Content.RootDirectory = "Content";

            TargetElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / TargetFrameRate);

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
                level = new Level(Services, currentLevel, windowWidth, windowHeight);

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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            level.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GameTime startRender;

            GraphicsDevice.Clear(Color.ForestGreen);

            level.Draw(gameTime, windowWidth, windowHeight,GraphicsDevice);

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);

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

        }
    }
}
