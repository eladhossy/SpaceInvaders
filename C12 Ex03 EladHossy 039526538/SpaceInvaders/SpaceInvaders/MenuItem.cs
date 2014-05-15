using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.ObjectModel;
using Microsoft.Xna.Framework;

namespace SpaceInvaders
{
    public class MenuItem
    {
        protected string m_Text;
        
        public string Text 
        {
            get
            {
                return m_Text;
            }

            set
            {
                m_Text = value;
                ItemTextWriter.TextToWrite = m_Text;
            }
        }
       
        public TextWriter ItemTextWriter { get; set; }

        public MenuItem(string i_Text, Game i_Game, string i_FontName)
        {
            this.ItemTextWriter = new TextWriter(i_Game, i_FontName);
            this.Text = i_Text;
        }
    }
}
