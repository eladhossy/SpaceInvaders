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
using Infrastructure.ObjectModel.Screens;

namespace SpaceInvaders
{
    public delegate void SpaceShipCollidedEventHandler(SpaceShip i_SpaceShip, ICollidable i_Collidable);
    
    public class SpaceShip : Sprite, IGunHolder, ICollidable
    {
        public enum eSpaceshipControlledOperations
        {
            MOVERIGHT,
            MOVELEFT,
            FIRE
        }

        public Vector2 StartingPosition { get; set; }
        
        private Dictionary<eSpaceshipControlledOperations, Keys> keyboardMapping;
        
        public void SetSpaceshipKeys(Keys i_MoveRightKey, Keys i_MoveLeftKey, Keys i_FireKey)
        {
            keyboardMapping[eSpaceshipControlledOperations.MOVERIGHT] = i_MoveRightKey;
            keyboardMapping[eSpaceshipControlledOperations.MOVELEFT] = i_MoveLeftKey;
            keyboardMapping[eSpaceshipControlledOperations.FIRE] = i_FireKey;
        }

        public string TextureName 
        { 
            get { return m_TextureName; } 
        }

        private MachineGun m_MachineGun;
        
        private IInputManager m_InputManager;
        
        public event SpaceShipCollidedEventHandler SpaceShipCollided;

        private SoundBank m_SoundBank;

        private string m_HitCueName;

        public SpaceShip(Game i_Game, string i_TextureName, GameScreen i_Screen, int i_CartridgeSize)
            : base(i_Game, i_TextureName)
        {
            m_TextureName = i_TextureName;
            m_Velocity.X = 165;
            m_Velocity.Y = 0;
            m_MachineGun = new MachineGun(i_Game, Color.Red, this, i_Screen, i_CartridgeSize);
            m_MachineGun.VelocityOfBullets = new Vector2(0, -155);
            i_Screen.Add(m_MachineGun);
            keyboardMapping = new Dictionary<eSpaceshipControlledOperations, Keys>();
            keyboardMapping.Add(eSpaceshipControlledOperations.FIRE, Keys.Enter); // default
            keyboardMapping.Add(eSpaceshipControlledOperations.MOVELEFT, Keys.Left); // default
            keyboardMapping.Add(eSpaceshipControlledOperations.MOVERIGHT, Keys.Right); // default
            IsMouseControlled = false;
        }

        public void SetShootSound(SoundBank i_SoundBank, string i_CueName)
        {
            m_MachineGun.SetShootSound(i_SoundBank, i_CueName);
        }

        public void SetHitSound(SoundBank i_SoundBank, string i_CueName)
        {
            m_SoundBank = i_SoundBank;
            m_HitCueName = i_CueName;
        }

        public override void Initialize()
        {
            m_InputManager = (IInputManager)Game.Services.GetService(typeof(IInputManager));
            base.Initialize();
        }

        public bool IsMouseControlled; // so the mouse can control only player 1
        
        public override void Update(GameTime i_GameTime)
        {
            if (!m_IsDying)
            {
                base.Update(i_GameTime);
                if (IsMouseControlled)
                {
                    if (m_InputManager.MousePositionDelta.X != 0)
                    {
                        m_Position.X = m_InputManager.MouseState.X;
                    }
                }

                if (Keyboard.GetState().IsKeyDown(keyboardMapping[eSpaceshipControlledOperations.MOVERIGHT]))
                {
                    Position += m_Velocity * (float)i_GameTime.ElapsedGameTime.TotalSeconds;
                }

                if (Keyboard.GetState().IsKeyDown(keyboardMapping[eSpaceshipControlledOperations.MOVELEFT]))
                {
                    Position -= m_Velocity * (float)i_GameTime.ElapsedGameTime.TotalSeconds;
                    this.Animations.Enabled = true;
                }

                m_Position.X = MathHelper.Clamp(Position.X, 0, this.GraphicsDevice.Viewport.Width - m_Texture.Width);
                m_MachineGun.Position = new Vector2(Position.X + (Width / 2) - 2, Position.Y - 30); // The space ship update its gun about the new position
                m_MachineGun.TriggerDown = m_InputManager.KeyPressed(keyboardMapping[eSpaceshipControlledOperations.FIRE]) || (m_InputManager.ButtonPressed(eInputButtons.Left) && IsMouseControlled);
            }

            base.Update(i_GameTime);
        }

        private BlinkAnimator m_BlinkAnimator = null;
        private FadeAnimator m_FadeAnimator = null;
        private RotateAnimator m_RotateAnimator = null;
        private bool m_IsDying = false;
        
        public override void Collided(ICollidable i_Collidable)
        {
            if (i_Collidable.GetType() != this.GetType() && !m_IsDying)
            {
                if (m_SoundBank != null)
                {
                    m_SoundBank.PlayCue(m_HitCueName);
                }

                m_IsDying = true;
                if (m_BlinkAnimator == null)
                {
                    m_BlinkAnimator = new BlinkAnimator("blink", TimeSpan.FromSeconds(0.25), TimeSpan.FromSeconds(2));
                    m_FadeAnimator = new FadeAnimator("fade", 0.25f, TimeSpan.FromSeconds(2));
                    m_FadeAnimator.Finished += new EventHandler(m_FadeAnimator_Finished);
                    m_RotateAnimator = new RotateAnimator("rotate", (float)Math.PI * 2 * 4, TimeSpan.FromSeconds(2));
                    this.Animations.Add(m_BlinkAnimator);
                    this.Animations.Add(m_FadeAnimator);
                    this.Animations.Add(m_RotateAnimator);
                    this.Animations.Enabled = true;
                }
                else
                {
                    m_BlinkAnimator.Restart();
                    m_FadeAnimator.Restart();
                    m_RotateAnimator.Restart();
                }

                (i_Collidable as Sprite).Dispose();
                if (SpaceShipCollided != null)
                {
                    SpaceShipCollided.Invoke(this, i_Collidable);
                }
            }
        }

        private void m_FadeAnimator_Finished(object sender, EventArgs e)
        {
            m_IsDying = false;
            this.Position = StartingPosition; // after a soul is dying, the spacship is positioned in the starting position
        }

        protected override void InitOrigins()
        {
            RotationOrigin = new Vector2(Width / 2, Height / 2); // so the rotation when dying is around the center
        }
    }
}
