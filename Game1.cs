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

namespace GJ20215_BecauseZombies
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        WorldManager Manager;

        Player p;
        private Texture2D PlayerTexture;
        
        private List<Player> CurrentPlayers;
        private List<Entity> CurrentEntities;
        private Texture2D BulletTexture;
        private List<Texture2D> GameTextures;
        private List<SoundEffect> GameSoundFx;
        private Texture2D EnemyTexture;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            graphics.IsFullScreen = true;

            Content.RootDirectory = "Content";
            
            // add test player
            this.p = new Player();
            this.CurrentPlayers = new List<Player>();
            this.CurrentPlayers.Add(p);

            // test, default entities
            this.CurrentEntities = new List<Entity>();
            this.GameTextures = new List<Texture2D>();
            this.GameSoundFx = new List<SoundEffect>();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

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

            // TODO: use this.Content to load your game content here
            this.PlayerTexture = Content.Load<Texture2D>("32x32_10x10_Grid_Player_Placeholder");
            this.BulletTexture = Content.Load<Texture2D>("32x32_10x10_Grid_Bullet_Placeholder");
            this.EnemyTexture = Content.Load<Texture2D>("32x32_10x10_Grid_Zombie_Placeholder");

            this.GameTextures.Add(this.PlayerTexture); this.GameTextures[0].Name = "32x32_10x10_Grid_Player_Placeholder";// 0
            this.GameTextures.Add(this.BulletTexture); this.GameTextures[1].Name = "32x32_10x10_Grid_Bullet_Placeholder";// 1 
            this.GameTextures.Add(this.EnemyTexture); this.GameTextures[2].Name = "32x32_10x10_Grid_Zombie_Placeholder";// 2 
            this.GameTextures.Add(Content.Load<Texture2D>("32x32_10x10_Grid_HUD_Enemy")); this.GameTextures[3].Name = "32x32_10x10_Grid_HUD_Enemy"; // 3 
            this.GameTextures.Add(Content.Load<Texture2D>("32x32_10x10_Grid_HUD_Player")); this.GameTextures[4].Name = "32x32_10x10_Grid_HUD_Player"; // 4
            this.GameTextures.Add(Content.Load<Texture2D>("32x32_10x10_Grid_HUD_Grid")); this.GameTextures[5].Name = "32x32_10x10_Grid_HUD_Grid"; // 5
            this.GameTextures.Add(Content.Load<Texture2D>("32x32_10x10_GrassTile")); this.GameTextures[6].Name = "32x32_10x10_GrassTile";    // 6
            this.GameTextures.Add(Content.Load<Texture2D>("32x32_10x10_ShotgunAmmo")); this.GameTextures[7].Name = "32x32_10x10_ShotgunAmmo";   // 7
            this.GameTextures.Add(Content.Load<Texture2D>("32x32_10x10_HealthBar")); this.GameTextures[8].Name = "32x32_10x10_HealthBar"; // 8 
            this.GameTextures.Add(Content.Load<Texture2D>("32x32_10x10_HealthBar_RedSliver")); this.GameTextures[9].Name = "32x32_10x10_HealthBar_RedSliver"; // 9
            this.GameTextures.Add(Content.Load<Texture2D>("32x32_10x10_HealthBar_YellowSliver")); this.GameTextures[10].Name = "32x32_10x10_HealthBar_YellowSliver"; // 1t0
            this.GameTextures.Add(Content.Load<Texture2D>("32x32_10x10_HealthBar_BlueSliver")); this.GameTextures[11].Name = "32x32_10x10_HealthBar_BlueSliver"; // 11
            this.GameTextures.Add(Content.Load<Texture2D>("32x32_10x10_WeaponPlaceholder")); this.GameTextures[12].Name = "32x32_10x10_WeaponPlaceholder";// 12

            this.GameTextures.Add(Content.Load<Texture2D>("32x32_10x10_Grid_Player_Placeholder_FL")); this.GameTextures[13].Name = "32x32_10x10_Grid_Player_Placeholder_FL";// 13
            this.GameTextures.Add(Content.Load<Texture2D>("32x32_10x10_Grid_Player_Placeholder_FR")); this.GameTextures[14].Name = "32x32_10x10_Grid_Player_Placeholder_FR";// 14
            this.GameTextures.Add(Content.Load<Texture2D>("32x32_10x10_Grid_Player_Placeholder_FF")); this.GameTextures[15].Name = "32x32_10x10_Grid_Player_Placeholder_FF";// 15

            this.GameTextures.Add(Content.Load<Texture2D>("32x32_10x10_Grid_Zombie_Placeholder_FB")); this.GameTextures[16].Name = "32x32_10x10_Grid_Zombie_Placeholder_FB";// 16
            this.GameTextures.Add(Content.Load<Texture2D>("32x32_10x10_Grid_Zombie_Placeholder_FL")); this.GameTextures[17].Name = "32x32_10x10_Grid_Zombie_Placeholder_FL";// 17
            this.GameTextures.Add(Content.Load<Texture2D>("32x32_10x10_Grid_Zombie_Placeholder_FR")); this.GameTextures[18].Name = "32x32_10x10_Grid_Zombie_Placeholder_FR";// 18

            int sfxIndex = 0;
            // THANKS TO MR. WILLIAM ROUSE ( https://soundcloud.com/suturesounds ) for all his assistance at the DC GGJ 2015 Woot!
            string[] SoundFxFilenames = { "gunshot", "Impact", "pain0", "pain1", "pain2", "pain3", "pain4", "pain5", "pain6", "pain7", "pain8", "pain9", "pain9", "pain10", "pain11", "pain12", "pickup", "reload", "respawn", "walking", "zombiedeath1", "zombiedeath2", "zombiepain1", "zombiepain2", "zombiepain3", "zombiepain4" };
            foreach (string Filename in SoundFxFilenames)
            {
                this.GameSoundFx.Add(Content.Load<SoundEffect>(Filename));
                this.GameSoundFx[sfxIndex].Name = Filename;
                sfxIndex++;
            }

            this.CurrentPlayers[0].Texture = this.PlayerTexture;

            this.Manager = new WorldManager(spriteBatch, this.CurrentPlayers, this.CurrentEntities, this.GameTextures, this.GameSoundFx);
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
            //this.Manager.ProcessTick(gameTime, Keyboard.GetState());
            //this.Manager.ProcessTick(gameTime, GamePad.GetState(PlayerIndex.One));

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
            this.Manager.ProcessTick(gameTime, Keyboard.GetState());
            this.Manager.DrawScene();

            base.Draw(gameTime);
        }
    }
}
