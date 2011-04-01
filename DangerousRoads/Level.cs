using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using LevelDataLibrary;
using Microsoft.Xna.Framework;

namespace DangerousRoads
{
    class Level
    {

        public String Name;
        public int NumberOfLanes;
        public int Length;
        public int Speed;
        public int CarProbability;
        public int CarSwitchLanesProbability;
        public int OilLeakProbability;
        public int RoadBlockProbability;
        public int TruckProbability;
                                   
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
        
        PlayerCar playerCar;

        // Level content.        
        public ContentManager Content
        {
            get { return content; }
        }
        ContentManager content;

        public Level(IServiceProvider serviceProvider, int level_number)
        {
            content = new ContentManager(serviceProvider, "Content");
            LevelData levelData = content.Load<LevelData>( String.Format("Levels/Level_{0}", level_number));
            
            Name = levelData.Name;
            NumberOfLanes = levelData.NumberOfLanes;
            Length = levelData.Length;
            Speed = levelData.Speed;
            CarProbability = levelData.CarProbability;
            CarSwitchLanesProbability = levelData.CarSwitchLanesProbability;
            OilLeakProbability = levelData.OilLeakProbability;
            RoadBlockProbability = levelData.RoadBlockProbability;
            TruckProbability = levelData.TruckProbability;

            LoadContent();
            playerCar = new PlayerCar(this,new Vector2(200,400));
        }

        public void LoadContent()
        {
            roadTexture = content.Load<Texture2D>("Sprites/road1");
        }


        public void Draw(GameTime gameTime, int windowWidth, int windowHeight,GraphicsDevice GraphicsDevice)
        {
            // draw road texture
            int road_widht = NumberOfLanes * roadTileWidth;
            Rectangle destRect = new Rectangle( (windowWidth-road_widht)/2, 0, road_widht, windowHeight);
            
            SpriteBatch roadTiledSprite = new SpriteBatch(GraphicsDevice);
            roadTiledSprite.Begin(SpriteBlendMode.AlphaBlend,
               SpriteSortMode.Immediate, SaveStateMode.None);
            GraphicsDevice.SamplerStates[0].AddressU = TextureAddressMode.Wrap;
            GraphicsDevice.SamplerStates[0].AddressV = TextureAddressMode.Wrap;

            roadTiledSprite.Draw(
                roadTexture,
                new Vector2((windowWidth-road_widht)/2,0),
                destRect,
                Color.White,
                0,
                Vector2.Zero,
                1.0f,
                SpriteEffects.None,
                1.0f);

            roadTiledSprite.End();


            // draw road borders


            SpriteBatch spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
            playerCar.Draw(gameTime,spriteBatch);
            spriteBatch.End();
            // draw other cars

        }
    }
}
