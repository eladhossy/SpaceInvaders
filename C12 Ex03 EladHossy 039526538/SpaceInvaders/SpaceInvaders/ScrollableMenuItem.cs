using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SpaceInvaders
{
    public class ScrollableMenuItem : MenuItem
    {
        public string[] Options;
        
        private int m_Index;
        
        private int m_DefaultIndex;
        
        private string m_BaseText;
        
        public Action<string> ScollableItemCommand { get; set; }

        public int Index
        {
            get
            {
                return m_Index;
            }
            
            set
            {
                if (value < 0)
                {
                    value = Options.Length - 1;
                }

                m_Index = value;
                m_Index %= Options.Length;
                Text = m_BaseText + Options[m_Index];
                ScollableItemCommand.Invoke(Options[m_Index]);
            }
        }

        public ScrollableMenuItem(string i_Text, string[] i_Options, int i_IndexOfDefaultValue,  string i_FontName, Action<string> i_Action, Game i_Game)
            : base(i_Text, i_Game, i_FontName)
        {
            Options = i_Options;
            m_BaseText = i_Text;
            Text = m_BaseText + Options[m_Index];
            ScollableItemCommand = i_Action;
            m_DefaultIndex = i_IndexOfDefaultValue;
            m_Index = m_DefaultIndex;
            Text = m_BaseText + Options[m_Index];
        }
    }
}
