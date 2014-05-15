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
using Infrastructure.ObjectModel.Screens;

namespace SpaceInvaders
{
    public class SoulsBoard : Microsoft.Xna.Framework.GameComponent
    {
        private Dictionary<Player, string> m_PlayersAndSoulTextures;
        
        private Dictionary<Player, List<SoulIcon>> m_PlayersAndSoulIcons = new Dictionary<Player, List<SoulIcon>>();
        
        public Vector2 Position { get; set; }

        private GameScreen m_ContainingScreen;

        public SoulsBoard(Game i_Game, Dictionary<Player, string> i_PlayersAndSoulTextures, GameScreen i_GameScreen)
            : base(i_Game)
        {
            m_PlayersAndSoulTextures = i_PlayersAndSoulTextures;
            m_ContainingScreen = i_GameScreen;
            populateIconsForPlayers();
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public void locateIconsPositions()
        {
            List<Player> players = m_PlayersAndSoulTextures.Keys.ToList<Player>();
            int VerticalMult = 0;
            foreach (Player player in players)
            {
                List<SoulIcon> soulIcons = m_PlayersAndSoulIcons[player];
                int horizontalMult = 1;
                foreach (SoulIcon soulIcon in soulIcons)
                {
                    float x = this.Position.X - (soulIcon.Width * horizontalMult);
                    float y = this.Position.Y + (soulIcon.Height * VerticalMult);
                    horizontalMult++;
                    soulIcon.Position = new Vector2(x, y);
                }

                VerticalMult++;
            }
        }

        private void populateIconsForPlayers()
        {
            List<Player> players = m_PlayersAndSoulTextures.Keys.ToList<Player>();
            string textureName;
            foreach (Player player in players)
            {
                player.SoulIsDead += new SoulIsDeadEventHandler(player_SoulIsDead); // to react when a soul is dead, and remove one icon from the soulboard
                textureName = m_PlayersAndSoulTextures[player];
                m_PlayersAndSoulIcons.Add(player, new List<SoulIcon>());
                for (int i = 0; i < player.Souls; i++)
                {
                    SoulIcon newSoulIcon = new SoulIcon(Game, textureName);
                    m_PlayersAndSoulIcons[player].Add(newSoulIcon);
                    m_ContainingScreen.Add(newSoulIcon);
                }
            }
        }

        private void player_SoulIsDead(Player player) // when notified, remove a soul icon from the board
        {
            List<SoulIcon> playerSoulIcons = m_PlayersAndSoulIcons[player];
            int numOfIcons = playerSoulIcons.Count;
            if (numOfIcons > 0)
            {
                SoulIcon soulIconToRemove = playerSoulIcons[numOfIcons - 1];
                m_ContainingScreen.Remove(soulIconToRemove);
                playerSoulIcons.Remove(soulIconToRemove);
                soulIconToRemove.Dispose();
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
