
/*
 * Entity bases clases [012315] Jto_daA
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GJ20215_BecauseZombies
{
    public struct Vect2D
    {
        public int X,Y;
    }
    
    /// <summary>
    /// Base entity class
    /// </summary>
    class Entity
    {
        #region Data Members

        public Vect2D Position, Dimensions;
        public Texture2D Texture;
        public int CurrentFrame;
        public int CurrentRow;

        public bool Relevant;
        public Vect2D Target;
                                
        #endregion 

        #region Construction 

        public Entity(int x, int y,int w,int h)
        {
            this.Position.X = x;
            this.Position.Y = y;

            this.Dimensions.X = w;
            this.Dimensions.Y = h;

            this.CurrentFrame = 0;
            this.CurrentRow = 0;

            this.Relevant = true;
        }

        #endregion 

        public virtual void Think(GameTime Gt)
        {

        }        
    }

    class Player : Entity
    {
        #region Data Members 

        public Inventory CurrentInventory;
        public int Health, RageLevel, RemainingLives;

        public bool IsEnraged;

        // 4-dof status
        public bool IsFacingRight, IsFacingLeft, IsFacingUp, IsFacingDown;
        public bool HasRightSingleTap, HasLeftSingleTap, HasUpSingleTap, HasDownSingleTap;
        public int UpSingleTapTime, DownSingleTapTime, LeftSingleTapTime, RightSingleTapTime;
        public bool MoveupKeyWasPressed;
        public bool MoveLeftKeyWasPressed;
        public bool MoveDownKeyWasPressed;
        public bool MoveRightKeyWasPressed;
        public int TimeLastRoundFired;

        #endregion 

        #region Construction 

        public Player() : base(400,300,32,32)
        {
            this.CurrentInventory = new Inventory();
            int Health = 100;

            this.IsFacingUp = true;
            this.IsFacingDown = this.IsFacingLeft = this.IsFacingRight = false;
        }

        #endregion
    }

    public enum ProjectileType
    {
        NormalBullet
    }

    class Projectile : Entity
    {
        #region Data Members

        Vect2D Velocity;
        private TimeSpan TimeAirborne;
        private ProjectileType AmmoType;
        
        #endregion


        public Projectile(ProjectileType t,int x,int y,int w,int h,int vx,int vy,Texture2D tex) : base(x,y,w,h)
        {
            this.Velocity.X = vx;
            this.Velocity.Y = vy;
            this.AmmoType = t;

            this.Texture = tex;
        }

        /// <summary>
        /// Projectiles, for now, will move at a constant rate once fired.
        /// </summary>
        /// <param name="Gt"></param>
        public override void Think(GameTime Gt)
        {
            this.TimeAirborne += Gt.ElapsedGameTime;

            switch (this.AmmoType)
            {
                default:
                    {
                        // attenuate... 
                        break;
                    }
            }

            if (TimeAirborne < new TimeSpan(0, 0, 3))
            {
                this.Position.X += this.Velocity.X;
                this.Position.Y += this.Velocity.Y;
            }
            else
                this.Relevant = false;

            // ALL THIS IS IRRELEVANT NOW THAT THE BULLET SIMPLY TRAVELS FOR A FIXED AMOUNT OF TIME.
            //if (this.Velocity.Y < 0) // moving S2N
            //{
            //    if (TimeAirborne < new TimeSpan(0,0,3))
            //    {
            //        this.Position.X += this.Velocity.X;
            //        this.Position.Y += this.Velocity.Y;
            //    }
            //    else
            //        this.Relevant = false;
            //}
            //else if (this.Velocity.Y > 0) // moving N2S
            //{
            //    if ((this.Position.X < 800) &&
            //        (this.Position.Y < 600))
            //    {
            //        this.Position.X += this.Velocity.X;
            //        this.Position.Y += this.Velocity.Y;
            //    }
            //    else
            //        this.Relevant = false;
            //}

            //if (this.Velocity.X > 0) // moving W2E
            //{
            //    if ((this.Position.X < 800) &&
            //        (this.Position.Y < 600))
            //    {
            //        this.Position.X += this.Velocity.X;
            //        this.Position.Y += this.Velocity.Y;
            //    }
            //    else
            //        this.Relevant = false;
            //}
            //else if (this.Velocity.X < 0) // moving E2W
            //{
            //    if ((this.Position.X > 0) &&
            //        (this.Position.Y > 0))
            //    {
            //        this.Position.X += this.Velocity.X;
            //        this.Position.Y += this.Velocity.Y;
            //    }
            //    else
            //        this.Relevant = false;
            //}
        }        
    }

    public enum EnemyType
    {
        RunOfTheMillZombieGuy, 
        Flocker,
        Juggernaut
    }   

    class Enemy : Entity
    {
        #region Data Members
                
        public bool IsFacingUp, IsFacingDown, IsFacingRight, IsFacingLeft;

        #endregion

        #region Construction 

        public Enemy(EnemyType t,int x,int y,int w,int h,int gh,int gw,Texture2D tex)
            : base(new Random(DateTime.Now.Millisecond).Next(0, gw), new Random(DateTime.Now.Millisecond).Next(0, gh), 32, 32)
        {
            this.Texture = tex;
        }

        
        /// <summary>
        /// Adusted zombie behavior ( TRACKING )
        /// 
        /// /// </summary>
        /// <param name="Gt"></param>
        public override void Think(GameTime Gt)
        {
            int ZombieSpeed = new Random(DateTime.Now.Millisecond).Next(0, Gt.TotalGameTime.Minutes + 2);

            //if (new Random(DateTime.Now.Second).Next(0, 5) > ZombieSpeed)
            //    return;

            if (this.Target.X > this.Position.X)
            {
                this.IsFacingLeft = true;
                this.IsFacingRight = this.IsFacingUp = this.IsFacingDown = false;
                this.Position.X += ZombieSpeed;
            }
            else if (this.Target.X < this.Position.X)
            {
                this.IsFacingRight = true;
                this.IsFacingLeft = this.IsFacingUp = this.IsFacingDown = false; 
                this.Position.X -= ZombieSpeed;
            }

            if (this.Target.Y > this.Position.Y)
            {
                this.IsFacingUp = true;
                this.IsFacingDown = this.IsFacingLeft = this.IsFacingRight = false;
                this.Position.Y += ZombieSpeed;
            }

            else if (this.Target.Y < this.Position.Y)
            {
                this.IsFacingDown = true;
                this.IsFacingUp = this.IsFacingLeft = this.IsFacingRight = false;
                this.Position.Y -= ZombieSpeed;
            }
        }

        #endregion 
    }

    class Inventory
    {
        public int PistolCount, ShotgunCount, MachineGunCount;
        public int ShotgunAmmoPackCount, MachineGunClipCount;
        public int MaxClipCount = 1000;
        public int MaxShotgunPackCount = 1000;

        #region Construction 

        public Inventory()
        {

        }

        #endregion
    }
}
