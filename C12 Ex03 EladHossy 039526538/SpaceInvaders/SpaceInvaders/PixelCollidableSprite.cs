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
    // we decided to inherite from Sprite for 2 reasons: 1) not every sprite 'wants' to be pixel-collidable
    // 2) since we've implemented the abstract injection point - pixel collided, so the derived can inject to the mechanism
    public abstract class PixelCollidableSprite : Sprite 
    {
        protected Color[] m_MyPixels;
        protected Color[] m_OtherSpritePixels;

        public Color[] Pixels
        {
            get
            {
                return m_MyPixels;
            }
        }

        public PixelCollidableSprite(Game game, string i_TextureName)
            : base(game, i_TextureName)
        { 
        }
         
        protected override void InitBounds()
        {
            base.InitBounds();
            m_MyPixels = new Color[Texture.Width * Texture.Height];
            Texture.GetData<Color>(m_MyPixels);
        }

        // when a pixel-collidable sprite is rectangle-based collided, it then check to see if it is pixel collided
        public override void Collided(Infrastructure.ServiceInterfaces.ICollidable i_Collidable)
        {
            Sprite otherSprite = i_Collidable as Sprite;
            m_OtherSpritePixels = new Color[otherSprite.Texture.Width * otherSprite.Texture.Height];
            otherSprite.Texture.GetData<Color>(m_OtherSpritePixels);

            // minimize the area need to be pixel scanned
            int top = Math.Max(Bounds.Top, otherSprite.Bounds.Top);
            int bottom = Math.Min(Bounds.Bottom, otherSprite.Bounds.Bottom);
            int left = Math.Max(Bounds.Left, otherSprite.Bounds.Left);
            int right = Math.Min(Bounds.Right, otherSprite.Bounds.Right);

            // scan the area
            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    int myPixelIndex = (x - Bounds.Left) + ((y - Bounds.Top) * Bounds.Width);
                    int otherSpritePixelIndex = (x - otherSprite.Bounds.Left) + ((y - otherSprite.Bounds.Top) * otherSprite.Bounds.Width);
                    Color color1 = m_MyPixels[myPixelIndex]; // get the pixels at the spot from the 2 sprites
                    Color color2 = m_OtherSpritePixels[otherSpritePixelIndex];

                    // if 2 opaque pixels are on the same place - collision!
                    if (color1.A != 0 && color2.A != 0)
                    {
                        PixelCollided(myPixelIndex, otherSpritePixelIndex, i_Collidable);
                    }
                }
            }
        }

        // injection point, to let the derived classes implement the concrete behaviors
        protected abstract void PixelCollided(int i_MyPixelIndex, int i_OtherPixelIndex, ICollidable i_Collidable);
    }
}