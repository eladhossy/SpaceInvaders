using System;
using Infrastructure.ObjectModel.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Infrastructure.ObjectModel;

namespace SpaceInvaders.Screens
{
    public class PauseScreen : GameScreen
    {
        private TextWriter m_Message;
        private TextWriter m_PauseLogo;

        public PauseScreen(Game i_Game)
            : base(i_Game)
        {
            this.IsModal = true;
            this.IsOverlayed = true;
            this.UseGradientBackground = true;
            this.BlackTintAlpha = 0.65f;

            m_PauseLogo = new TextWriter(Game, @"Fonts\Calibri70");
            Add(m_PauseLogo);
            m_Message = new TextWriter(Game, @"Fonts\Calibri14");
            Add(m_Message);
        }

        public override void Initialize()
        {
            base.Initialize();
            m_PauseLogo.Position = CenterOfViewPort + new Vector2(-150);
            m_Message.Position = m_PauseLogo.Position + new Vector2(0, 100);

            m_PauseLogo.TextToWrite = "Pause";
            m_Message.TextToWrite = "Press 'R' To Return To Game";
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (InputManager.KeyPressed(Keys.R))
            {
                this.ExitScreen();
            }
        }
    }
}

