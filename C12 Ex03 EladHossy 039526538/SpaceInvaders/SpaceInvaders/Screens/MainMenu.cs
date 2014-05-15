using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.ObjectModel.Screens;
using Microsoft.Xna.Framework;
using Infrastructure.ObjectModel;

namespace SpaceInvaders.Screens
{
    public class MainMenu : GameMenu
    {
        private SpaceInvadersPlayScreen.eGameMode m_GameMode = SpaceInvadersPlayScreen.eGameMode.OnePlayer;

        public MainMenu(Game i_Game, string i_SoundBankName, string i_CueName)
            : base(i_Game, i_SoundBankName, i_CueName)
        {
            m_MenuName = "Main Menu";

            AddCommandMenuItem("Screen Options", () => ScreensManager.SetCurrentScreen(new ScreenOptionsMenu(Game, i_SoundBankName, i_CueName)));
            AddCommandMenuItem("Sound Options", () => ScreensManager.SetCurrentScreen(new SoundOptionsMenu(Game, i_SoundBankName, i_CueName)));
            AddScrollableMenuItem("Players: ", new string[] { "One", "Two" }, 0, (string numOfPlayers) =>
                {
                    if (numOfPlayers == "One")
                    {
                        m_GameMode = SpaceInvadersPlayScreen.eGameMode.OnePlayer;
                    }
                    else
                    {
                        m_GameMode = SpaceInvadersPlayScreen.eGameMode.TwoPlayers;
                    }
                });

            AddCommandMenuItem("Play", () => 
                {
                    ScreensManager.SetCurrentScreen(new SpaceInvadersPlayScreen(Game, i_SoundBankName, m_GameMode));
                    m_ScreensManager.SetCurrentScreen(new LevelTransitionScreen(Game, 1, "SpaceInvadersSoundBank"));
                });
            AddCommandMenuItem("Quit", () => Game.Exit());
        }
    }
}
