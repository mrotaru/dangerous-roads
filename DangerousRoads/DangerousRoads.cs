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
        float speed = 100;
        int lanes = 8;

        Level level;
        int currentLevel;
        int totalLevels;

        // global resources
        SpriteFont hudFont;
        Texture2D players_car;
        Texture2D car1, car2, car3;

        private const int TargetFrameRate = 60;
        private const int BackBufferWidth = 600;
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
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // fonts
            hudFont = Content.Load<SpriteFont>("gui_font");
            
            // textures
            players_car = Content.Load<Texture2D>("Sprites/players_car");
            car1 = Content.Load<Texture2D>("Sprites/ai_car_1");
        }

        private void LoadNextLevel()
        {
            if (currentLevel == totalLevels)
                return;
            else
            {
                // 'Level complete' screen, if != 1

                currentLevel++;
                level = new Level(Services, currentLevel);

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

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkGray);

            // TODO: Add your drawing code here

            // drawing code
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);

            level.Draw(gameTime, spriteBatch);

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

        }
    }
}
