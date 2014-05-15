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
using Infrastructure.ServiceInterfaces;

namespace SpaceInvaders
{
    public class Barrier : PixelCollidableSprite, ICollidable
    {
        SoundBank m_SoundBank;
        string m_HitCueName;

        public Barrier(Game game, string i_TextureNameNumber)
            : base(game, string.Format(@"Sprites\Barrier{0}_44x32", i_TextureNameNumber))
        {
        }

        public void SetHitSound(SoundBank i_SoundBank, string i_CueName)
        {
            m_SoundBank = i_SoundBank;
            m_HitCueName = i_CueName;
        }
        
        protected override void PixelCollided(int i_MyPixelIndex, int i_OtherPixelIndex, ICollidable i_Collidable)
        {
          

            m_MyPixels[i_MyPixelIndex] = new Color(0, 0, 0, 0);
            Sprite otherSprite = i_Collidable as Sprite;
            int signMult;
            if (otherSprite.Velocity.Y > 0)
            {
                signMult = 1;
            }
            else
            {
                signMult = -1;
            }
            
            int defpthOfHit = (int)Math.Round(0.8 * otherSprite.Height);
            Rectangle rec = otherSprite.Bounds;
            rec.Location = new Point(rec.Location.X, rec.Location.Y + (defpthOfHit * signMult));
            int top = Math.Max(Bounds.Top, rec.Top);
            int bottom = Math.Min(Bounds.Bottom, rec.Bottom);
            int left = Math.Max(Bounds.Left, rec.Left);
            int right = Math.Min(Bounds.Right, rec.Right);

            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    int myPixelIndex = (x - Bounds.Left) + ((y - Bounds.Top) * Bounds.Width);
                    int otherSpritePixelIndex = (x - rec.Left) + ((y - otherSprite.Bounds.Top - (defpthOfHit * signMult)) * rec.Width);
                    Color color1 = m_MyPixels[myPixelIndex];
                    Color color2 = m_OtherSpritePixels[otherSpritePixelIndex];

                    if (color1.A != 0 && color2.A != 0)
                    {
                        m_MyPixels[myPixelIndex] = new Color(0, 0, 0, 0);
                    }
                }
            }

           // otherSprite.Dispose();
            if (otherSprite is IBullet)
            {
                if (m_SoundBank != null)
                {
                    m_SoundBank.PlayCue(m_HitCueName);
                }
                otherSprite.Dispose();
            }
            Texture.SetData(m_MyPixels);
            
        }

        public void ReportPositionChanged()
        {
            OnPositionChanged();
        }
    }
}
