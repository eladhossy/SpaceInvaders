using System;
using Infrastructure.ObjectModel;
using Infrastructure.ObjectModel.Animators.ConcreteAnimators;
using Infrastructure.ServiceInterfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Infrastructure.ObjectModel.Animators.ConcreteAnimators
{
    public class ShrinkAnimator : SpriteAnimator
    {
        private Vector2 m_ShrinkVectorPerSecond;
    
        public ShrinkAnimator(string i_Name, Vector2 i_ShrinkFactorPerSecond, TimeSpan i_AnimationLength)
            : base(i_Name, i_AnimationLength)
        {
            m_ShrinkVectorPerSecond = i_ShrinkFactorPerSecond;
        }


        protected override void RevertToOriginal()
        {
            this.BoundSprite.Scales = m_OriginalSpriteInfo.Scales;
        }

        protected override void DoFrame(GameTime i_GameTime)
        {
            BoundSprite.Scales -= m_ShrinkVectorPerSecond * (float)i_GameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
