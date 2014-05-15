using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace SpaceInvaders
{
    public class SoulIcon : Sprite
    {
        public SoulIcon(Game i_Game, string i_TextureName)
            : base(i_Game, i_TextureName)
        {
           m_TextureName = i_TextureName;
        }

        public override void Initialize()
        {
            Opacity = 0.5f;
            Scales = new Vector2(0.5f);

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
