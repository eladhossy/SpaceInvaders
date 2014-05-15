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
using SpaceInvaders;


namespace Infrastructure.ObjectModel
{
    public class TextWriter : Sprite
    {
        protected SpriteFont m_Font;
        private string m_FontName;
        public string TextToWrite { get; set; }
        public int LineSpacing
        {
            get
            {
                if (m_Font != null)
                {
                    return m_Font.LineSpacing;
                }
                else
                {
                    return 0;
                }
            }
        }


        public TextWriter(Game i_Game, string i_FontName)
            : base(i_Game, string.Empty)
        {
            m_FontName = i_FontName;
        }

       
        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            m_Font = Game.Content.Load<SpriteFont>(m_FontName);
        }
        
        public override void Draw(GameTime gameTime)
        {
            if (TextToWrite != null)
            {
                if (!m_UseSharedBatch)
                {
                    m_SpriteBatch.Begin();
                }
                
                m_SpriteBatch.DrawString(m_Font, TextToWrite, Position, TintColor, Rotation, RotationOrigin, Scales, SpriteEffects.None, 0);

                if (!m_UseSharedBatch)
                {
                    m_SpriteBatch.End();
                }
            }
        }
    }
}
