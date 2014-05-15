using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Infrastructure.ServiceInterfaces;
using Infrastructure.ObjectModel.Animators.ConcreteAnimators;
using Infrastructure.ObjectModel.Screens;

namespace SpaceInvaders
{
    public delegate void InvaderIsDeadEventHandler(Invader i_Invader, ICollidable i_Collidable);
    
    public delegate void InvaderReachedBottomOfScreenEventHandler();
    
    public class Invader : Sprite, ICollidable, IGunHolder
    {
        private const string k_TextureName = @"Sprites\AllInvaders";
        private int m_NumOfFrames = 6;
        private int m_FirstSourceRectangle;
        private int m_SecondSourceRecatangle;
        private int m_PointsForDestruction;
        
        public CelAnimator InvaderCelAnimator { get; set; }
        
        public event InvaderIsDeadEventHandler InvaderIsDead;
        
        public event InvaderReachedBottomOfScreenEventHandler InvaderReachedBottomOfScreen;
        
        private double m_TimeBetweenShoots;
        private double m_TimeToShoot = 0;
        private MachineGun m_MachineGun;
        private Random m_Rnd;
        private GameScreen m_GameScreen;
        private SoundBank m_SoundBank;
        private string m_HitCueName;
        private ICollidable m_BulletThatKilledMe;
     
        public int PointsForDestruction
        {
            get { return m_PointsForDestruction; }
            set { m_PointsForDestruction = value; }
        }

        public Invader(Game i_Game, Color i_TintColor, int i_PointsForDestruction, int i_InvaderFirstSourceRectangle, int i_InvaderSecondSourceRectangle, GameScreen i_GameScreen, int i_NumOfBullets)
            : base(i_Game, k_TextureName)
        {
            m_PointsForDestruction = i_PointsForDestruction;
            m_TintColor = i_TintColor;
            m_MachineGun = new MachineGun(i_Game, Color.Blue, this, i_GameScreen, i_NumOfBullets);
            m_MachineGun.VelocityOfBullets = new Vector2(0, 155);
            m_MachineGun.Position = new Vector2(300, 300);
            i_GameScreen.Add(m_MachineGun);
            m_Rnd = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
            m_TimeBetweenShoots = m_Rnd.Next(5, 20);
            Thread.Sleep(20); // in order for the random-seed to be different in each instanciation of each invader, 
            m_FirstSourceRectangle = i_InvaderFirstSourceRectangle;
            m_SecondSourceRecatangle = i_InvaderSecondSourceRectangle;
            m_GameScreen = i_GameScreen;
        }

        public void SetShootSound(SoundBank i_SoundBank, string i_CueName)
        {
            m_MachineGun.SetShootSound(i_SoundBank, i_CueName);
        }

        public override void Update(GameTime i_GameTime)
        {
            m_MachineGun.TriggerDown = false;
            m_TimeToShoot += i_GameTime.ElapsedGameTime.TotalSeconds;
            if (m_TimeToShoot >= m_TimeBetweenShoots)
            {
                m_MachineGun.TriggerDown = true;
                m_TimeToShoot = 0;
                m_TimeBetweenShoots = m_Rnd.Next(10, 20);
            }
          
           base.Update(i_GameTime);
           m_MachineGun.Position = Position;
           m_MachineGun.Update(i_GameTime);
        }

        private bool m_IsDying = false;
        
        public override void Collided(ICollidable i_Collidable)
        {
            if (!m_IsDying)
            {
                if (i_Collidable is IBullet)
                {
                    IGunHolder gunHolder = (i_Collidable as IBullet).Gun.GunHolder;
                    if (gunHolder.GetType() != this.GetType())
                    {
                        m_BulletThatKilledMe = i_Collidable;
                        
                        if (m_SoundBank != null)
                        {
                            m_SoundBank.PlayCue(m_HitCueName);
                        }

                        m_IsDying = true;
                        ShrinkAnimator shrinkAnimator = new ShrinkAnimator("shrink", new Vector2(2f), TimeSpan.FromSeconds(0.5));
                        shrinkAnimator.Finished += new EventHandler(shrinkAnimator_Finished);
                        BlinkAnimator blinkAnimator = new BlinkAnimator("blink", TimeSpan.FromSeconds(0.16), TimeSpan.FromSeconds(0.5));
                       
                        Animations.Add(blinkAnimator);
                        Animations.Add(shrinkAnimator);
                       (i_Collidable as Sprite).Dispose();
                    }
                }
            }
        }

        private void shrinkAnimator_Finished(object sender, EventArgs e)
        {
            Visible = false;
            Dispose();
            m_GameScreen.Remove(this);
            
            if (InvaderIsDead != null)
            {
                InvaderIsDead.Invoke(this, m_BulletThatKilledMe);
            }
        }

        protected override void OnPositionChanged()
        {
            if (Position.Y + Height >= GraphicsDevice.Viewport.Height - 32)
            {
                if (InvaderReachedBottomOfScreen != null)
                {
                    if (Visible)
                    {
                        InvaderReachedBottomOfScreen.Invoke();
                    }
                }
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            initAnimations();
        }

        private void initAnimations()
        {
            InvaderCelAnimator = new CelAnimator(TimeSpan.FromSeconds(0.5), TimeSpan.Zero, m_FirstSourceRectangle, m_SecondSourceRecatangle);
            
            Animations.Add(InvaderCelAnimator);
            Animations.Enabled = true;
        }

        protected override void InitOrigins()
        {
            RotationOrigin = new Vector2(Width / 2, Height / 2);
        }

        protected override void InitSourceRectangle()
            {
                base.InitSourceRectangle();
                int cellWidth = m_SourceRectangle.Width / m_NumOfFrames;
                WidthBeforeScale = cellWidth;

                this.SourceRectangle = new Rectangle(
                    m_FirstSourceRectangle * cellWidth,
                    0,
                    (int)(m_SourceRectangle.Width / m_NumOfFrames),
                    (int)m_HeightBeforeScale);
           }

        public void SetHitSound(SoundBank i_SoundBank, string i_CueName)
        {
            m_SoundBank = i_SoundBank;
            m_HitCueName = i_CueName;
        }
    }
}
