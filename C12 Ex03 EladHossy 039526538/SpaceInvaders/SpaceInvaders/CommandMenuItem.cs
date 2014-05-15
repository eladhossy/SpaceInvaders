using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SpaceInvaders
{
    public class CommandMenuItem : MenuItem
    {
        public Action ItemCommand { get; set; }

        public CommandMenuItem(string i_Text, string i_FontName, Action i_Action, Game i_Game)
            : base(i_Text, i_Game, i_FontName)
        {
            ItemCommand = i_Action;
        }
    }
}
