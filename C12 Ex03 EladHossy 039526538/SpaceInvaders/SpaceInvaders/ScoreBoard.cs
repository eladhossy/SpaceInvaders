using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Infrastructure.ObjectModel;

namespace SpaceInvaders
{
    public class ScoreBoard : TextWriter
    {
        private const string k_FontName = @"Fonts\Calibri14";
        private List<Player> m_Players;

        public ScoreBoard(Game game, List<Player> i_Players)
            : base(game, k_FontName)
        {
            m_Players = i_Players;
        }

        public override void Draw(GameTime gameTime)
        {
            int i = 0;
            foreach (Player player in m_Players)
            {
                TextToWrite = string.Format("{0} Score: {1}\n", player.Name, player.Scores);
                m_SpriteBatch.DrawString(m_Font, TextToWrite, Position + new Vector2(10, i * 18), player.Color);
                i++;
            }
        }
    }
}
