//*** Guy Ronen © 2008-2011 ***//
using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Infrastructure.ObjectModel.Animators.ConcreteAnimators
{
    public class CelAnimator : SpriteAnimator
    {
        public TimeSpan FrameLength { get; set; }
        private TimeSpan m_TimeLeftForFrame;
        private bool m_Loop = true;
        private List<int> m_CelIndexes = new List<int>();
        private int m_CurrCellIndex = 0;
     

        // CTORs
        public CelAnimator(TimeSpan i_FrameLength, TimeSpan i_AnimationLength, params int[] celIndexes)
            : base("CelAnimation", i_AnimationLength)
        {
            this.FrameLength = i_FrameLength;
            this.m_TimeLeftForFrame = i_FrameLength;
            for (int i = 0; i < celIndexes.Length; i++)
            {
                m_CelIndexes.Add(celIndexes[i]);
            }

            m_Loop = i_AnimationLength == TimeSpan.Zero;
        }

        public void NextFrame()
        {
            m_CurrCellIndex++;
            if (m_CurrCellIndex >= m_CelIndexes.Count)
            {
                if (m_Loop)
                {
                    m_CurrCellIndex = 0;
                }
                else
                {
                    m_CurrCellIndex = m_CelIndexes.Count; // lets stop at the last frame
                    this.IsFinished = true;
                }
            }
        }

        protected override void RevertToOriginal()
        {
            this.BoundSprite.SourceRectangle = m_OriginalSpriteInfo.SourceRectangle;
        }

        protected override void DoFrame(GameTime i_GameTime)
        {
            if (FrameLength != TimeSpan.Zero)
            {
                m_TimeLeftForFrame -= i_GameTime.ElapsedGameTime;
                if (m_TimeLeftForFrame.TotalSeconds <= 0)
                {
                    // we have elapsed, so blink
                    NextFrame();
                    m_TimeLeftForFrame = FrameLength;
                }
            }

            this.BoundSprite.SourceRectangle = new Rectangle(
                m_CelIndexes[m_CurrCellIndex] * this.BoundSprite.SourceRectangle.Width,
                this.BoundSprite.SourceRectangle.Top,
                this.BoundSprite.SourceRectangle.Width,
                this.BoundSprite.SourceRectangle.Height);
        }
    }
}
