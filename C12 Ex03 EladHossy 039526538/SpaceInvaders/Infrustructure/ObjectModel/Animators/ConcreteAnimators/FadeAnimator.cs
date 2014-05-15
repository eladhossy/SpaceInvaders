using System;
using Infrastructure.ObjectModel;
using Infrastructure.ObjectModel.Animators.ConcreteAnimators;
using Infrastructure.ServiceInterfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Infrastructure.ObjectModel.Animators.ConcreteAnimators
{
    public class FadeAnimator : SpriteAnimator
    {
        private float m_FadeFactorPerSecond;

        public FadeAnimator(string i_Name, float i_FadeFactorPerSecond, TimeSpan i_AnimationLength)
            : base(i_Name, i_AnimationLength)
        {
            m_FadeFactorPerSecond = i_FadeFactorPerSecond;
        }


        protected override void RevertToOriginal()
        {
            this.BoundSprite.Opacity = m_OriginalSpriteInfo.Opacity;
        }

        protected override void DoFrame(GameTime i_GameTime)
        {
            BoundSprite.Opacity -= m_FadeFactorPerSecond * (float)i_GameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
