using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.ObjectModel.Screens;
using Microsoft.Xna.Framework;
using Infrastructure.ObjectModel;

namespace SpaceInvaders.Screens
{
    public class GameOverScreen : GameScreen
    {
        private Sprite m_GameOverLogo;
        private TextWriter m_GameOverMenu;
        private TextWriter m_GameOverMessage;
        private string m_SoundBankName;
        private ScoreBoard m_ScoreBoard;

        public GameOverScreen(Game i_Game, string i_SoundBankName, ScoreBoard i_ScoreBoard)
            : base(i_Game)
        {
            m_SoundBankName = i_SoundBankName;
            m_GameOverLogo = new Sprite(this.Game, @"Sprites\GameOver");
            this.Add(m_GameOverLogo);

            m_GameOverMenu = new TextWriter(this.Game, @"Fonts\Calibri14");
            this.Add(m_GameOverMenu);

            m_GameOverMessage = new TextWriter(this.Game, @"Fonts\Calibri14");
            this.Add(m_GameOverMessage);

            m_ScoreBoard = i_ScoreBoard;
            this.Add(i_ScoreBoard);
        }

        public override void Initialize()
        {
            base.Initialize();
            initComponents();
        }

        private void initComponents()
        {
            // init Game Over Logo
            m_GameOverLogo.PositionOrigin = m_GameOverLogo.SourceRectangleCenter;
            m_GameOverLogo.Position = new Vector2(CenterOfViewPort.X, 100);

            // init TextWriters
            m_GameOverMessage.Position = m_GameOverLogo.Position + new Vector2(-100, 100);
            m_GameOverMessage.TextToWrite = @"
The Game Is Over! Final Scores:";

            m_ScoreBoard.Position = m_GameOverMessage.Position + new Vector2(50);

            m_GameOverMenu.Position = m_GameOverMessage.Position + new Vector2(0, 100);

            m_GameOverMenu.TextToWrite = @"
Choose An Option:
    Esc - Exit The Game
    N   - Start A New Game
    F1 - Show The Main Menu";
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (InputManager.KeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                Game.Exit();
            }

            if (InputManager.KeyPressed(Microsoft.Xna.Framework.Input.Keys.N))
            {
                ExitScreen();
            }

            if (InputManager.KeyPressed(Microsoft.Xna.Framework.Input.Keys.F1))
            {
                ScreensManager.SetCurrentScreen(new MainMenu(Game, m_SoundBankName, "MenuMove"));
            }
        }
    }
}
