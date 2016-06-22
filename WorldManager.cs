
/*
 * World Mangement stuffs [012415] Jto_daA
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace GJ20215_BecauseZombies
{
    class WorldManager
    {        
        #region Data Members 

        private SpriteBatch WorldRenderer;

        List<Entity> GameEntities;
        List<Player> ActivePlayers;
        List<Texture2D> GameTextures;
        GameTime TimeInSession;

        Camera WorldCam;
        GameGrid CurrentGrid;
        Hud GameHud;
        List<HealthBar> HealthBars;
        List<WeaponDisplay> WeaponDisplays;
        List<AmmoDisplay> AmmoDisplays;
        List<SoundEffect> GameSfx;

        #endregion 

        #region Construction 

        public WorldManager(SpriteBatch sb,List<Player> ps,List<Entity> es,List<Texture2D> ts,List<SoundEffect> sfxs)
        {
            this.WorldRenderer = sb;

            this.CurrentGrid = new GameGrid(new Texture2D[]{ts[6],ts[7]});
            this.HealthBars = new List<HealthBar>();
            this.WeaponDisplays = new List<WeaponDisplay>();
            this.AmmoDisplays = new List<AmmoDisplay>();

            this.GameEntities = new List<Entity>();
            this.ActivePlayers = new List<Player>();
            this.GameTextures = ts;
            this.GameSfx = sfxs;

            this.GameHud = new Hud(680, 350, 100, 100, this.CurrentGrid, this.GameEntities, this.WorldRenderer);
            this.GameHud.EnemyDot = this.GameTextures[3];
            this.GameHud.PlayerDot = this.GameTextures[4];
            this.GameHud.GridTexture = this.GameTextures[5];

            this.WorldCam = new Camera();

            foreach (Entity e in es)
                this.GameEntities.Add(e);

            int PlayerIndex = 0;
            foreach (Player p in ps)
            {
                this.ActivePlayers.Add(p);

                Texture2D BarTexture = ts[8];
                Texture2D YellowTexture = ts[9];
                Texture2D RedTexture = ts[10];
                Texture2D BlueTexture = ts[11];

                this.HealthBars.Add(new HealthBar(p, this.WorldRenderer, new Texture2D[] { BarTexture, YellowTexture, RedTexture, BlueTexture }));
                
                WeaponDisplay PX_WeaponDisplay = new WeaponDisplay();
                PX_WeaponDisplay.Texture = this.GameTextures[12];
                this.WeaponDisplays.Add(PX_WeaponDisplay);

                AmmoDisplay PX_AmmoDisplay = new AmmoDisplay();
                PX_AmmoDisplay.Texture = this.GameTextures[7];
                this.AmmoDisplays.Add(PX_AmmoDisplay);

                PlayerIndex++;
            }
        }

        #endregion 

        public void AddEntity(Entity e)
        {
            this.GameEntities.Add(e);
        }

        /// <summary>
        /// Draws a frame for the active session
        /// </summary>
        public void DrawScene()
        {            
            // Draw BG
            foreach (GameGridCell g in this.CurrentGrid.GameGridCells)
            {
                // draw tile
                DrawEntity(g,false);

                // draw inventory texture if it exists
                if (g.InventoryTexture != null)
                {
                    this.WorldRenderer.Begin(SpriteSortMode.BackToFront, null, null, null, null, null, this.WorldCam.TransformMatrix);

                    this.WorldRenderer.Draw(g.InventoryTexture,
                                            new Rectangle(g.Position.X + 4, g.Position.Y + 4, 16, 16),
                                            Color.White);

                    this.WorldRenderer.End();
                }
            }

            // Draw (active) Entities
            foreach (Entity e in this.GameEntities)
                DrawEntity(e,false);

            // Draw Players
            foreach (Player p in this.ActivePlayers)
                DrawPlayer(p);
            
            // Draw UI           
            this.GameHud.Draw(this.ActivePlayers,GetActiveEntType(typeof(Enemy)));
            foreach (HealthBar h in this.HealthBars)
                h.Draw();
            this.WorldRenderer.Begin();
            for (int pIndex = 0; pIndex < this.ActivePlayers.Count; pIndex++)
            {
                this.WorldRenderer.Draw(this.WeaponDisplays[pIndex].Texture,
                                        new Rectangle(this.WeaponDisplays[pIndex].Position.X,
                                                      this.WeaponDisplays[pIndex].Position.Y,
                                                      this.WeaponDisplays[pIndex].Dimensions.X,
                                                      this.WeaponDisplays[pIndex].Dimensions.Y), Color.White);

                this.WorldRenderer.Draw(this.AmmoDisplays[pIndex].Texture,
                                        new Rectangle(this.AmmoDisplays[pIndex].Position.X,
                                                      this.AmmoDisplays[pIndex].Position.Y,
                                                      this.AmmoDisplays[pIndex].Dimensions.X,
                                                      this.AmmoDisplays[pIndex].Dimensions.Y), Color.White);

            }
            this.WorldRenderer.End();
        }

        /// <summary>
        /// Draws player sprite
        /// </summary>
        /// <param name="p"></param>
        private void DrawPlayer(Player p)
        {
            this.WorldRenderer.Begin(SpriteSortMode.BackToFront, null, null, null, null, null, this.WorldCam.TransformMatrix);

            this.WorldRenderer.Draw(p.Texture, new Microsoft.Xna.Framework.Rectangle(p.Position.X, p.Position.Y, p.Dimensions.X, p.Dimensions.Y), Microsoft.Xna.Framework.Color.White);

            this.WorldRenderer.End();
        }

        /// <summary>
        /// Draws entity sprite
        /// </summary>
        /// <param name="e"></param>
        private void DrawEntity(Entity e,bool OverrideDimension)
        {
            int Width = (OverrideDimension) ? (e.Dimensions.X) : (32);
            int Height = (OverrideDimension) ? (e.Dimensions.Y) : (32);

            this.WorldRenderer.Begin(SpriteSortMode.BackToFront, null, null, null, null, null, this.WorldCam.TransformMatrix);

            //this.WorldRenderer.Draw(e.Texture,
            //                        new Microsoft.Xna.Framework.Rectangle(e.Position.X, e.Position.Y, Width, Height),
            //                        new Microsoft.Xna.Framework.Rectangle(32 * e.CurrentFrame + (1 * e.CurrentFrame),
            //                                                              32 * e.CurrentRow + (1 * e.CurrentRow),Width, Height),
            //                                                              Color.White);

            this.WorldRenderer.Draw(e.Texture,
                                    new Rectangle(e.Position.X, e.Position.Y, e.Dimensions.X, e.Dimensions.Y), Color.White);


            this.WorldRenderer.End();
        }

        /// <summary>
        /// Updates Player status according to the current keyboard input
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="keyboardState"></param>
        internal void ProcessTick(Microsoft.Xna.Framework.GameTime Gt, Microsoft.Xna.Framework.Input.KeyboardState CurrentKeyboardState)
        {
            #region Process User Input
            // basic movement
            // W - UP 
            // A - LEFT 
            // S - RIGHT
            // D - DOWN 
            // SPACE - SHOOT 
            // CTRL - SECONDAY WEAPON
            // ALT - INVENTORY CYCLE
            // ESC - EXIT
            // ?? - PAUSE... SOMEHOW
            // GROUP ONE : MOVEMENT ====================================================================================================
            if(CurrentKeyboardState.IsKeyDown(Keys.W))  // MOVING/SWITCHING TO UP =======================================================
            {
                this.ActivePlayers[0].Position.Y -= 2;
                this.WorldCam.Update(0, -2, 0);

                this.ActivePlayers[0].MoveupKeyWasPressed = true;

                if (!CurrentKeyboardState.IsKeyDown(Keys.Space))
                {
                    this.ActivePlayers[0].IsFacingUp = true;
                    this.ActivePlayers[0].Texture = GetTexture("Player_Placeholder");
                    this.ActivePlayers[0].IsFacingDown = this.ActivePlayers[0].IsFacingLeft = this.ActivePlayers[0].IsFacingRight = false;
                }
                
            }
            if (CurrentKeyboardState.IsKeyDown(Keys.S)) // MOVING/SWITCHING TO DOWN =========================================================
            {                
                this.ActivePlayers[0].Position.Y += 2;
                this.WorldCam.Update(0, 2, 0);

                this.ActivePlayers[0].MoveDownKeyWasPressed = true;

                if (!CurrentKeyboardState.IsKeyDown(Keys.Space))
                {
                    this.ActivePlayers[0].IsFacingDown = true;
                    this.ActivePlayers[0].Texture = GetTexture("Player_Placeholder_FF");
                    this.ActivePlayers[0].IsFacingUp = this.ActivePlayers[0].IsFacingLeft = this.ActivePlayers[0].IsFacingRight = false;
                }
            }
            if (CurrentKeyboardState.IsKeyDown(Keys.A)) // MOVING/SWITCHING TO LEFT ==============================================================
            {
                this.ActivePlayers[0].Position.X -= 2;
                this.WorldCam.Update(-2, 0, 0);

                this.ActivePlayers[0].MoveLeftKeyWasPressed = true;

                if (!CurrentKeyboardState.IsKeyDown(Keys.Space))
                {
                    this.ActivePlayers[0].IsFacingLeft = true;
                    this.ActivePlayers[0].Texture = GetTexture("Player_Placeholder_FL");
                    this.ActivePlayers[0].IsFacingRight = this.ActivePlayers[0].IsFacingDown = this.ActivePlayers[0].IsFacingUp = false;
                }
            }/* KEEP ONE INSTANCE OF THE SOURCE TO ALLOW FOR DIRECTION SWITCHING USING A DOUBLE-TAP. THIS WAS ABANDONED DUE TO AWKARDNESS
            else if (CurrentKeyboardState.IsKeyUp(Keys.A) && this.ActivePlayers[0].MoveLeftKeyWasPressed) // left release, allow for double-tap direction change
            {
                this.ActivePlayers[0].MoveLeftKeyWasPressed = false;

                if (!this.ActivePlayers[0].HasLeftSingleTap)
                {
                    this.ActivePlayers[0].HasLeftSingleTap = true;
                    this.ActivePlayers[0].LeftSingleTapTime = Gt.TotalGameTime.Seconds; // keep a track of the first time the key was pressed, so that it can be compared 
                }
                else
                {
                    int DeltaTimePressed = Gt.TotalGameTime.Seconds - this.ActivePlayers[0].LeftSingleTapTime;

                    if (DeltaTimePressed <= 1) // legit double table must occur less than 1 seconds after the fist one
                    {
                        this.ActivePlayers[0].IsFacingLeft = true;
                        this.ActivePlayers[0].IsFacingRight = this.ActivePlayers[0].IsFacingUp = this.ActivePlayers[0].IsFacingDown = false;
                    }

                    this.ActivePlayers[0].HasLeftSingleTap = false;                         
                }
            }*/
            // =========================================================================================================================================
            if (CurrentKeyboardState.IsKeyDown(Keys.D)) // MOVING/SWITCHING TO RIGHT
            {
                this.ActivePlayers[0].Position.X += 2;
                this.WorldCam.Update(2, 0, 0);

                this.ActivePlayers[0].MoveRightKeyWasPressed = true;

                if (!CurrentKeyboardState.IsKeyDown(Keys.Space))
                {
                    this.ActivePlayers[0].IsFacingRight = true;
                    this.ActivePlayers[0].Texture = GetTexture("Player_Placeholder_FR");
                    this.ActivePlayers[0].IsFacingLeft = this.ActivePlayers[0].IsFacingUp = this.ActivePlayers[0].IsFacingDown = false;
                }
            }
            // GROUP TWO : SHOOTING ============================================================
            if (CurrentKeyboardState.IsKeyDown(Keys.Space)) // shoot
             {
                int Vx = (this.ActivePlayers[0].IsFacingLeft || this.ActivePlayers[0].IsFacingRight) ? ((this.ActivePlayers[0].IsFacingLeft) ? (-5) : (5)) : (0); ;
                int Vy = (this.ActivePlayers[0].IsFacingUp || this.ActivePlayers[0].IsFacingDown) ? ((this.ActivePlayers[0].IsFacingUp)? (-5) : (5)) : (0);

                if ((DateTime.Now.Millisecond - this.ActivePlayers[0].TimeLastRoundFired) > 200) // weapon fire rate handling
                {
                    this.GameEntities.Add(new Projectile(ProjectileType.NormalBullet,
                                          this.ActivePlayers[0].Position.X,
                                          this.ActivePlayers[0].Position.Y,
                                          20,
                                          20,
                                          Vx,
                                          Vy,
                                          this.GameTextures[1]));

                    this.ActivePlayers[0].TimeLastRoundFired = DateTime.Now.Millisecond;

                    SoundEffect[] ShootSfxList = GetSoundEffect("gunshot");
                    ShootSfxList[0].Play();
                }
                else
                    this.ActivePlayers[0].TimeLastRoundFired = 0;
            }

            else if (CurrentKeyboardState.IsKeyDown(Keys.LeftControl)) // alt fire
            {

            }

            else if (CurrentKeyboardState.IsKeyDown(Keys.LeftAlt)) // cycle through inventory
            {

            }

            // GROUP THREE : ETC I/O ===========================================================
            if (CurrentKeyboardState.IsKeyDown(Keys.Escape))
            {

            }

            // CAMERA CONTROL ( FOR DEV ONLY )==================================================
            if (CurrentKeyboardState.IsKeyDown(Keys.Up))
            {
                this.WorldCam.Update(0, 1, 0);
            }
            if (CurrentKeyboardState.IsKeyDown(Keys.Down))
            {
                this.WorldCam.Update(0, -1, 0);
            }
            if (CurrentKeyboardState.IsKeyDown(Keys.Left))
            {
                this.WorldCam.Update(-1, 0, 0);
            }
            if (CurrentKeyboardState.IsKeyDown(Keys.Right))
            {
                this.WorldCam.Update(1, 0, 0);
            }
            if (CurrentKeyboardState.IsKeyDown(Keys.OemPlus))
            {
                this.WorldCam.Update(0, 0, 0.0005f);
            }
            if (CurrentKeyboardState.IsKeyDown(Keys.OemMinus))
            {
                this.WorldCam.Update(0, 0, -0.0005f);
            }

            #endregion

            #region Entity Processing

            // process all the active game entities =========================================================
            for(int i=0;i<this.GameEntities.Count;i++)
            {
                Entity e = this.GameEntities[i];

                if (e.Relevant)
                    e.Think(Gt);
                else
                    this.GameEntities.Remove(e);
            }

            #endregion 

            #region Collision Detection(s)

            // CHECK FOR COLLISIONS BETWEEN BULLETS AND ENEMIES
            List<Entity> Bullets = GetActiveEntType(typeof(Projectile));
            List<Entity> Enemies = GetActiveEntType(typeof(Enemy));
            
            for (int e = 0; e < Enemies.Count; e++)
            {
                // update each zombies target with the players current position
                Enemies[e].Target = this.ActivePlayers[0].Position;

                Enemy emy = (Enemy)Enemies[e];

                if (emy.IsFacingUp)
                    emy.Texture = GetTexture("Grid_Zombie_Placeholder");
                else if (emy.IsFacingDown)
                    emy.Texture = GetTexture("Grid_Zombie_Placeholder_FB");
                else if (emy.IsFacingLeft)
                    emy.Texture = GetTexture("Grid_Zombie_Placeholder_FL");
                else if (emy.IsFacingRight)
                    emy.Texture = GetTexture("Grid_Zombie_Placeholder_FR");


                if (Math.Abs(Enemies[e].Position.X - this.ActivePlayers[0].Position.X) <= this.ActivePlayers[0].Dimensions.X)
                {
                    if (Math.Abs(Enemies[e].Position.Y - this.ActivePlayers[0].Position.Y) <= this.ActivePlayers[0].Dimensions.Y)
                    {
                        this.ActivePlayers[0].Health--;

                        if (this.ActivePlayers[0].Health <= 0)
                        {
                            //this.ActivePlayers[0].Relevant = false;  // dead bub.... respawn and keep going

                            this.CurrentGrid = new GameGrid(new[] { this.GameTextures[6], this.GameTextures[7] });
                            this.ActivePlayers[0].Health = 100;
                        }
                    }
                }

                for (int b = 0; b < Bullets.Count; b++)
                {
                    Random SfxRandom = new Random(DateTime.Now.Millisecond);

                    // check for collision
                    if (Math.Abs(Bullets[b].Position.X - Enemies[e].Position.X) < Bullets[b].Dimensions.X)
                    {
                        if (Math.Abs(Bullets[b].Position.Y - Enemies[e].Position.Y) < Bullets[b].Dimensions.Y)
                        {
                            // YOU DONE COLLIDED!!!!
                            // ADD SCORING HERE
                            Bullets[b].Relevant = Enemies[e].Relevant = false;
                            SoundEffect[] SfxPainList = GetSoundEffect("zombiepain");

                            SfxPainList[SfxRandom.Next(0, SfxPainList.Length)].Play(0.8f, 0.0f, 0.0f);

                            SoundEffect[] SfxHitList = GetSoundEffect("Impact");
                            SfxHitList[0].Play();
                        }
                    }
                }
            }

            #region // CHECK FOR PLAYER(s) COLLISION TO CURRENT GAME GRID TILE ( FOR INVENTORY PICKUPS ETC. ) =============================
            for (int p = 0; p < this.ActivePlayers.Count; p++)
            {
                Player px = this.ActivePlayers[p];
                int PlayerGridRowPosition = px.Position.X / 32;
                int PlayerGridColPosition = px.Position.Y / 32;

                // Keep player within the defined area.
                if (PlayerGridRowPosition == 0)
                {
                    this.ActivePlayers[0].Position.X += 2;
                    this.WorldCam.Update(2, 0, 0);
                }
                if(PlayerGridColPosition == 0)
                {
                    this.ActivePlayers[0].Position.Y += 2;
                    this.WorldCam.Update(0,2,0);                        
                }
                if (PlayerGridRowPosition == (this.CurrentGrid.GridHeight-1))
                {
                    this.ActivePlayers[0].Position.X -= 2;
                    this.WorldCam.Update(-2, 0,0);
                }

                if (PlayerGridColPosition == (this.CurrentGrid.GridWidth-1))
                {
                    this.ActivePlayers[0].Position.Y -= 2;
                    this.WorldCam.Update(0,-2,0);
                }

                GameGridCell ActiveGameGridCell =  this.CurrentGrid.GameGridCells[PlayerGridRowPosition, PlayerGridColPosition];

                switch (ActiveGameGridCell.Inventory)
                {
                    case "SGN_AMMO":
                        {
                            if (px.CurrentInventory.ShotgunAmmoPackCount < px.CurrentInventory.MaxShotgunPackCount)
                            {
                                px.CurrentInventory.ShotgunAmmoPackCount++;
                                
                                ActiveGameGridCell.Inventory = string.Empty;
                                ActiveGameGridCell.InventoryTexture = null;

                                GetSoundEffect("pickup")[0].Play();
                            }

                            break;
                        }
                    case "MGN_AMMO":
                        {
                            if (px.CurrentInventory.MachineGunClipCount < px.CurrentInventory.MaxClipCount)
                            {
                                px.CurrentInventory.MachineGunClipCount++;

                                ActiveGameGridCell.Inventory = string.Empty;
                                ActiveGameGridCell.InventoryTexture = null;

                                GetSoundEffect("pickup")[0].Play();
                            }

                            break;
                        }
                    case "SGN":
                        {
                            if (px.CurrentInventory.ShotgunCount == 0)
                            {
                                px.CurrentInventory.ShotgunCount = 1;

                                ActiveGameGridCell.Inventory = string.Empty;
                                ActiveGameGridCell.InventoryTexture = null;

                                GetSoundEffect("pickup")[0].Play();
                            }
                            else
                            {
                                if (px.CurrentInventory.ShotgunAmmoPackCount < px.CurrentInventory.MaxShotgunPackCount)
                                {
                                    px.CurrentInventory.ShotgunAmmoPackCount++;

                                    ActiveGameGridCell.Inventory = string.Empty;
                                    ActiveGameGridCell.InventoryTexture = null;
                                }
                            }                                

                            break;
                        }
                    case "MGN":
                        {
                            if (px.CurrentInventory.MachineGunCount == 0)
                            {
                                px.CurrentInventory.MachineGunCount = 1;

                                ActiveGameGridCell.Inventory = string.Empty;
                                ActiveGameGridCell.InventoryTexture = null;

                                GetSoundEffect("pickup")[0].Play();
                            }
                            else
                            {
                                if (px.CurrentInventory.MachineGunClipCount < px.CurrentInventory.MaxClipCount)
                                {
                                    px.CurrentInventory.MachineGunClipCount++;

                                    ActiveGameGridCell.Inventory = string.Empty;
                                    ActiveGameGridCell.InventoryTexture = null;
                                }
                            }
                            break;
                        }
                    case "HTH":
                        {
                            //this.GameGridCells[gr, gc].InventoryTexture = texts[1];
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }

                
            }

            #endregion

            #endregion

            #region Enemy Spawning

            if (Gt.TotalGameTime.Seconds % 5 == 0)
                this.GameEntities.Add(new Enemy(EnemyType.RunOfTheMillZombieGuy, 0, 0, 0, 0, this.CurrentGrid.GameGridCells.Length, this.CurrentGrid.GameGridCells.Length, this.GameTextures[2]));

            #endregion

            #region Time Maintenance 

            this.TimeInSession = Gt;

            #endregion

            // health toggle ( DEVELOPMENT ONLY!!!! )
            //if (this.ActivePlayers[0].Health > 0)
            //    this.ActivePlayers[0].Health--;
            //else
            //    this.ActivePlayers[0].Health = 100;            
        }

        /// <summary>
        /// Returns a texture containing the specified filename fragment
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private Texture2D GetTexture(string FilenameFragment)
        {
            Texture2D SearchTexture = null;

            foreach (Texture2D t in this.GameTextures)
            {
                if (t.Name.Contains(FilenameFragment))
                {
                    SearchTexture = t;
                    break;
                }
            }

            return SearchTexture;
        }

        /// <summary>
        /// Returns a list of all sound effects with a shared name fragment ( to return a full set of randomized, similary named sfx 
        /// /// </summary>
        /// <param name="NameFragment"></param>
        /// <returns></returns>
        private SoundEffect[] GetSoundEffect(string NameFragment)
        {
            List<SoundEffect> SharedSfxList = new List<SoundEffect>();

            for (int sfxIndex = 0; sfxIndex < this.GameSfx.Count; sfxIndex++)
            {
                if (this.GameSfx[sfxIndex].Name.Contains(NameFragment))
                    SharedSfxList.Add(this.GameSfx[sfxIndex]);
            }

            return SharedSfxList.ToArray();
        }

        /// <summary>
        /// Returns list the specified type (subset) from within the compiled,"living" list
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private List<Entity> GetActiveEntType(Type type)
        {
            List<Entity> EntTypeList = new List<Entity>();

            foreach (Entity e in this.GameEntities)
            {
                if (e.GetType() == type)
                    EntTypeList.Add(e);
            }

            return EntTypeList;
        }
        /// <summary>
        /// Updates player IO status according to current gamepad input
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="gamePadState"></param>
        internal void ProcessTick(Microsoft.Xna.Framework.GameTime gameTime, Microsoft.Xna.Framework.Input.GamePadState gamePadState)
        {
            
        }
    }

    class Camera
    {
        #region Data Members

        private Vect2D Position, HighWithstraints, LowWithstraints;
        private float Zoom;
        public Matrix TransformMatrix;
        private Matrix TranslateMatrix, ScalingMatrix;

        #endregion 

        #region Construction 

        public Camera()
        {
            this.Position.X = 0;
            this.Position.Y = 0;

            this.LowWithstraints.X = 0;
            this.LowWithstraints.Y = 0;

            this.HighWithstraints.X = 0;
            this.HighWithstraints.Y = 0;

            this.Zoom = 1.0f;

            this.TranslateMatrix = Matrix.Identity;
            this.ScalingMatrix = Matrix.Identity;
            this.TransformMatrix = Matrix.Identity;
        }

        #endregion

        /// <summary>
        /// Allows for updating to the cameras vertical and lateral movement, as well as zoom
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        public void Update(int dx, int dy,float dz)
        {
            this.Position.X += dx;
            this.Position.Y += dy;

            this.Zoom += dz; // scaling constant

            this.TranslateMatrix = Matrix.CreateTranslation(-this.Position.X, -this.Position.Y, 0);
            this.ScalingMatrix = Matrix.CreateScale(this.Zoom);

            this.TransformMatrix = this.TranslateMatrix * this.ScalingMatrix;
        }
    }

    class GameGridCell : Entity
    {
        #region Data Members

        public bool HasBeenVisited;

        public Texture2D InventoryTexture;

        private string Enemy;
        public string Inventory;
        private string Trigger;
        
        private bool PlayerIsOnCell;

        #endregion

        #region Construction 

        public GameGridCell() : base(0,0,32,32)
        {

        }

        public GameGridCell(int r, int c,string e, string i, string t) : base(32*r,32*c,32,32)
        {            
            this.Enemy = e;
            this.Inventory = i;
            this.Trigger = t;
        }

        

        #endregion
    }

    class GameGrid
    {
        #region Data Members

        public GameGridCell[,] GameGridCells;
        public string[,] EnemyCells;
        public string[,] InventoryCells;
        public string[,] TriggerCells;
        public string[,] TileCells;

        private int InventoryTypeCount;
        private int EnemyTypeCount;
        private int TriggerTypeCount; // effectively BOOLEAN

        public int GridWidth, GridHeight;

        // strings to be used to encode grid cell values ( NOTE : THESE WILL EXIST IN PARALELL )
        private string[] InventoryStrings = { "SGN_AMMO", string.Empty, string.Empty, string.Empty, "MGN_AMMO", string.Empty, string.Empty, "SGN", string.Empty, string.Empty, "MGN", "HTH" };
        private string[] EnemyStrings = { "REG", string.Empty, string.Empty, "FLOCK", "BEZERK", string.Empty, string.Empty, string.Empty };
        private string[] TriggerStrings = { "0", "1", string.Empty, string.Empty, string.Empty, string.Empty };
        private Texture2D[] GridTextures;

        #endregion

        #region Construction

        /// <summary>
        /// Creates a randomized world
        /// </summary>
        public GameGrid(Texture2D[] texts) 
        {
            int w = new Random(DateTime.Now.Millisecond).Next(30, 50);
            int h = new Random(DateTime.Now.Millisecond).Next(30, 100);

            this.GridWidth = w;
            this.GridHeight = h;

            this.GridTextures = texts;

            this.InventoryTypeCount = InventoryStrings.Length;
            this.EnemyTypeCount = EnemyStrings.Length;
            this.TriggerTypeCount = TriggerStrings.Length;

            this.GameGridCells = new GameGridCell[h, w];
            this.EnemyCells = new string[h, w];
            this.InventoryCells = new string[h, w];
            this.TriggerCells = new string[h, w];
            this.TileCells = new string[h, w];

            // Build a randomized level by encoding the occurence, or entity type based on randomized assignement
            for (int gr = 0; gr < h; gr++)
            {
                Random PassRandom = new Random(DateTime.Now.Millisecond);
                
                for (int gc = 0; gc < w; gc++)
                {
                    int InventoryRandomIndex = PassRandom.Next(InventoryTypeCount - 1);
                    int EnemyRandomIndex = PassRandom.Next(EnemyTypeCount - 1);
                    int TriggerRandomIndex = PassRandom.Next(TriggerTypeCount - 1);

                    this.EnemyCells[gr, gc] = EnemyStrings[EnemyRandomIndex];
                    this.InventoryCells[gr, gc] = InventoryStrings[InventoryRandomIndex];
                    this.TriggerCells[gr, gc] = TriggerStrings[TriggerRandomIndex];

                    this.GameGridCells[gr, gc] = new GameGridCell(gr, gc, this.EnemyCells[gr, gc], this.InventoryCells[gr, gc], this.TriggerCells[gr, gc]);
                    this.GameGridCells[gr, gc].Texture = this.GridTextures[0];

                    // determine which, if any of the inventory texture to display
                    switch(this.InventoryCells[gr,gc])
                    {
                        case "SGN_AMMO":
                            {
                                this.GameGridCells[gr, gc].InventoryTexture = texts[1];
                                break;
                            }
                        case "MGN_AMMO":
                            {
                                //this.GameGridCells[gr, gc].InventoryTexture = texts[1];
                                break;
                            }
                        case "SGN":
                            {
                                //this.GameGridCells[gr, gc].InventoryTexture = texts[1];
                                break;
                            }
                        case "MGN":
                            {
                                //this.GameGridCells[gr, gc].InventoryTexture = texts[1];
                                break;
                            }
                        case "HTH":
                            {
                                //this.GameGridCells[gr, gc].InventoryTexture = texts[1];
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }                    
                }
            }
        }

        /// <summary>
        /// Loads a predefined gamegrid
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="ecs"></param>
        /// <param name="ivs"></param>
        /// <param name="tcs"></param>
        public GameGrid(Texture2D[] texts,int w, int h, string[,] ecs, string[,] ivs, string[,] tcs,string[,]tlcs)
        {
            this.GridWidth = w;
            this.GridHeight = h;

            this.GridTextures = texts;

            this.GameGridCells = new GameGridCell[w, h];
            this.EnemyCells = ecs;
            this.InventoryCells = ivs;
            this.TriggerCells = tcs;
            this.TileCells = tlcs;
        }

        #endregion
    }

    class Hud
    {
        #region Data Members 

        private GameGrid CurrentGameGrid;
        private List<Entity> CurrentGameEntities;
        private SpriteBatch ManagerSpriteBatch;
        private Vect2D Position, Dimensions;

        public Texture2D PlayerDot, EnemyDot,GridTexture;

        #endregion

        #region Construction

        public Hud(int x,int y, int w,int h,GameGrid gg,List<Entity> ge,SpriteBatch sb)
        {
            this.CurrentGameGrid = gg;
            this.CurrentGameEntities = ge;
            this.ManagerSpriteBatch = sb;

            this.Position.X = x;
            this.Position.Y = y;
            this.Dimensions.X = w;
            this.Dimensions.Y = h;
        }

        #endregion Construction 

        /// <summary>
        /// Draws the current HUD representation of the game world
        /// </summary>
        public void Draw(List<Player> Players,List<Entity> Enemies)
        {
            // map enemies to HUD space
            Vect2D[] MappedEnemyPositions = new Vect2D[Enemies.Count];

            // draw enemy blips ( NEEDS WORK!!!!! )
            for (int e = 0; e < Enemies.Count; e++)
            {
                MappedEnemyPositions[e].X = this.Position.X + Enemies[e].Position.X;
                MappedEnemyPositions[e].Y = this.Position.Y + Enemies[e].Position.Y;

                if ((MappedEnemyPositions[e].X < (this.Position.X + this.Dimensions.X)) &&
                   (MappedEnemyPositions[e].Y < (this.Position.Y + this.Dimensions.Y)))
                {

                    this.ManagerSpriteBatch.Begin();

                    this.ManagerSpriteBatch.Draw(this.EnemyDot,
                                                 new Rectangle(MappedEnemyPositions[e].X,
                                                               MappedEnemyPositions[e].Y, 5, 5), Color.Green);

                    this.ManagerSpriteBatch.End();
                }
            }            

            // Draw Player blip, and grid
            this.ManagerSpriteBatch.Begin();

            this.ManagerSpriteBatch.Draw(this.PlayerDot,
                                         new Rectangle(this.Position.X + this.Dimensions.X/2     ,
                                                       this.Position.Y + this.Dimensions.Y/2, 5, 5),
                                         Color.Red);

            this.ManagerSpriteBatch.Draw(this.GridTexture, 
                                         new Microsoft.Xna.Framework.Rectangle(this.Position.X, this.Position.Y, this.Dimensions.X, this.Dimensions.Y), Color.White);

            this.ManagerSpriteBatch.End();
        }
    }

    class HealthBar : Entity
    {
        #region Data Members

        private Texture2D[] AdditionalTextures;
        private Player MonitoredPlayer;
        private SpriteBatch RenderingSpriteBatch;

        #endregion

        #region Construction

        public HealthBar(Player p, SpriteBatch sb, Texture2D[] Textures) : base(20,20,250,32)
        {
            this.MonitoredPlayer = p;
            this.AdditionalTextures = Textures;
            this.Texture = Textures[0];
            this.RenderingSpriteBatch = sb;
        }

        #endregion

        /// <summary>
        /// Draws the current state of the users health
        /// </summary>
        /// <param name="Health"></param>
        internal void Draw()
        {
            int PercentAlive = (int)(100 * (float)this.MonitoredPlayer.Health / (float)100);
            int PercentDead = 100 - PercentAlive;
            int RagePercentage = this.MonitoredPlayer.RageLevel / 100;
            int Slice = 248 / 100;

            this.RenderingSpriteBatch.Begin();

            // outline
            this.RenderingSpriteBatch.Draw(this.Texture, new Rectangle(this.Position.X, this.Position.X, this.Dimensions.X, this.Dimensions.Y), Color.White);
            // portion alive
            this.RenderingSpriteBatch.Draw(this.AdditionalTextures[2], new Rectangle(this.Position.X + 1, this.Position.Y + 1, Slice*(this.MonitoredPlayer.Health), 21), Color.White);
            // rage meter
            //this.RenderingSpriteBatch.Draw(this.AdditionalTextures[3], new Rectangle(this.Position.X + 1, this.Position.Y + 23, this.Dimensions.X, 10), Color.White);

            this.RenderingSpriteBatch.End();
        }
    }

    class WeaponDisplay : Entity 
    {
        #region Data Members

        #endregion

        #region Construction 

        public WeaponDisplay()
            : base(20, 50, 250, 50)
        {

        }    

        #endregion 
    }

    class AmmoDisplay : Entity
    {
        public AmmoDisplay()
            : base(300, 50, 32, 32)
        {
        }
    }

    class TimeDisplay : Entity
    {
        public TimeDisplay()
            : base(0, 0, 0, 0)
        {

        }
    }
}
