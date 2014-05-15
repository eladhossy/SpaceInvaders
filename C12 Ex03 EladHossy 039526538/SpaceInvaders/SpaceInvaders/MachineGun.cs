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
using Infrastructure.ServiceInterfaces;
using Infrastructure.ObjectModel.Screens;

namespace SpaceInvaders
{
    public class MachineGun : Microsoft.Xna.Framework.GameComponent, IGun
    {
        public Vector2 Position { get; set; }
        
        private List<Bullet> m_Bullets;
        
        public bool TriggerDown { get; set; }
        
        private Color m_BulletsColor;
    
        public Vector2 VelocityOfBullets { get; set; }

        private GameScreen m_ContainingScreen;

        private SoundBank m_ShootSoundBank;

        private string m_ShootCueName;
        
        private int m_CartridgeSize; // this property sets the size of the cartridge for the machine gun = how many bullets it could fire at once

        public MachineGun(Game game, Color i_BulletsColor, IGunHolder i_GunHolder, GameScreen i_GameScreen, int i_CartrigeSize)
            : base(game)
        {
            m_CartridgeSize = i_CartrigeSize;
            m_Bullets = new List<Bullet>();
            m_BulletsColor = i_BulletsColor;
            GunHolder = i_GunHolder;
            m_ContainingScreen = i_GameScreen;
        }

        public void SetShootSound(SoundBank i_ShootSoundBank, string i_ShootCueName)
        {
            m_ShootSoundBank = i_ShootSoundBank;
            m_ShootCueName = i_ShootCueName;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (TriggerDown)
            {
                if (m_Bullets.Count < m_CartridgeSize) // is there ammo left in the cartrige? (so the gun could fire)
                {
                    Bullet newBullet = new Bullet(Game, m_BulletsColor, Position, VelocityOfBullets, this, m_ContainingScreen);
                    m_ContainingScreen.Add(newBullet);
                    if (m_ShootSoundBank != null)
                    {
                        m_ShootSoundBank.PlayCue(m_ShootCueName);
                    }

                    newBullet.BulletLeftTheScreen = new BulletLeftTheScreenEventHandler(bulletDisposedHandler);
                    m_Bullets.Add(newBullet);
                    TriggerDown = false;
                }
            }
           
            base.Update(gameTime);
        }

        private void bulletDisposedHandler(Bullet i_Bullet)
        {
            m_Bullets.Remove(i_Bullet);
        }

        public IGunHolder GunHolder { get; set; } 
    }
}
