using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SpaceInvaders
{
    public delegate void SoulIsDeadEventHandler(Player player);

    public class Player
    {
        public event SoulIsDeadEventHandler SoulIsDead;
        
        public int Scores { get; set; }
       
        public int Souls { get; set; }
        
        public string Name { get; set; }
        
        public Color Color { get; set; }

        public Player()
        {
            Souls = 3;
            Scores = 0;
        }

        public void KillSoul()
        {
            Souls--;
            SoulIsDead.Invoke(this);
        }
    }
}
