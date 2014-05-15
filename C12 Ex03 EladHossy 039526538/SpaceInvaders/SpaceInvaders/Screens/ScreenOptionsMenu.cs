using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SpaceInvaders.Screens
{
    public class ScreenOptionsMenu : GameMenu
    {
        public ScreenOptionsMenu(Game i_Game, string i_SoundBankName, string i_CueName)
            : base(i_Game, i_SoundBankName, i_CueName)
        {
            m_MenuName = "Screen Options";

            AddScrollableMenuItem("Allow Window Resizing: ", new string[] { "Off", "On" }, 0, (string onOrOff) =>
                {
                    Game.Window.AllowUserResizing = onOrOff == "On";
                });

            AddScrollableMenuItem("Mouse Visibility: ", new string[] { "Visible", "Invisible" }, 0, (string visibility) =>
                {
                    Game.IsMouseVisible = visibility == "Visible";
                });

            AddScrollableMenuItem("Full Screen Mode: ", new string[] { "Off", "On" }, 0, (string onOrOff) =>
                {
                    GraphicsDeviceManager graphics = (GraphicsDeviceManager)Game.Services.GetService(typeof(IGraphicsDeviceManager));
                    graphics.ToggleFullScreen();
                });

            AddCommandMenuItem("Done", () => ExitScreen());
        }
    }
}
