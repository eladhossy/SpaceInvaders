using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Infrastructure.ServiceInterfaces;
using Infrastructure.Managers;
using SpaceInvaders.Screens;

namespace SpaceInvaders
{
    public class SpaceInvaders : Microsoft.Xna.Framework.Game
    {
        private Texture2D m_BackGround;
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private IInputManager m_InputManager;
        private ICollisionManager m_CollisionManager;
        private ScreensMananger m_ScreensManager;
        private IAudioManager m_AudioManager;
        private AudioEngine m_AudioEngine;
        private SoundBank m_SoundBank;
        private Cue m_BGMusic;
        private float m_InitialMusicVolume = 4;
        private float m_InitialSoundFXVolume = 1;

        public SpaceInvaders()
        {
            IsMouseVisible = true;

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            
            ////create services for the game
            m_InputManager = new InputManager(this);
            m_CollisionManager = new CollisionManager(this);
            m_ScreensManager = new ScreensMananger(this);

            // create the Audio service
            // the audio service is a service that we wrote to encapsulate and reuse the audio logic across the game code.
            // it can be found in the infrastructure project, attached to the solution, under Managers folder.
            m_AudioEngine = new AudioEngine(@"Content\Audio\spaceInvadersAutioProject.xgs");
            m_AudioManager = new AudioManager(this, m_AudioEngine);
            m_AudioManager.AddWaveBank(new WaveBank(m_AudioEngine, @"Content\Audio\WaveBank.xwb"));
            m_SoundBank = new SoundBank(m_AudioEngine, @"Content\Audio\SoundBank.xsb");
            m_AudioManager.AddSoundBank("SpaceInvadersSoundBank", m_SoundBank);
            m_AudioManager.AddAudioCategory("Music", m_InitialMusicVolume);
            m_AudioManager.AddAudioCategory("SoundFX", m_InitialSoundFXVolume);

            // Adding the screens
            m_ScreensManager.SetCurrentScreen(new SpaceInvadersPlayScreen(this, "SpaceInvadersSoundBank", SpaceInvadersPlayScreen.eGameMode.OnePlayer));
            m_ScreensManager.SetCurrentScreen(new LevelTransitionScreen(this, 1, "SpaceInvadersSoundBank"));
            m_ScreensManager.SetCurrentScreen(new WelcomeScreen(this, "SpaceInvadersSoundBank"));
        }

        protected override void Initialize()
        {
            base.Initialize();
            m_BGMusic = m_SoundBank.GetCue("BGMusic");
            m_BGMusic.Play();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (m_InputManager.KeyPressed(Microsoft.Xna.Framework.Input.Keys.M))
            {
                if (m_AudioManager.GetAudioState() == eVolumeStates.On)
                {
                    m_AudioManager.SetAudioState(eVolumeStates.Off);
                }
                else
                {
                    m_AudioManager.SetAudioState(eVolumeStates.On);
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            spriteBatch.Draw(m_BackGround, Vector2.Zero, Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            spriteBatch = new SpriteBatch(GraphicsDevice);
            m_BackGround = this.Content.Load<Texture2D>(@"Sprites\BG_Space01_1024x768");
        }
    }
}
