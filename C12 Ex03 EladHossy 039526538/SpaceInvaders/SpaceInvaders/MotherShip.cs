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
using Infrastructure.ObjectModel.Animators.ConcreteAnimators;

namespace SpaceInvaders
{
    public delegate void MotherShipIsDeadEventHandler(MotherShip i_MotherShip, ICollidable i_Collidable);
   
    public class MotherShip : Sprite, ICollidable
    {
        private const string k_TextureName = @"Sprites\MotherShip_32x120";
        private double m_TimeBetweenAppearances = 5;
        private double m_TimeToAppear = 0;
        private Vector2 m_InitPosition;
        private SoundBank m_SoundBank;
        private string m_HitCueName;
        
        public event MotherShipIsDeadEventHandler MotherShipIsDead;
        
        public int PointsForDestruction { get; set; }

        public MotherShip(Game game, Color i_TintColor)
            : base(game, k_TextureName)
        {
            m_TintColor = i_TintColor;
            Visible = false;
            PointsForDestruction = 400;
        }

        public void SetHitSound(SoundBank i_SoundBank, string i_CueName)
        {
            m_SoundBank = i_SoundBank;
            m_HitCueName = i_CueName;
        }

        protected override void InitBounds()
        {
            base.InitBounds();
            m_Velocity = new Vector2(Width / 2, 0);
            m_InitPosition = new Vector2(0 - Width, Height);
            Position = m_InitPosition;
        }

        public override void Update(GameTime i_GameTime)
        {
            if (!m_IsDying)
            {
                Position += m_Velocity * (float)i_GameTime.ElapsedGameTime.TotalSeconds;
                m_TimeToAppear += i_GameTime.ElapsedGameTime.TotalSeconds;

                // calculate the time it takes the mother ship to travel across the screen
                double timeItTakesToTravelAcrossScreen = GraphicsDevice.Viewport.Width / m_Velocity.X;

                if (m_TimeToAppear >= m_TimeBetweenAppearances + timeItTakesToTravelAcrossScreen)
                {
                    Position = m_InitPosition;
                    Visible = true;
                    m_TimeToAppear -= m_TimeBetweenAppearances + timeItTakesToTravelAcrossScreen;
                }
            }

            base.Update(i_GameTime);
        }

        private bool m_IsDying = false;
        private ShrinkAnimator m_ShrinkAnimator = null;
        
        public override void Collided(ICollidable i_Collidable)
        {
            if (!m_IsDying)
            {
                if (m_SoundBank != null)
                {
                    m_SoundBank.PlayCue(m_HitCueName);
                }
                
                if (m_ShrinkAnimator == null)
                {
                    m_ShrinkAnimator = new ShrinkAnimator("Shrink", new Vector2(0.5f), TimeSpan.FromSeconds(2));
                    m_ShrinkAnimator.Finished += new EventHandler(m_ShrinkAnimator_Finished);
                    this.Animations.Add(m_ShrinkAnimator);
                    Animations.Enabled = true;
                }
                else
                {
                    m_ShrinkAnimator.Restart();
                }

                m_IsDying = true;

                if (MotherShipIsDead != null)
                {
                    MotherShipIsDead.Invoke(this, i_Collidable);
                }
            }
        }

        private void m_ShrinkAnimator_Finished(object sender, EventArgs e)
        {
            Visible = false;
            m_IsDying = false;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void InitOrigins()
        {
            m_RotationOrigin = new Vector2(Width / 2, Height / 2);
        }
    }
}
