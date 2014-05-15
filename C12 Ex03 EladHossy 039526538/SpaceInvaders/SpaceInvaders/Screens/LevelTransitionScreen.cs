using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.ObjectModel.Screens;
using Microsoft.Xna.Framework;
using Infrastructure.ObjectModel;
using Infrastructure.ServiceInterfaces;

namespace SpaceInvaders.Screens
{
    public class LevelTransitionScreen : GameScreen
    {
        private TextWriter m_LevelNumberWriter;
        private TextWriter m_CountDownWriter;
        private float m_SecondsTimer = 4;
        private int m_LevelNumber;
        private string m_SoundBankName;
        private IAudioManager m_AudioManager;

        public LevelTransitionScreen(Game i_Game, int i_LevelNumber, string i_SoundBankName)
            : base(i_Game)
        {
            m_SoundBankName = i_SoundBankName;
            m_LevelNumber = i_LevelNumber;

            m_LevelNumberWriter = new TextWriter(Game, @"Fonts\Calibri70");
            this.Add(m_LevelNumberWriter);

            m_CountDownWriter = new TextWriter(Game, @"Fonts\Calibri70");
            this.Add(m_CountDownWriter);
        }

        public override void Initialize()
        {
            base.Initialize();
            m_AudioManager = (IAudioManager)Game.Services.GetService(typeof(IAudioManager));
            m_LevelNumberWriter.TextToWrite = String.Format("Level {0}!", m_LevelNumber);

            m_LevelNumberWriter.Position = CenterOfViewPort - new Vector2(100);
            m_CountDownWriter.Position = m_LevelNumberWriter.Position + new Vector2(110);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            if(InputManager.KeyPressed(Microsoft.Xna.Framework.Input.Keys.K))
            {
                ScreensManager.SetCurrentScreen(new WelcomeScreen(Game, m_SoundBankName));
            }
            
            m_SecondsTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (m_SecondsTimer <= 1)
            {
                this.ExitScreen();
            }

            m_CountDownWriter.TextToWrite = ((int)m_SecondsTimer).ToString();
        }
    }
}
