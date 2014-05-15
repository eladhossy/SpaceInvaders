using System;
using Infrastructure.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Infrastructure.ObjectModel.Animators.ConcreteAnimators;
using Infrastructure.ObjectModel.Screens;

namespace SpaceInvaders.Screens
{
    public class WelcomeScreen : GameScreen
    {
        private Sprite m_Logo;
        private Sprite m_Background;
        private TextWriter m_TextWriter;
        private MainMenu m_MainMenu;
        private string m_SoundBankName;
        
        public WelcomeScreen(Game i_Game, string i_SoundBankName) 
            : base(i_Game)
        {
            m_SoundBankName = i_SoundBankName;
            m_Background = new Sprite(this.Game, @"Sprites\BG_Space01_1024x768");
            this.Add(m_Background);
            
            m_Logo = new Sprite(this.Game, @"Sprites\SpaceInvadersLogo");
            this.Add(m_Logo);

            m_TextWriter = new TextWriter(Game, @"Fonts\Calibri14");
            this.Add(m_TextWriter);

            m_MainMenu = new MainMenu(Game, m_SoundBankName,"MenuMove");
        }

        public override void Initialize()
        {
            base.Initialize();
            initComponentsPositions();
        }

        private void initComponentsPositions()
        {
            m_Logo.Position = new Vector2(CenterOfViewPort.X - (m_Logo.Width / 2), 60);
            m_Logo.RotationOrigin = m_Logo.SourceRectangleCenter;
            m_TextWriter.Position = new Vector2(m_Logo.Bounds.Left + 60, m_Logo.Bounds.Bottom + 20);
            m_TextWriter.TextToWrite = @"
Hello, and welcome to Space Invaders!!
  - Press Enter to start the game.
  - Press O for the Main Menu
                                                                                                                                                                                                        - Press Esc to exit";
            m_Logo.Animations.Add(new PulseAnimator("Pulse", TimeSpan.Zero, 1.05f, 0.7f));
            m_Logo.Animations.Enabled = true;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (InputManager.KeyPressed(Keys.Enter))
            {
                ExitScreen();
            }

            if (InputManager.KeyPressed(Keys.O))
            {
                ScreensManager.SetCurrentScreen(m_MainMenu);
            }
        }
    }
}
