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
using SpaceInvaders;
using Infrastructure.ServiceInterfaces;
using Infrastructure.ObjectModel.Screens;

public delegate void BulletLeftTheScreenEventHandler(Bullet i_Bullet);

namespace SpaceInvaders
{
    public class Bullet : PixelCollidableSprite, IBullet, ICollidable
    {
        private const string k_TextureName = @"Sprites\Bullet";
        public BulletLeftTheScreenEventHandler BulletLeftTheScreen;
        private const float maxHitDepth = 0.8f * 16f;
        private Vector2 m_StartingPosition;
        private GameScreen m_GameScreen;
       
        public Bullet(Game game, Color i_TintColor, Vector2 i_GunPosition, Vector2 i_Velocity, IGun i_Gun, GameScreen i_GameScreen)
            : base(game, k_TextureName)
        {
            Gun = i_Gun;
            m_TintColor = i_TintColor;
            m_StartingPosition = i_GunPosition;
            m_Velocity = i_Velocity;
            m_GameScreen = i_GameScreen;
        }

        protected override void InitBounds()
        {
            base.InitBounds();
        }
      
        public override void Initialize()
        {
            base.Initialize();
            Position = m_StartingPosition;
        }

        public override void Update(GameTime gameTime)
        {
            Position += m_Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Position.Y <= 0 || Position.Y >= GraphicsDevice.Viewport.Height) // the bullet has left the screen
            {
                if (BulletLeftTheScreen != null)
                {
                    BulletLeftTheScreen.Invoke(this);
                }
            }
            
            base.Update(gameTime);
        }

        public override void Collided(ICollidable i_Collidable)
        {
            IGunHolder myGunHolder = this.Gun.GunHolder;
            if (i_Collidable.GetType() != myGunHolder.GetType() && i_Collidable is Barrier)
            {
                base.Collided(i_Collidable);
            }
        }
        
        protected override void PixelCollided(int i_MyPixelIndex, int i_OtherPixelIndex, ICollidable i_Collidable)
        {
            if (i_Collidable is IBullet)
            {
                if (Velocity.Y > 0) // the bullet hit another bullet, and is shot from an invader, so we randomize the desicion for its' dispose
                {
                    Random random = new Random();
                    int randomNum = random.Next(0, 2);
                    if (randomNum == 1)
                    {
                        Dispose();
                    }
                }
                else
                {
                    this.Dispose();
                }
            }
            else
            {
                this.Dispose();
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            m_GameScreen.Remove(this);
            if (BulletLeftTheScreen != null)
            {
                BulletLeftTheScreen.Invoke(this);
            }
        }

        public IGun Gun { get; set; }

        public override void Draw(GameTime gameTime)
        {
            m_UseSharedBatch = false;
            base.Draw(gameTime);
        }
    }
}
